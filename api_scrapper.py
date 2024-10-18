import json
import requests


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


print(callApiSbx("/accounts-api/21/v1/", "/accounts/130471100000EUR" , token=token, printBody=True))








