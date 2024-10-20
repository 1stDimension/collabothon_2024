from google.cloud import discoveryengine_v1beta as discoveryengine_v1
import os
os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = "lodzkiterror-65599eb0142d.json"

# Replace these placeholders with your actual values
project_id = "lodzkiterror"
location = "global"
collection_id = "default_collection"
engine_id = "genericagentingestion-glob_1729410060971"
serving_config = "default_search"
query = "how to make pizza"  # Replace with your actual search query

# Authenticate using Google Cloud Application Default Credentials
client = discoveryengine_v1.SearchServiceClient()

# Construct the search request
request = discoveryengine_v1.SearchRequest(
    query=query,
    page_size=10,
    # query_expansion_spec=discoveryengine_v1.SearchRequest.QueryExpansionSpec(
    #     condition=discoveryengine_v1.SearchRequest.QueryExpansionSpec.Condition.AUTO
    # ),
    spell_correction_spec=discoveryengine_v1.SearchRequest.SpellCorrectionSpec(
        mode=discoveryengine_v1.SearchRequest.SpellCorrectionSpec.Mode.AUTO
    ),
    serving_config=f"projects/{project_id}/locations/{location}/collections/{collection_id}/engines/{engine_id}/servingConfigs/{serving_config}"
)

# Send the request and get the response
response = client.search(request)

# Process the response and extract the desired information
for entity in response.results:
    print(entity.document.derived_struct_data["link"])
    # for snippet in entity.document.derived_struct_data:
    #     print(snippet)