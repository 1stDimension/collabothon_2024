from fastapi import FastAPI, File, UploadFile, Form, WebSocket
from google.cloud import speech
from base64 import b64encode
from asyncio import Event, Future
from uuid import uuid4
from dataclasses import dataclass
from typing import Optional, Union
from functools import partial
from os import environ
from pydantic import BaseModel

@dataclass
class RecognizeData:
    event: Event
    result: Optional[str]
    id: str

class RecognitionResponse(BaseModel):
    transactionId: str

class ErrorResponse(BaseModel):
    error: str

environ["GOOGLE_APPLICATION_CREDENTIALS"] = "lodzkiterror-65599eb0142d.json"
app = FastAPI(debug=True)
TRANSACTIONS: dict[str, RecognizeData] = {}

@app.post("/vertex/recognize")
async def recognize_audio(
    pcm: UploadFile = File(...), 
    sampleRate: int = Form(...), 
    channels: int = Form(...), 
    resolution: int = Form(...)
) -> Union[RecognitionResponse, ErrorResponse]:
    """
    Endpoint for recognizing speech in PCM audio data.
    """

    global TRANSACTIONS

    try:
        client = speech.SpeechClient()
        pcm64 = b64encode(await pcm.read()).decode("utf-8")
        audio = speech.RecognitionAudio(content=pcm64)
        config = speech.RecognitionConfig(
            encoding={
                16: speech.RecognitionConfig.AudioEncoding.LINEAR16,
                8: speech.RecognitionConfig.AudioEncoding.MULAW,
            }[resolution],
            sample_rate_hertz=sampleRate,
            language_code="en-US",
            model="default",
            audio_channel_count=channels,
            enable_word_confidence=True,
            enable_word_time_offsets=True,
        )

        client.long_running_recognize(config=config, audio=audio)
        transactionId = str(uuid4())
        TRANSACTIONS[transactionId] = RecognizeData(Event(), None, transactionId)

        operation = client.long_running_recognize(config=config, audio=audio)
        operation.add_done_callback(partial(_recognize_callback, TRANSACTIONS[transactionId]))

        return RecognitionResponse(transactionId=transactionId)

    except Exception as e:
        return ErrorResponse(error=str(e))

@app.websocket("/vertex/notify/{transactionId}")
async def notify_socket(websocket: WebSocket, transactionId: str):
    global TRANSACTIONS
    await websocket.accept()
    
    if not (data := TRANSACTIONS.get(transactionId, None)):
        return

    TRANSACTIONS.pop(transactionId)
    await data.event.wait()
    await websocket.send_json({"text": data.result})
    await websocket.close()

def _recognize_callback(data: RecognizeData, future: Future):
    global TRANSACTIONS
    response = future.result()
    conf = -1
    for result in response.results:
        for alternative in result.alternatives:
            transcript = alternative.transcript
            confidence = alternative.confidence
            data.result = transcript if confidence > conf else data.result

    data.event.set()
