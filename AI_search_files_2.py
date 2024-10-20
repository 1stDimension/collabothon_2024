from google.cloud import discoveryengine_v1
from html import unescape
from html.parser import HTMLParser
from io import StringIO
import os
os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = "lodzkiterror-65599eb0142d.json"

class MLStripper(HTMLParser):
    def __init__(self):
        super().__init__()
        self.reset()
        self.strict = False
        self.convert_charrefs= True
        self.text = StringIO()
    def handle_data(self, d):
        self.text.write(d)
    def get_data(self):
        return self.text.getvalue()

def strip_tags(html):
    s = MLStripper()
    s.feed(html)
    return s.get_data()

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

stor_client = storage.Client()

# Process the response and extract the desired information
for entity in response.results:
    dat = entity.document.derived_struct_data
    if "link" in dat:
        print("FOUND:", dat["link"])
        if "snippets" in dat:
            print("SNIPPET:", strip_tags(unescape(dat["snippets"][0]["snippet"])))
    else:
        print("NO LINK")
