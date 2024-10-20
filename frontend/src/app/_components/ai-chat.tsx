"use client";

import { useState } from "react";
import { ApiClient, ISearchResult } from "../api-client";

interface Props {
  active: boolean;
  onDismiss: () => void;
};

interface IAiMessage {
  text: string | null;
  isUser: boolean;
  url?: string;
}

const api = new ApiClient();

function mergeBuffers(channelBuffer: Float32Array[], recordingLength: number): Float32Array {
   const result = new Float32Array(recordingLength);
   let offset = 0;

   for (let i = 0; i < channelBuffer.length; i++) {
       result.set(channelBuffer[i], offset);
       offset += channelBuffer[i].length;
   }

   return result.slice();
}

function toPCM16(samples: Float32Array): Uint16Array {
  const PCM16iSamples = [];

  for (let i = 0; i < samples.length; i++)
  {
     let val = Math.floor(32767 * samples[i]);
     val = Math.min(32767, val);
     val = Math.max(-32768, val);

     PCM16iSamples.push(val);
  }

  return new Uint16Array(PCM16iSamples);
}

async function processAudio(buffers: Float32Array[], totalSampleCount: number, sampleRate: number, transcribed: (text: string) => void) {
  const f32 = mergeBuffers(buffers, totalSampleCount);
  const u16 = toPCM16(f32);
  const pcmBytes = new Uint8Array(u16.buffer, u16.byteOffset, u16.byteLength);

  const transactionId = await api.requestTranscription(pcmBytes, sampleRate, 1, 16);
  const result = await api.awaitTranscription(transactionId);
  transcribed(result);
}

async function recordAudio(transcribed: (text: string) => void, recordCallback: () => void) {
  const pcm32: Float32Array[] = [];
  let totalSampleCount = 0;

  const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
  const audioContext = new AudioContext();
  const sampleRate = audioContext.sampleRate;
  const volume = audioContext.createGain();
  const audioInput = audioContext.createMediaStreamSource(stream);
  const analyser = audioContext.createAnalyser();;
  analyser.minDecibels = -45;
  audioInput.connect(volume);
  audioInput.connect(analyser);

  const bufferLength = analyser.frequencyBinCount;
  const domainData = new Uint8Array(bufferLength);

  let silentChunks = 0;
  let hadAny = false;
  const detectSound = () => {
    analyser.getByteFrequencyData(domainData);
    for (let i = 0; i < bufferLength; i++) {
      if (domainData[i] > 0) {
        silentChunks = 0;
        hadAny = true;
        window.requestAnimationFrame(detectSound);
        return;
      }
    }

    if (hadAny) {
      ++silentChunks;
      if (silentChunks > 50) {
        console.log("silent chunk! cut!");
        recorder.disconnect();
        volume.disconnect();
        audioInput.disconnect();
        analyser.disconnect();
        recordCallback();
        processAudio(pcm32, totalSampleCount, sampleRate, transcribed);
        return;
      }
    }

    window.requestAnimationFrame(detectSound);
  };
  window.requestAnimationFrame(detectSound);

  const recorder = audioContext.createScriptProcessor(2048, 1, 1);
  recorder.onaudioprocess = e => {
    const samples = e.inputBuffer.getChannelData(0);
    const samples32 = new Float32Array(samples);
    pcm32.push(samples32);
    totalSampleCount += samples32.length;
  };

  volume.connect(recorder);
  recorder.connect(audioContext.destination);
}

async function search(query: string): Promise<ISearchResult> {
  const results = await api.search(query);
  return results;
}

export default function AiChatPanel({ active, onDismiss } : Props) {
  const [ messages, setMessages ] = useState<IAiMessage[]>([]);
  const [ recording, setRecording ] = useState(false);
  const [ userInput, setUserInput ] = useState("");
  const [ isProcessing, setIsProcessing ] = useState(false);
  const [ isThinking, setIsThinking ] = useState(false);
  const addUserMessage = (text: string | null) => {
    console.log("received user message:", text);
    setIsProcessing(false);
    setMessages((prev) => [ ...prev, { isUser: true, text } ])

    if (text === null)
      return;

    setIsThinking(true);
    search(text).then(x => {
      if (x.files.length < 1) {
        addSystemMessage(null);
        return;
      }

      const file = x.files[0];
      if (!!file.snippet || !!file.url) {
        // TODO: tts
        addSystemMessage(file.snippet, file.url);
      }
    });
  };
  const addSystemMessage = (text: string | null, url?: string) => {
    console.log("received system message:", text);
    setIsThinking(false);
    setMessages((prev) => [ ...prev, { isUser: false, text, url } ])
  };

  return (
    <div style={{
      position: "absolute",
      top: 0,
      right: 0,
      bottom: 0,
      left: 0,
      background: 'rgba(0, 0, 0, 0.3)',
      opacity: active ? 1 : 0,
      display: "flex",
      pointerEvents: active ? 'auto' : 'none',
      transition: 'opacity 0.3s',
      flexDirection: "row",
    }}>
      {
        // grayout pane
      }
      <div onClick={ () => onDismiss() } style={{
        flex: '1 0 auto'
      }}>&nbsp;</div>

      {
        // right pane
      }
      <div style={{
        position: "absolute",
        top: 0,
        right: 0,
        bottom: 0,
        width: '30%',
        background: 'white',
        display: "flex",
        flexDirection: "column",
        padding: "12px",
      }}>
        {
          // top bar
        }
        <div style={{
          flex: "0 0 auto",
          padding: "8px",
          fontSize: '16pt',
          fontWeight: 700,
          display: "flex",
          flexDirection: "row",
        }}>
          <span style={{ flex: "1 0 auto" }}>Ask your AI assistant</span>
          <button style={{ flex: "0 0 auto", aspectRatio: 1, border: "none", background: "transparent" }} onClick={ onDismiss }>
            ✖
          </button>
        </div>

        {
          // message pane
        }
        <div style={{
          flex: "0 0 auto",
          padding: "8px",
          display: "grid",
          gridTemplateColumns: "48px auto",
          gap: "20px 8px",
          alignItems: "end",
        }}>
          { messages.map((x, i) => <div key={i} style={{ display: "contents" }}><div style={{
            boxSizing: "border-box",
            width: "48px",
            height: "48px",
            lineHeight: "48px",
            textAlign: "center",
            borderRadius: "24px",
            background: "#eee",
          }}>{ x.isUser ? "👤" : "✨" }</div><div style={{
            border: "1px solid #ddd",
            background: x.isUser ? "transparent" : "#ddd",
            borderRadius: '8px',
            padding: '6px 18px',
            fontSize: '13pt',
            lineHeight: '36px',
          }}>
            { x.text !== null ? x.text : <i>{ x.isUser ? "Sorry, didn't quite catch that" : "Sorry, didn't find anything" }</i> }
            { x.url !== null && x.url !== undefined ? <><hr style={{ borderTop: "1px solid #999" }} /><a href={x.url} target="_blank">📄 See document</a></> : <></> }
          </div></div>) 
          }
          {
            isProcessing ? <div style={{ display: "contents" }}><div style={{
              boxSizing: "border-box",
              width: "48px",
              height: "48px",
              lineHeight: "48px",
              textAlign: "center",
              borderRadius: "24px",
              background: "#eee",
            }}>✨</div><div style={{
              border: "1px solid #ddd",
              background: "transparent",
              borderRadius: '8px',
              padding: '6px 18px',
              fontSize: '13pt',
              lineHeight: '36px',
            }}><i>processing...</i></div></div> : <></>
          }
          {
            isThinking ? <div style={{ display: "contents" }}><div style={{
              boxSizing: "border-box",
              width: "48px",
              height: "48px",
              lineHeight: "48px",
              textAlign: "center",
              borderRadius: "24px",
              background: "#eee",
            }}>✨</div><div style={{
              border: "1px solid #ddd",
              background: "#ddd",
              borderRadius: '8px',
              padding: '6px 18px',
              fontSize: '13pt',
              lineHeight: '36px',
            }}><i>Thinking...</i></div></div> : <></>
          }
        </div>

        {
          // filler
        }
        <div style={{ flex: "1 0 auto" }}>&nbsp;</div>

        {
          //input bar
        }
        <div style={{
          flex: "0 0 auto",
          padding: "8px",
          display: "flex",
          flexDirection: "row",
          gap: '12px'
        }}>
          <div style={{
            flex: "1 0 auto",
            boxSizing: 'border-box',
            height: '48px',
            borderRadius: '24px',
            border: '2px solid #999',
            paddingLeft: '8px',
            paddingRight: '8px',
          }}>
            <input
              type="text"
              placeholder="Ask me about anything you want to know..."
              value={ userInput }
              onChange={ e => setUserInput(e.target.value) }
              style={{
                display: "block",
                width: '100%',
                height: '100%',
                lineHeight: '48px',
                border: 'none',
                background: 'transparent',
              }} />
          </div>
          <button onClick={ () => { addUserMessage(userInput); setUserInput(""); } } style={{
            flex: "0 0 auto",
            width: '48px',
            height: '48px',
            lineHeight: '48px',
            fontSize: '24px',
            background: '#ccc',
            borderRadius: '24px',
            boxSizing: 'border-box',
          }}>▶</button>
          <button
            onClick={ () => { if (!recording && !isProcessing) { setRecording(true); recordAudio(addUserMessage, () => { setRecording(false); setIsProcessing(true); }); } } }
            disabled={ recording || isProcessing }
            style={{
              flex: "0 0 auto",
              width: '48px',
              height: '48px',
              lineHeight: recording ? '40px' : '48px',
              fontSize: '24px',
              background: '#ccc',
              borderRadius: '24px',
              boxSizing: 'border-box',
              border: recording ? '4px solid red' : 'none',
          }}>🎤</button>
        </div>
      </div>
    </div>
  )
}
