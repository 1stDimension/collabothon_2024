# Import the Speech-to-Text client library
from google.cloud import speech
import os

os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = "lodzkiterror-65599eb0142d.json"
# Instantiates a client
client = speech.SpeechClient()

# The content of the audio file to transcribe
import scipy.io.wavfile as wav
import base64  # for encoding to byte string

# Read the WAV file
sample_rate, audio_data = wav.read("pizza_voice_sample.wav")

# Convert audio data to byte string (assuming PCM encoding)
audio_content = base64.b64encode(audio_data.tobytes()).decode('utf-8')

# transcribe speech
audio = speech.RecognitionAudio(content=audio_content)

config = speech.RecognitionConfig(
    encoding=speech.RecognitionConfig.AudioEncoding.LINEAR16,
    sample_rate_hertz=44100,
    language_code="de-DE",
    model="default",
    audio_channel_count=2,
    enable_word_confidence=True,
    enable_word_time_offsets=True,
)

# Detects speech in the audio file
operation = client.long_running_recognize(config=config, audio=audio)

print("Waiting for operation to complete...")
response = operation.result(timeout=90)

for result in response.results:
  print("Transcript: {}".format(result.alternatives[0].transcript))

### return promt to vertex search