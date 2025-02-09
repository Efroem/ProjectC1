import os
import sys
import pytest
import requests
from dotenv import load_dotenv

load_dotenv()

# Add paths for module imports
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

@pytest.fixture
def _data():
    return {
        "URL": "http://localhost:5000/api/v1/",
        "AdminApiToken": os.getenv('AdminApiToken'),
        "EmployeeApiToken": os.getenv('EmployeeApiToken'),
        "FloorManagerApiToken": os.getenv('FloorManagerApiToken'),
        "WarehouseManagerApiToken": os.getenv('WarehouseManagerToken'),
        "InvalidToken": "asdasasd",

    }

def get_headers(token):
    return {
        "ApiToken": token
    }


def test_admin_token_get_access(_data):
    url = _data["URL"] + "Clients"
    headers = get_headers(_data["AdminApiToken"])

    body = {
        "name": "Vincent test",
        "address": "123 Main St",
        "city": "Anytown",
        "zipCode": "12346",
        "province": "Zeeland",
        "country": "Netherlands",
        "contactName": "Joe",
        "contactPhone": "+123456798",
        "contactEmail": "opodw@gmail.com"
    }

    # Perform GET request to retrieve existing clients
    response = requests.get(url, headers=headers)
    assert response.status_code == 200, f"GET failed with status {response.status_code}"

    # Perform POST request to create a new client
    post_response = requests.post(url, headers=headers, json=body)
    assert post_response.status_code == 200, f"POST failed with status {post_response.status_code}"

    # Retrieve the newly created client ID
    client_id = post_response.json().get("clientId")
    assert client_id, "Client ID not returned in POST response"

    # Verify the client exists with a GET request
    get_response = requests.get(f"{url}/{client_id}", headers=headers)
    assert get_response.status_code == 200, f"Client GET failed with status {get_response.status_code}"

    # Perform DELETE request to remove the client
    delete_response = requests.delete(f"{url}/{client_id}", headers=headers)
    assert delete_response.status_code == 200, f"DELETE failed with status {delete_response.status_code}"

    # Confirm the client has been deleted by attempting to GET it again
    confirm_response = requests.get(f"{url}/{client_id}", headers=headers)
    assert confirm_response.status_code == 404, "Client was not deleted successfully"


def test_floor_manager_token_put_limited_paths(_data):
    # Define URLs for both allowed and disallowed paths
    url_allowed = _data["URL"] + "Orders/1"
    url_not_allowed = _data["URL"] + "Clients/1"
    
    # Headers with Floor Manager API token
    headers = get_headers(_data["FloorManagerApiToken"])

    # Define request body for the allowed PUT request
    body = {
    "SourceId": 123,
    "OrderDate": "2025-01-07T00:00:00",
    "RequestDate": "2025-01-07T00:00:00",
    "Reference": "Order123",
    "ReferenceExtra": "Some extra reference",
    "OrderStatus": "Delivered",
    "Notes": "Some notes",
    "ShippingNotes": "Shipping instructions",
    "PickingNotes": "Picking instructions",
    "WarehouseId": 5,
    "ShipTo": 10,
    "BillTo": 20,
    "ShipmentId": 30,
    "TotalAmount": 100.0,
    "TotalDiscount": 10.0,
    "TotalTax": 5.0,
    "TotalSurcharge": 2.0,
    "OrderItems": []
    }

    # Define request body for the disallowed PUT request
    body2 = {
        "name": "Jane Doe",
        "address": "456 Secondary St",
        "city": "New City",
        "zipCode": "54321",
        "province": "Berlin",
        "country": "Germany",
        "contactName": "Jane Doe",
        "contactPhone": "+987654321",
        "contactEmail": "jane.doe@example.com"
    }

    # Floor Manager can PUT on allowed paths (Orders)
    response = requests.put(url_allowed, headers=headers, json=body)
    assert response.status_code == 200, f"PUT failed with status {response.status_code} on allowed path"

    # Floor Manager cannot PUT on disallowed paths (Clients)
    response = requests.put(url_not_allowed, headers=headers, json=body2)
    assert response.status_code == 403, f"Expected 403 for PUT on disallowed path, got {response.status_code}"


### Tests for Warehouse Manager Token
def test_warehouse_manager_token_put_limited_paths(_data):
    url_allowed = _data["URL"] + "Warehouses/1"
    url_not_allowed = _data["URL"] + "Clients/9830"
    headers = get_headers(_data["WarehouseManagerApiToken"])

    body = {
  "code": "WH123",
  "name": "Updated Warehouse",
  "address": "123 Updated St",
  "zip": "54321",
  "city": "Updated City",
  "province": "UC",
  "country": "USA",
  "contactName": "Updated Contact",
  "contactPhone": "321-654-9870",
  "contactEmail": "updated@example.com",
  "restrictedClassificationsList": ["4.1", "5.2"]
}
    body2 = {
    "name": "Jane Doe",
    "address": "456 Secondary St",
    "city": "New City",
    "zipCode": "54321",
    "province": "Berlin",
    "country": "Germany",
    "contactName": "Jane Doe",
    "contactPhone": "+987654321",
    "contactEmail": "jane.doe@example.com"
    }

    

    # Warehouse Manager can PUT on allowed paths
    response = requests.put(url_allowed, headers=headers, json=body)
    print(response.text)
    assert response.status_code == 200, f"PUT failed with status {response.status_code} on allowed path"

    # Warehouse Manager cannot PUT on disallowed paths
    response = requests.put(url_not_allowed, headers=headers, json=body2)
    assert response.status_code == 403, f"Expected 403 for PUT on disallowed path, got {response.status_code}"

### Tests for Invalid Tokens
def test_invalid_token(_data):
    url = _data["URL"] + "Clients"
    headers = get_headers("InvalidToken")

    body = {
        "name": "Jane Doe",
        "address": "456 Secondary St",
        "city": "New City",
        "zipCode": "54321",
        "province": "Berlin",
        "country": "Germany",
        "contactName": "Jane Doe",
        "contactPhone": "+987654321",
        "contactEmail": "jane.doe@example.com"
    }

    # Invalid tokens should be forbidden
    response = requests.get(url, headers=headers)
    assert response.status_code == 403, f"Expected 403 for GET with invalid token, got {response.status_code}"

    response = requests.post(url, headers=headers, json=body)
    assert response.status_code == 403, f"Expected 403 for POST with invalid token, got {response.status_code}"

### Tests for Missing Token
def test_missing_token(_data):
    url = _data["URL"] + "Clients"

    body = {
        "name": "Jane Doe",
        "address": "456 Secondary St",
        "city": "New City",
        "zipCode": "54321",
        "province": "Berlin",
        "country": "Germany",
        "contactName": "Jane Doe",
        "contactPhone": "+987654321",
        "contactEmail": "jane.doe@example.com"
    }

    # Requests without tokens should be unauthorized
    response = requests.get(url)
    assert response.status_code == 401, f"Expected 401 for GET without token, got {response.status_code}"

    response = requests.post(url, json=body)
    assert response.status_code == 401, f"Expected 401 for POST without token, got {response.status_code}"
