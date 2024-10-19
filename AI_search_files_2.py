from google.cloud import discoveryengine_v1
import os
os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = "lodzkiterror-65599eb0142d.json"

# Replace these placeholders with your actual values
project_id = "lodzkiterror"
location = "eu"
collection_id = "default_collection"
engine_id = "genericagentingestion_1729302945333"
serving_config = "default_search"
query = "how to make pizza"  # Replace with your actual search query

# Authenticate using Google Cloud Application Default Credentials
client = discoveryengine_v1.DiscoveryEngineClient()

# Construct the search request
request = discoveryengine_v1.SearchRequest(
    query=query,
    page_size=10,
    query_expansion_spec=discoveryengine_v1.QueryExpansionSpec(
        condition=discoveryengine_v1.QueryExpansionSpec.Condition.AUTO
    ),
    spell_correction_spec=discoveryengine_v1.SpellCorrectionSpec(
        mode=discoveryengine_v1.SpellCorrectionSpec.Mode.AUTO
    ),
    name=f"projects/{project_id}/locations/{location}/collections/{collection_id}/engines/{engine_id}/servingConfigs/{serving_config}:search"
)

# Send the request and get the response
response = client.search(request)

# Process the response and extract the desired information
for entity in response.results:
    print(entity.name)
    for property in entity.properties:
        print(f"{property.key}: {property.value}")