# Imports the Google Cloud Translation library
from google.cloud import translate_v3 as translate
import os
os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = "lodzkiterror-65599eb0142d.json"


DATASET_ID = "example-set"  # change this to any dataset ID

def create_adaptive_mt_dataset():
    # Create a client
    client = translate.TranslationServiceClient()
    # Initialize request argument(s)
    adaptive_mt_dataset = translate.types.AdaptiveMtDataset()
    adaptive_mt_dataset.name = f"projects/lodzkiterror/locations/us-central1/adaptiveMtDatasets/{DATASET_ID}"
    adaptive_mt_dataset.display_name = "Example set"
    adaptive_mt_dataset.source_language_code = "de"
    adaptive_mt_dataset.target_language_code = "en"
    request = translate.CreateAdaptiveMtDatasetRequest(
        parent="projects/lodzkiterror/locations/us-central1",
        adaptive_mt_dataset=adaptive_mt_dataset,
    )
    # Make the request
    response = client.create_adaptive_mt_dataset(request=request)
    # Handle the response
    print(response)

def import_adaptive_mt_file():
    # Create a client
    client = translate.TranslationServiceClient()

    # Make the request
    response = client.import_adaptive_mt_file(request)
    # Handle the response
    print(response)

def adaptive_mt_translate():
    text1 = """You are an expert Translator. You are tasked to translate documents from en to fr.Please provide an accurate translation of this document and return translation text only:"""
    # Create a client
    client = translate.TranslationServiceClient()
    # Initialize the request
    request = translate.AdaptiveMtTranslateRequest(
        parent="projects/lodzkiterror/locations/us-central1",
        dataset=f"projects/lodzkiterror/locations/us-central1/adaptiveMtDatasets/{DATASET_ID}",
        content=[text1]
    )
    # Make the request
    response = client.adaptive_mt_translate(request)
    # Handle the response
    print(response)

create_adaptive_mt_dataset()
import_adaptive_mt_file()
adaptive_mt_translate()