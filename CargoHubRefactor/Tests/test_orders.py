
import sys
import os
import pytest
import requests

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))) 

@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:5000/api/v1/'}]

def test_get_orders_integration(_data):
    url = _data[0]["URL"] + 'orders'
    # params = {'id': 12}


    # Send a GET request to the API
    response = requests.get(url)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1

def test_get_orders_by_id_integration(_data):
    url = _data[0]["URL"] + 'orders/2'
    # params = {'id': 12}

    # Send a GET request to the API
    response = requests.get(url)

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
    # header = _data[1]
    body = {
        "sourceId": 1255,
        "orderDate": "2024-12-10T10:00:00Z",
        "requestDate": "2024-12-12T10:00:00Z",
        "reference": "ORD12347",
        "referenceExtra": "EXTRA125",
        "orderStatus": "Pending",
        "notes": "Handle with care",
        "shippingNotes": "No special instructions",
        "pickingNotes": "Pick all items",
        "warehouseId": 4,
        "shipTo": 60,
        "billTo": 100,
        "shipmentId": 91,
        "totalAmount": 2500.00,
        "totalDiscount": 200.00,
        "totalTax": 150.00,
        "totalSurcharge": 30.00,
        "orderItems": [
            {
                "itemId": "P000001",
                "amount": 5
            }
        ]
    }

    post_response = requests.post(url, json=body)
    assert post_response.status_code == 200
    id = post_response.json().get("id")
    
    get_response = requests.get(f"{url}/{id}")

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()
    # response_data = response.json()

    # Verify that the status code is 200 (OK)
    # print(uid)
    # print(response_data)
    dummy = requests.delete(f"{url}/{id}")
    assert status_code == 200 and response_data["shippingNotes"] == body["shippingNotes"] and response_data["pickingNotes"] == body["pickingNotes"]


def test_put_orders_integration(_data):
    url = _data[0]["URL"] + 'orders/3054'

    original_order = requests.get(url)
    assert original_order.status_code == 200
    original_body = original_order.json()
    print(original_body)

    # header = _data[1]
    body = {
        "sourceId": 1255,
        "orderDate": "2024-12-10T10:00:00Z",
        "requestDate": "2024-12-12T10:00:00Z",
        "reference": "ORD12347",
        "referenceExtra": "EXTRA125",
        "orderStatus": "Pending",
        "notes": "Handle with care",
        "shippingNotes": "No special instructions",
        "pickingNotes": "Pick all items",
        "warehouseId": 4,
        "shipTo": 60,
        "billTo": 100,
        "shipmentId": 91,
        "totalAmount": 2500.00,
        "totalDiscount": 200.00,
        "totalTax": 150.00,
        "totalSurcharge": 30.00,
        "orderItems": [
            {
                "itemId": "P000001",
                "amount": 5
            }
        ]
    }

    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, json=body)
    assert put_response.status_code == 200

    get_response = requests.get(url)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()

    # Restore original body
    original_put_response = requests.put(url, json=original_body)
    assert original_put_response.status_code == 200

    # Verify that the status code is 200 (OK) and the body in this code and the response data are basically equal
    assert status_code == 200 and response_data["id"] == original_body["id"] and response_data["notes"] == body["notes"]

def test_delete_orders_integration(_data):

    url = _data[0]["URL"] + 'orders'
    body = {
            "sourceId": 111,
            "orderDate": "2024-12-10T10:00:00Z",
            "requestDate": "2024-12-12T10:00:00Z",
            "reference": "ORD12347",
            "referenceExtra": "EXTRA1eaheaare25",
            "orderStatus": "Pending",
            "notes": "Handle witSGsgh care",
            "shippingNotes": "No speciSGsgdSDgal instructions",
            "pickingNotes": "Pick adgSGSDgSEll items",
            "warehouseId": 4,
            "shipTo": 60,
            "billTo": 100,
            "shipmentId": 91,
            "totalAmount": 2500.00,
            "totalDiscount": 200.00,
            "totalTax": 150.00,
            "totalSurcharge": 30.00,
            "orderItems": [
                {
                    "itemId": "P000001",
                    "amount": 5
                }
            ]
        }
    post_response = requests.post(url, json=body)
    assert post_response.status_code == 200
    order_id = post_response.json().get("Id")

    get1_response = requests.get(f"{url}/{order_id}")

    

    # Als recourse bestaat, stuurt hij delete request
    if get1_response.status_code == 200:

        delete_response = requests.delete(f"{url}/{order_id}") 

        # Check if the DELETE request was successful
        assert delete_response.status_code == 200
    else:
        print("Resource with ID 3 not found.")



#Edge cases

# def test_orders_response_no_key_integration(_data):
#     url = _data[0]["URL"] + 'orders'
#     # params = {'id': 12}

#     # Send a GET request to the API
#     response = requests.get(url)

#     # Get the status code and response data
#     status_code = response.status_code
#     # response_data = response.json()

#     # Verify that the status code is 401 (Unauthorized)
#     assert status_code == 401

def test_post_order_missing_fields_integration(_data):
    url = _data[0]["URL"] + 'orders'
    # header = _data[1]
    body = {
        # Missing required fields like order_date, shipment_id, etc.
        "source_id": 29,
        "total_amount": 123.45
    }

    # Send a POST request with missing fields
    response = requests.post(url, json=body)

    # Verify that the status code is 400 (Bad Request)
    assert response.status_code == 400
