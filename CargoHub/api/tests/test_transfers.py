# Test file for all GET methods in the code

# To run tests. run the following command
# $env:PYTHONPATH="<root directory>" ; pytest api/tests/test_inventories.py

import sys
import os
import pytest
import requests

sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))) 


from api.models import orders
from api.providers import data_provider

data_provider.init()



@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:3000/api/v1/'}, {"API_KEY": "a1b2c3d4e5"}]


def test_get_transfers_integration(_data):
    url = _data[0]["URL"] + 'transfers'
    # params = {'id': 12}
    header = _data[1]

    # Send a GET request to the API
    response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1

    # Verify the response data
    # assert response_data['id'] == 123
    # assert response_data['name'] == 'John Smith'

def test_get_transfers_by_id_integration(_data):
    url = _data[0]["URL"] + 'transfers/12'
    # params = {'id': 12}
    header = _data[1]

    # Send a GET request to the API
    response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and response_data["id"] == 12

    # Verify the response data
    # assert response_data['id'] == 123
    # assert response_data['name'] == 'John Smith'

def test_post_transfers_integration(_data):
    url = _data[0]["URL"] + 'transfers'
    # params = {'id': 12}
    header = _data[1]
    body =    {
        "id": 999999,
        "reference": "TR00001",
        "transfer_from": 9000,
        "transfer_to": 9229,
        "transfer_status": "Completed",
        "created_at": "2000-03-11T13:11:14Z",
        "updated_at": "2000-03-12T16:11:14Z",
        "items": [
            {
                "item_id": "P007435",
                "amount": 23
            }
        ]
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, headers=header, json=body)
    assert post_response.status_code == 201

    get_response = requests.get(url + "/999999", headers=header)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()
    # response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 
    dummy = requests.delete(url + "/999999", headers=header)


def test_put_transfers_integration(_data):
    url = _data[0]["URL"] + 'transfers/2'
    header = _data[1]
    body =    {
        "id": 2,
        "reference": "TR00002",
        "transfer_from": 9229,
        "transfer_to": 9284,
        "transfer_status": "Completed",
        "created_at": "2017-09-19T00:33:14Z",
        "updated_at": "2017-09-20T01:33:14Z",
        "items": [
            {
                "item_id": "P007435",
                "amount": 23
            }
        ]
    }

    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, headers=header, json=body)
    assert put_response.status_code == 200

    get_response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()
    # response_data = response.json()

    # Verify that the status code is 200 (OK) and the body in this code and the response data are basically equal
    assert status_code == 200 and response_data["id"] == body["id"] and response_data["reference"] == body["reference"] and response_data["transfer_to"] == body["transfer_to"]


def test_delete_transfers_integration(_data):
    url = _data[0]["URL"] + 'transfers/35'
    header = _data[1]

    get1_response = requests.get(url, headers=header)

    # Send a DELETE request to the API and check if it was successful
    delete_response = requests.delete(url, headers=header)
    assert delete_response.status_code == 200

    get2_response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = get2_response.status_code
    response_data = get2_response.json()

    # Verify that the status code is 200 (OK) and that the client doesn't exist anymore
    assert status_code == 200 and response_data == None
    
    # Repost the deleted inventory for later use
    post_response = requests.post(url, headers=header, json=get1_response.json())

#Edge Cases

def test_transfers_response_no_key_integration(_data):
    url = _data[0]["URL"] + 'transfers'
    # params = {'id': 12}

    # Send a GET request to the API
    response = requests.get(url)

    # Get the status code and response data
    status_code = response.status_code
    # response_data = response.json()

    # Verify that the status code is 401 (Unauthorized)
    assert status_code == 401

def test_post_transfer_nonexistent_item_integration(_data):
    url = _data[0]["URL"] + 'transfers'
    header = _data[1]
    body = {
        "id": 1001,
        "transfer_from": 9000,
        "transfer_to": 9229,
        "items": [{"item_id": "INVALID_ITEM", "amount": 10}]
    }

    # Send a POST request with a non-existent item ID
    response = requests.post(url, headers=header, json=body)

    # Verify that the status code is 404 (Not Found) or 400 (Bad Request)
    assert response.status_code in [404, 400]
