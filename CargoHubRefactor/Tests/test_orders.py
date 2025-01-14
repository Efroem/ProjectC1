import sys
import os
import pytest
import requests

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))) 

@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:5000/api/v1/', 'AdminApiToken': 'A1B2C3D4'}]

# Helper function to get headers with AdminApiToken
def get_headers(admin_api_token):
    return {"ApiToken": admin_api_token}

def test_get_orders_integration(_data):
    url = _data[0]["URL"] + 'orders'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1

def test_get_orders_by_id_integration(_data):
    url = _data[0]["URL"] + 'orders/2'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and response_data["id"] == 2

def test_post_orders_integration(_data):
    url = _data[0]["URL"] + 'orders'
    headers = get_headers(_data[0]["AdminApiToken"])
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

    # Send a POST request to the API
    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 200
    order_id = post_response.json().get("id")
    
    # Get the created order
    get_response = requests.get(f"{url}/{order_id}", headers=headers)

    status_code = get_response.status_code
    response_data = get_response.json()

    # Cleanup: Delete the created order
    requests.delete(f"{url}/{order_id}", headers=headers)

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and response_data["shippingNotes"] == body["shippingNotes"] and response_data["pickingNotes"] == body["pickingNotes"]

def test_put_orders_integration(_data):

    url = _data[0]["URL"] + 'orders/1424'
    headers = get_headers(_data[0]["AdminApiToken"])
    original_order = requests.get(url, headers=headers)
    assert original_order.status_code == 200
    original_json = original_order.json()
    # print(original_body)
    original_body = {
        "sourceId": original_json.get("sourceId"),  # Corrected syntax for .get method
        "orderDate": original_json.get("orderDate", "2024-12-10T10:00:00Z"),
        "requestDate": original_json.get("requestDate", "2024-12-12T10:00:00Z"),
        "reference": original_json.get("reference", "ORD12347"),
        "referenceExtra": original_json.get("referenceExtra", "EXTRA125"),
        "orderStatus": original_json.get("orderStatus", "Pending"),
        "notes": original_json.get("notes", "Handle with care"),
        "shippingNotes": original_json.get("shippingNotes", "No special instructions"),
        "pickingNotes": original_json.get("pickingNotes", "Pick all items"),
        "warehouseId": original_json.get("warehouseId", 4),
        "shipTo": original_json.get("shipTo", 60),
        "billTo": original_json.get("billTo", 100),
        "shipmentId": original_json.get("shipmentId", 91),
        "totalAmount": original_json.get("totalAmount", 2500.00),
        "totalDiscount": original_json.get("totalDiscount", 200.00),
        "totalTax": original_json.get("totalTax", 150.00),
        "totalSurcharge": original_json.get("totalSurcharge", 30.00),
        "orderItems": original_json.get("orderItems", []),
    }

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
        },
        {
        "itemId": "P000002",
        "amount": 3
        }
    ],
    "createdAt": "2024-01-01T10:00:00Z",
    "updatedAt": "2024-12-01T10:00:00Z"
    }



    put_response = requests.put(url, json=body, headers=headers)

    assert put_response.status_code == 200

    # Get the updated order
    get_response = requests.get(url, headers=headers)

    status_code = get_response.status_code
    response_data = get_response.json()


    # Restore original body
    original_put_response = requests.put(url, json=original_body, headers=headers)
    print(original_put_response.text)
    assert original_put_response.status_code == 200

    # Verify that the status code is 200 (OK) and the body in this code and the response data are basically equal
    assert status_code == 200 and response_data["id"] == original_json["id"] and response_data["notes"] == body["notes"]


def test_delete_orders_integration(_data):
    url = _data[0]["URL"] + 'orders'
    headers = get_headers(_data[0]["AdminApiToken"])

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

    # Send a POST request to create a new order
    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 200
    order_id = post_response.json().get("id")

    # Verify the order exists
    get1_response = requests.get(f"{url}/{order_id}", headers=headers)

    if get1_response.status_code == 200:
        # Send a DELETE request to remove the order
        delete_response = requests.delete(f"{url}/{order_id}", headers=headers)

        # Verify that the DELETE request was successful
        assert delete_response.status_code == 200
    else:
        print("Resource with ID not found.")

def test_post_order_missing_fields_integration(_data):
    url = _data[0]["URL"] + 'orders'
    headers = get_headers(_data[0]["AdminApiToken"])

    body = {
        # Missing required fields like order_date, shipment_id, etc.
        "source_id": 29,
        "total_amount": 123.45
    }

    # Send a POST request with missing fields
    response = requests.post(url, json=body, headers=headers)

    # Verify that the status code is 400 (Bad Request)
    assert response.status_code == 400
