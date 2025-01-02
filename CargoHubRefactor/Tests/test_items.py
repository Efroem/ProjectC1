# Test file for all GET methods in the code


import pytest
import requests
import sys
import os
import json
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))) 


@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:5000/api/v1/'}]


def test_get_items_integration(_data):
    url = _data[0]["URL"] + 'Items'
    # params = {'id': 12}

    # Send a GET request to the API
    response = requests.get(url)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()
    # response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1


def test_get_items_by_id_integration(_data):
    url = _data[0]["URL"] + 'Items/P000001'
    # params = {'id': 12}

    # Send a GET request to the API
    response = requests.get(url)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    # print(response_data)
    assert status_code == 200 and response_data["uid"] == "P000001"


def test_post_items_integration(_data):
    url = _data[0]["URL"] + 'Items'
    # params = {'id': 12}
    body = {
        "code": "kHVBsdvbuvhdhVBsv",
        "description": "Face-to-face clear-thinking complexity",
        "shortDescription": "must",
        "upcCode": "vkonsdjVSPFIBNDZIBNZOBN",
        "modelNumber": "4958439249674269",
        "commodityCode": "087264968246904",
        "itemLine": 1,
        "itemGroup": 1,
        "itemType": 1,
        "unitPurchaseQuantity": 47,
        "unitOrderQuantity": 13,
        "packOrderQuantity": 11,
        "supplierId": 1,
        "supplierCode": "SUP423",
        "supplierPartNumber": "E-86805-uTM"
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, json=body)
    assert post_response.status_code == 200
    uid = post_response.json().get("uid")
    
    get_response = requests.get(f"{url}/{uid}")

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()
    # response_data = response.json()

    # Verify that the status code is 200 (OK)
    # print(uid)
    # print(response_data)
    dummy = requests.delete(f"{url}/{uid}")
    assert status_code == 200 and response_data["code"] == body["code"] and response_data["description"] == body["description"]


def test_put_items_integration(_data):
    url = _data[0]["URL"] + 'Items/P000001'
    # params = {'id': 12}
    body = {
        "code": "oijwror0wh09b0gwb0gj0suge",
        "description": "Face-to-face clear-thinking complexity",
        "shortDescription": "must",
        "upcCode": "467486868484",
        "modelNumber": "-et9tetiet9gbunet9ge7476",
        "commodityCode": "wb9ub0wur8un0wun2",
        "itemLine": 1,
        "itemGroup": 1,
        "itemType": 1,
        "unitPurchaseQuantity": 47,
        "unitOrderQuantity": 13,
        "packOrderQuantity": 11,
        "supplierId": 1,
        "supplierCode": "SUP423",
        "supplierPartNumber": "E-86805-uTM"
    }
    dummy_get = requests.get(url)
    assert dummy_get.status_code == 200
    dummyJson = dummy_get.json()
    print(dummyJson)
    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, json=body)
    assert put_response.status_code == 200
    uid = put_response.json().get("uid")

    # Get the status code and response data
    get_response = requests.get(url)
    status_code = get_response.status_code
    response_data = get_response.json()
    # response_data = response.json()
    dummy = requests.put(url, json=dummyJson)


    # Verify that the status code is 200 (OK) and the body in this code and the response data are basically equal
    assert status_code == 200 and response_data["uid"] == uid and response_data["code"] == body["code"] and response_data["description"] == body["description"]


def test_delete_items_integration(_data):
    # Make a POST reqeust first to make a dummy warehouse
    url = _data[0]["URL"] + 'Items'
    # params = {'id': 12}
    body = {
        "code": "xbox200000",
        "description": "Dummy",
        "shortDescription": "must",
        "upcCode": "999999999999999",
        "modelNumber": "12_QRSTUV",
        "commodityCode": "oTo300",
        "itemLine": 1,
        "itemGroup": 1,
        "itemType": 1,
        "unitPurchaseQuantity": 45,
        "unitOrderQuantity": 13,
        "packOrderQuantity": 11,
        "supplierId": 1,
        "supplierCode": "SUP423",
        "supplierPartNumber": "E-86805-uTM"
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, json=body)
    assert post_response.status_code == 200
    uid = post_response.json().get("uid")
    
    url += f"/{uid}"

    # Send a DELETE request to the API and check if it was successful
    delete_response = requests.delete(url)
    assert delete_response.status_code == 200

    get2_response = requests.get(url)


    # Get the status code and response data
    status_code = get2_response.status_code
    response_data = None 
    try:
        response_data = get2_response.json()
    except:
        pass

    # Verify that the status code is 200 (OK) and that the warehouse doesn't exist anymore
    assert status_code == 404 and response_data == None









