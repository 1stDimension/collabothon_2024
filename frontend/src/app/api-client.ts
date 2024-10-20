interface IRecognizeResponse {
  transactionId: string;
}

interface IRecognizeResult {
  text: string;
}

export type TranscriptionCallback = (text: string) => void;

export class ApiClient {
  async requestTranscription(pcm: Uint8Array, sampleRate: number, channels: number, resolution: number): Promise<string> {
    const data = new FormData();
    data.append("pcm", new Blob([ pcm ]));
    data.append("sampleRate", sampleRate.toString());
    data.append("channels", channels.toString());
    data.append("resolution", resolution.toString());

    const response = await fetch("/vertex/recognize", {
      method: "POST",
      body: data,
    });

    const result = await response.json() as IRecognizeResponse;
    return result.transactionId;
  }

  async awaitTranscription(transactionId: string): Promise<string> {
    const result = new Promise<string>((resolve, reject) => {
      const socket = new WebSocket(`/vertex/notify/${transactionId}`);
      socket.onclose = () => {
        reject();
      };
      socket.onmessage = e => {
        const data = JSON.parse(e.data) as IRecognizeResult;
        resolve(data.text);
      };
    });

    return result;
  }
};
