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

def test_get_orders_integration(_data):
    url = _data[0]["URL"] + 'orders'
    # params = {'id': 12}
    header = _data[1]

    # Send a GET request to the API
    response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1

def test_get_orders_by_id_integration(_data):
    url = _data[0]["URL"] + 'orders/2'
    # params = {'id': 12}
    header = _data[1]

    # Send a GET request to the API
    response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and response_data["id"] == 2

    # Verify the response data
    # assert response_data['id'] == 123
    # assert response_data['name'] == 'John Smith'

def test_post_orders_integration(_data):
    url = _data[0]["URL"] + 'orders'
    # params = {'id': 12}
    header = _data[1]
    body =    {
        "id": 999999, 
        "source_id": 29, 
        "order_date": "1990-11-22T19:50:42Z", 
        "request_date": "1990-11-26T19:50:42Z", 
        "reference": "ORD03054", 
        "reference_extra": "Team zwak deel hier.", 
        "order_status": "Delivered", 
        "notes": "Graf vis vork hobby herinneren.", 
        "shipping_notes": "Zinken broer persoon zwembad.", 
        "picking_notes": "Brood knippen ja glad.", 
        "warehouse_id": 21, 
        "ship_to": 4233, 
        "bill_to": 4233, 
        "shipment_id": 8365, 
        "total_amount": 6613.42, 
        "total_discount": 390.08, 
        "total_tax": 305.0, 
        "total_surcharge": 41.9, 
        "created_at": "1990-11-22T19:50:42Z", 
        "updated_at": "1990-11-24T15:50:42Z", 
        "items": [{"item_id": "P000032", "amount": 45}, {"item_id": "P011181", "amount": 39}, {"item_id": "P005104", "amount": 36}, {"item_id": "P000593", "amount": 2}, {"item_id": "P008136", "amount": 6}, {"item_id": "P011188", "amount": 29}, {"item_id": "P004274", "amount": 10}, {"item_id": "P000846", "amount": 8}, {"item_id": "P001188", "amount": 38}, {"item_id": "P008870", "amount": 24}, {"item_id": "P006794", "amount": 46}, {"item_id": "P011196", "amount": 19}, {"item_id": "P003299", "amount": 28}, {"item_id": "P008927", "amount": 26}, {"item_id": "P001048", "amount": 43}, {"item_id": "P010873", "amount": 8}, {"item_id": "P007494", "amount": 15}, {"item_id": "P004533", "amount": 14}]
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


def test_put_orders_integration(_data):
    url = _data[0]["URL"] + 'orders/3054'
    header = _data[1]
    body =    {
        "id": 3054, 
        "source_id": 29, 
        "order_date": "1990-11-22T19:50:42Z", 
        "request_date": "1990-11-26T19:50:42Z", 
        "reference": "ORD03054", 
        "reference_extra": "Team zwak deel hier.", 
        "order_status": "Delivered", 
        "notes": "Graf vis vork hobby herinneren.", 
        "shipping_notes": "Zinken broer persoon zwembad.", 
        "picking_notes": "Brood knippen ja glad.", 
        "warehouse_id": 21, 
        "ship_to": 4233, 
        "bill_to": 4233, 
        "shipment_id": 8365, 
        "total_amount": 6613.42, 
        "total_discount": 390.08, 
        "total_tax": 305.0, 
        "total_surcharge": 41.9, 
        "created_at": "1990-11-22T19:50:42Z", 
        "updated_at": "1990-11-24T15:50:42Z", 
        "items": [{"item_id": "P000032", "amount": 45}, {"item_id": "P011181", "amount": 39}, {"item_id": "P005104", "amount": 36}, {"item_id": "P000593", "amount": 2}, {"item_id": "P008136", "amount": 6}, {"item_id": "P011188", "amount": 29}, {"item_id": "P004274", "amount": 10}, {"item_id": "P000846", "amount": 8}, {"item_id": "P001188", "amount": 38}, {"item_id": "P008870", "amount": 24}, {"item_id": "P006794", "amount": 46}, {"item_id": "P011196", "amount": 19}, {"item_id": "P003299", "amount": 28}, {"item_id": "P008927", "amount": 26}, {"item_id": "P001048", "amount": 43}, {"item_id": "P010873", "amount": 8}, {"item_id": "P007494", "amount": 15}, {"item_id": "P004533", "amount": 14}]
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
    assert status_code == 200 and response_data["id"] == body["id"] and response_data["notes"] == body["notes"] and response_data["total_tax"] == body["total_tax"]


def test_delete_orders_integration(_data):
    url = _data[0]["URL"] + 'orders/35'
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

#Edge cases

def test_orders_response_no_key_integration(_data):
    url = _data[0]["URL"] + 'orders'
    # params = {'id': 12}

    # Send a GET request to the API
    response = requests.get(url)

    # Get the status code and response data
    status_code = response.status_code
    # response_data = response.json()

    # Verify that the status code is 401 (Unauthorized)
    assert status_code == 401

def test_post_order_missing_fields_integration(_data):
    url = _data[0]["URL"] + 'orders'
    header = _data[1]
    body = {
        # Missing required fields like order_date, shipment_id, etc.
        "source_id": 29,
        "total_amount": 123.45
    }

    # Send a POST request with missing fields
    response = requests.post(url, headers=header, json=body)

    # Verify that the status code is 400 (Bad Request)
    assert response.status_code == 400
