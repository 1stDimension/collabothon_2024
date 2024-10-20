from json import JSONDecodeError
from click import clear
from api_scrapper import *
from pprint import pprint
import httpx

# upload_file_to_bucket()

client,secret = os.environ["CLIENT"],os.environ["SECRET"]

cred = ClientCredentials(client,secret)

# token = getTokenSbxClientCredentials(cc)

base = "/securities-api/v4"
endpoint = "/accounts"
r_url = "localhost:8000/"


def get_OauthURL(client_id: str, redirect_url: str):
    query = {
        "response_type": "code",
        "client_id": client_id,
        # "redirect_uri": redirect_url
    }
    url = "https://api-sandbox.commerzbank.com/auth/realms/sandbox/protocol/openid-connect/auth"
    resp = httpx.get(url,params=query)
    print(resp)
    # print(resp.text)
    print(resp.request.url)

def get_token(auth_code: str, client_id: str,redirect_url:str,secret:str)-> dict:
    url ="https://api-sandbox.commerzbank.com/auth/realms/sandbox/protocol/openid-connect/token"
    h = {
        "Content-Type": "application/x-www-form-urlencoded"
    }
    q = {
          "grant_type": "authorization_code",
          "client_id": client_id,
          "client_secret": secret,
          "code": auth_code,
          "redirect_uri": redirect_url  # Doesn't work if you have multiple ones defined
        
    }
    r = httpx.post(url,headers=h,data=q)
    print(r.json())
    return r.json()



# get_OauthURL(client,r_url)

auth= "23e697ca-031b-4422-b2a9-0bd838de6a75.e2479ed7-a041-4887-bd4d-9e8e44182a3a.fc9b2704-c03b-4a7f-b286-cc464f0495be"
# get_token(auth,client,r_url,secret)

token = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJfY2pYNi12TUg3aGJLVjhrVEZ6eXVmckcxT21pelVtWi1TdGk4Y0RwSlpZIn0.eyJleHAiOjE3MjkzNTc0ODksImlhdCI6MTcyOTM1NjU4OSwiYXV0aF90aW1lIjoxNzI5MzU2MDI3LCJqdGkiOiI1Yjc1NWUzMC03YzAwLTRlZjEtYjY3Mi03N2E4YTM5YWI5NDMiLCJpc3MiOiJodHRwczovL3Rva2VubWFuYWdlci1zYnguaW50cmFuZXQuY29tbWVyemJhbmsuY29tL2F1dGgvcmVhbG1zL3NhbmRib3giLCJzdWIiOiIwNjA5OWRhOC04MGE1LTRhNzktODFlMi05NjhiZjU3NzExZGIiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiI5Nzc1OWY4Ny1iZmYwLTQ0MDgtOWE2MS03NzllMmFlYzI0NjUiLCJzZXNzaW9uX3N0YXRlIjoiZTI0NzllZDctYTA0MS00ODg3LWJkNGQtOWU4ZTQ0MTgyYTNhIiwiYWNyIjoiMCIsInNjb3BlIjoiIn0.cXiXbPri9vXK2_-crWyg-SLh9eW32mg0ffo5v4466rXZ8vhR6svlhkc8CWG4FwozlwCW2SficvbsRURScquRhaoXaWWKQbe5LUpfPIcB30IFcSJ6oJ29c6nkwAQMQ5c9mhAAIKJIRZkIiL0IypGH3_jwps48Qiu9JS8kr4toJaDTR9HdJNkdlNu6DD5xje8KCT0bdnv3rxGOv7Br6edQuM_6ZXqfCALf6tzw0BSx0DGCObc6diKUYvXriAFnpIPPZDP820uC_hsZxF3OjjYITSTCuac7zm3y4DPFfz7dOT9f2IM2utnyXyvVziqfQuH5NyVltrnC4vWCkG76XQDXfA"

ser_endpoint = "securities-api/v4/accounts"
base_name = f"https://api-sandbox.commerzbank.com/{ser_endpoint}"

h= {
        'Authorization': 'Bearer ' + token
    }

bucket_name = 'lodzkiterror'  # Replace with your bucket name
account_id = "130471100000EUR"

with httpx.Client() as rq:
    resp= rq.get(f"{base_name}/",headers=h)
    try:
        acc = resp.json()
        acc_sec_ids: list = acc.get("securitiesAccountIds")
        for i in acc_sec_ids:
            name = i.get("pseudonymizedAccountId")
            ids = i.get("securitiesAccountId")
            portfolio = rq.get(f"{base_name}/{ids}/portfolio")
        pprint(acc)
    except JSONDecodeError as e:
        pprint(resp.status_code)
        pprint(resp)
    
    # resp = callApiSbx(base,endpoint,token=token,ex_client=rq)
    # pprint(resp.request.headers)
    # pprint(resp.request.headers.get("authorization"))
    
