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


def test_get_shipments_integration(_data):
    url = _data[0]["URL"] + 'shipments'
    # params = {'id': 12}
    header = _data[1]

    # Send a GET request to the API
    response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1


def test_get_shipments_by_id_integration(_data):
    url = _data[0]["URL"] + 'shipments/13'
    # params = {'id': 12}
    header = _data[1]

    # Send a GET request to the API
    response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and response_data["id"] == 13


def test_post_shipments_integration(_data):
    url = _data[0]["URL"] + 'shipments'
    header = _data[1]
    body =    {
        "id": 999999,
        "order_id": 141,
        "source_id": 29,
        "order_date": "2002-01-16",
        "request_date": "2002-01-18",
        "shipment_date": "2002-01-20",
        "shipment_type": "I",
        "shipment_status": "Pending",
        "notes": "Reeds kraam wit gevaar jongen kasteel.",
        "carrier_code": "DHL",
        "carrier_description": "DHL Express",
        "service_code": "Fastest",
        "payment_type": "Automatic",
        "transfer_mode": "Sea",
        "total_package_count": 28,
        "total_package_weight": 69.21,
        "created_at": "2002-01-16T15:14:32Z",
        "updated_at": "2002-01-17T17:14:32Z",
        "items": [
            {
                "item_id": "P003670",
                "amount": 5
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


def test_put_shipments_integration(_data):
    url = _data[0]["URL"] + 'shipments/141'
    header = _data[1]
    body =    {
        "id": 141,
        "order_id": 141,
        "source_id": 29,
        "order_date": "2002-01-16",
        "request_date": "2002-01-18",
        "shipment_date": "2002-01-20",
        "shipment_type": "I",
        "shipment_status": "Pending",
        "notes": "Reeds kraam wit gevaar jongen kasteel.",
        "carrier_code": "DHL",
        "carrier_description": "DHL Express",
        "service_code": "Fastest",
        "payment_type": "Automatic",
        "transfer_mode": "Sea",
        "total_package_count": 28,
        "total_package_weight": 69.21,
        "created_at": "2002-01-16T15:14:32Z",
        "updated_at": "2002-01-17T17:14:32Z",
        "items": [
            {
                "item_id": "P003670",
                "amount": 5
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
    assert status_code == 200 and response_data["id"] == body["id"] and response_data["carrier_code"] == body["carrier_code"] and response_data["service_code"] == body["service_code"]


def test_delete_shipments_integration(_data):
    url = _data[0]["URL"] + 'shipments/35'
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

def test_shipments_response_no_key_integration(_data):
    url = _data[0]["URL"] + 'shipments'
    # params = {'id': 12}

    # Send a GET request to the API
    response = requests.get(url)

    # Get the status code and response data
    status_code = response.status_code
    # response_data = response.json()

    # Verify that the status code is 401 (Unauthorized)
    assert status_code == 401

def test_post_shipment_invalid_date_integration(_data):
    url = _data[0]["URL"] + 'shipments'
    header = _data[1]
    body = {
        "id": 1000,
        "order_id": 10,
        "shipment_date": "invalid-date",
        "shipment_status": "Pending"
    }

    # Send a POST request with an invalid date format
    response = requests.post(url, headers=header, json=body)

    # Verify that the status code is 400 (Bad Request)
    assert response.status_code == 400







