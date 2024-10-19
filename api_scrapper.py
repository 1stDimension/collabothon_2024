import json
import requests
import passwords
from google.cloud import storage
import os
import uuid
from google.cloud import storage 

class ClientCredentials():
    client_id = ""
    client_secret = ""

    def __init__(self, client_id, client_secret):
        self.client_secret = client_secret
        self.client_id = client_id

    def getClientId(self):
        return str(self.client_id)

#Track changes

    def getClientSecret(self):
        return str(self.client_secret)

cc = ClientCredentials("e5fd58c3-1e95-4376-9273-7851d24dba61", "75d24ff6-cc94-428c-b12e-a91b778e8463")



def getTokenSbxClientCredentials(cc):
    url = "https://api-sandbox.commerzbank.com/auth/realms/sandbox/protocol/openid-connect/token"

    payload = ("grant_type=client_credentials&client_id=" + cc.getClientId() + "&client_secret=" + cc.getClientSecret())

    headers = {
        'Content-Type': 'application/x-www-form-urlencoded'
    }

    response = requests.request("POST", url, headers=headers, data=payload)
    #print('token request outcome: ' + str(response.status_code))

    token = json.loads(response.text)['access_token']
    return token

token = getTokenSbxClientCredentials(cc)
#print(token)

def callApiSbx(basepath, endpoint, token, method="GET", query="", CAIDRequired=False, CAID="", printBody=False):
    url = "https://api-sandbox.commerzbank.com" + basepath + endpoint
    if query != "":
        url = url + "/" + query
    print(url)
    headers = {
        'Authorization': 'Bearer ' + token
    }
    if CAIDRequired:
        headers.update({
            'Coba-ActivityID': CAID
        })
    response = requests.request(method, url, headers=headers)
    print('API request outcome: ' + str(response.status_code))
    if printBody:
        print(response.text)

    return response


######################################
### GCP
###################################

import os

# Set the GOOGLE_APPLICATION_CREDENTIALS environment variable
def upload_file_to_bucket(bucket_name, destination_file_name, content):
    """Uploads a file to the bucket with a new unique name."""
    
    # Set the environment variable for Google Cloud credentials
    os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = "lodzkiterror-65599eb0142d.json"

    # Generate a unique destination blob name
    unique_file_name = destination_file_name[:-4] # Generates a unique file name using UUID
    destination_blob_name = f'{unique_file_name}.txt'  # Add extension as needed

    # Create a Cloud Storage client
    storage_client = storage.Client()

    # Get the bucket
    bucket = storage_client.bucket(bucket_name)

    # Create a blob object with the unique file name
    blob = bucket.blob(destination_blob_name)

    # Upload the file
    # blob.upload_from_filename(source_file_name)
    blob.upload_from_string(content)

    print(f"File {destination_file_name} uploaded as {destination_blob_name} in {bucket_name} bucket.")


# Example usage
bucket_name = 'lodzkiterror'  # Replace with your bucket name
account_id = "130471100000EUR"


##### end of declarations"""

account_id = "130471100000EUR"

api_base_path = "/accounts-api/21/v1/"
destination_file_name = f"{api_base_path.replace('/', '_').replace('-', '_')[1:]}{account_id}"# Path to the local file
#api_response_account = str(callApiSbx(f"{api_base_path}", f"/accounts/{account_id}" , token=token, printBody=False))

# upload_file_to_bucket(bucket_name,  destination_file_name=destination_file_name, content=api_response_account)


ap_transations = f"/securities-api/v4" # appi path
df_transactions = f"{ap_transations.replace('/', '_').replace('-', '_')[1:]}{account_id}"# Path to the local file

#securities-api/v4/accounts/XXXXXXXXXXXXXXXXXXXXXX-00/portfolio?
res_transations = str(callApiSbx(f"securities-api/v4", f"accounts//healthcheck" , token=token, printBody=False)) #api response
print(res_transations)



