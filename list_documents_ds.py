from typing import Any

from google.api_core.client_options import ClientOptions
from google.cloud import discoveryengine

import os

os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = "lodzkiterror-65599eb0142d.json"

# TODO(developer): Uncomment these variables before running the sample.
project_id = "lodzkiterror"
location = "eu"            # Values: "global", "us", "eu"
data_store_id = "dsingestion_1729302866431"


def list_documents_sample(project_id: str, location: str, data_store_id: str) -> Any:
    #  For more information, refer to:
    # https://cloud.google.com/generative-ai-app-builder/docs/locations#specify_a_multi-region_for_your_data_store
    client_options = (
        ClientOptions(api_endpoint=f"{location}-discoveryengine.googleapis.com")
        if location != "global"
        else None
    )

    # Create a client
    client = discoveryengine.DocumentServiceClient(client_options=client_options)

    # The full resource name of the search engine branch.
    # e.g. projects/{project}/locations/{location}/dataStores/{data_store_id}/branches/{branch}
    parent = client.branch_path(
        project=project_id,
        location=location,
        data_store=data_store_id,
        branch="default_branch",
    )

    response = client.list_documents(parent=parent)

    print(f"Documents in {data_store_id}:")
    for result in response:
        print(result)

    return response

list_documents_sample(project_id=project_id, location=location, data_store_id=data_store_id)