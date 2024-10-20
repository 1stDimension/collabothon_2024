from json import JSONDecodeError
import os
from typing import Any
from fastapi import BackgroundTasks, FastAPI
from fastapi.responses import RedirectResponse
from httpx import Response
from api_scrapper import *
from pprint import pprint

client, secret = os.environ["CLIENT"], os.environ["SECRET"]
cred = ClientCredentials(client, secret)

base = "/securities-api/v4"
endpoint = "/accounts"
r_url = "localhost:8000/"

CODE: str|None = None

def OauthURL(client_id: str, redirect_url: str):
    query = {
        "response_type": "code",
        "client_id": client_id,
        # "redirect_uri": redirect_url
    }
    url = "https://api-sandbox.commerzbank.com/auth/realms/sandbox/protocol/openid-connect/auth"
    resp = httpx.request("GET", url, params=query)
    return resp.url
    print(resp)
    pprint(resp.text)
    if resp.status_code >= 400:
        pprint(resp.text)
    # print(resp.text)
    print(resp.request.url)
    j = resp.json()
    return j


def get_token(
    auth_code: str, client_id: str, redirect_url: str, secret: str
) -> dict[str, Any]:
    url = "https://api-sandbox.commerzbank.com/auth/realms/sandbox/protocol/openid-connect/token"
    h = {"Content-Type": "application/x-www-form-urlencoded"}
    q = {
        "grant_type": "authorization_code",
        "client_id": client_id,
        "client_secret": secret,
        "code": auth_code,
        "redirect_uri": redirect_url,  # Doesn't work if you have multiple ones defined
    }
    r = httpx.post(url, headers=h, data=q)
    j = r.json()
    print(j)
    return j


app = FastAPI()


@app.get("/auth")
def authorize():
    Oauth_url = OauthURL(client, r_url)
    print(f"Oauth url: {Oauth_url}")
    return RedirectResponse(Oauth_url)


@app.get("/scrape")
def scrape():
    pass


bucket_name = "lodzkiterror"  # Replace with your bucket name
account_id = "130471100000EUR"


def is_ok(r: Response):
    if r.status_code >= 400:
        print(f"An error occurred {r.status_code}")
        print(r.request.url)
        pprint(r.text)

        print(r.json())
        return False
    elif r.status_code >= 200:
        return True

def ping_service(code,base_name:str,auth_params: dict[str,str]):
        resp = httpx.get(f"{base_name}", headers=auth_params)
        print(f"Service is {resp.status_code}: {resp.text}")


@app.get("/{path:path}")
def read_root(path: str, code: str,bt: BackgroundTasks):
    pprint(path)
    pprint(code)
    CODE = code
    service ="securities-api/v4"
    ser_endpoint = f"{service}/accounts"
    base = f"https://api-sandbox.commerzbank.com"
    base_name = f"{base}/{ser_endpoint}"
    auth_params = get_token(code, client, r_url, secret)
    bt.add_task(ping_service,code,f"{base}/{service}//healthcheck",auth_params)
    if token := auth_params.get("access_token"):
        h = {"Authorization": "Bearer " + token}
        with httpx.Client() as rq:
            resp = rq.get(f"{base_name}", headers=h)
            bt.add_task(ping_service,code,f"{base}/{service}//healthcheck",h)
            if is_ok(resp):
                try:
                    acc = resp.json()
                    acc_sec_ids: list[dict[str, Any]] = acc.get(
                        "securitiesAccountIds", []
                    )
                    for i in acc_sec_ids:
                        name = i.get("pseudonymizedAccountId")
                        idx = i.get("securitiesAccountId")
                        portfolio = rq.get(f"{base_name}/{name}/portfolio", headers=h)
                        if is_ok(portfolio):
                            pprint(portfolio.json())
                        else:
                            continue
                        try:
                            p = portfolio.json()

                            upload_file_to_bucket(
                                bucket_name, f"{name}-securities.json", portfolio.text
                            )
                            pprint(p)
                        except JSONDecodeError as e:
                            pprint(resp.status_code)
                            pprint(resp)
                    pprint(acc)
                except JSONDecodeError as e:
                    # pprint(resp.status_code)
                    pprint(resp)
    else:
        print("No access token")
    return {"Hello": "World"}
