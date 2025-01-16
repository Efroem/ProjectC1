import os
import sys
import pytest
import requests

# Add paths for module imports
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

@pytest.fixture
def _data():
    return {
        "URL": "http://localhost:5000/api/v1/",
        "AdminApiToken": "A1B2C3D4",
        "EmployeeApiToken": "H8I9J10",
        "FloorManagerApiToken": "F12345",
        "WarehouseManagerApiToken": "W12345",
    }

def get_headers(token):
    return {
        "ApiToken": token
    }

import requests

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
        "contactEmail": "vdkvms@gmail.com"
    }

    # Perform GET request to retrieve existing clients
    response = requests.get(url, headers=headers)
    assert response.status_code == 200, f"GET failed with status {response.status_code}"

    # Perform POST request to create a new client
    post_response = requests.post(url, headers=headers, json=body)
    assert post_response.status_code == 200, f"POST failed with status {post_response.status_code}"



def test_employee_token_get_only(_data):
    url = _data["URL"] + "Clients"
    headers = get_headers(_data["EmployeeApiToken"])

    body = {
    "name": "Vincent test",
    "address": "123 Main St",
    "city": "Anytown",
    "zipCode": "12346",
    "province": "Zeeland",
    "country": "Netherlands",
    "contactName": "Joe",
    "contactPhone": "+123456798",
    "contactEmail": "altijd@gmail.com"
}

    response = requests.get(url, headers=headers)
    assert response.status_code == 200, f"GET failed with status {response.status_code}"

    response = requests.post(url, headers=headers, json=body)
    assert response.status_code == 403, f"Expected 403 for PUT on disallowed path, got {response.status_code}"


def test_floor_manager_token_put_limited_paths(_data):
    url_allowed = _data["URL"] + "Orders"
    url_not_allowed = _data["URL"] + "Clients"
    headers = get_headers(_data["FloorManagerApiToken"])

    body = {
  "SourceId": 125,
  "OrderDate": "2024-12-10T10:00:00Z",
  "RequestDate": "2024-12-12T10:00:00Z",
  "Reference": "ORD12347",
  "ReferenceExtra": "EXTRA125",
  "OrderStatus": "Pending",
  "Notes": "Handle with care",
  "ShippingNotes": "No special instructions",
  "PickingNotes": "Pick all items",
  "WarehouseId": 4,
  "ShipTo": 60,
  "BillTo": 100,
  "ShipmentId": 91,
  "TotalAmount": 2500.00,
  "TotalDiscount": 200.00,
  "TotalTax": 150.00,
  "TotalSurcharge": 30.00
}

    # Floor Manager can PUT on allowed paths
    response = requests.put(url_allowed, headers=headers, json=body)
    assert response.status_code == 200, f"PUT failed with status {response.status_code} on allowed path"

    # Floor Manager cannot PUT on disallowed paths
    response = requests.put(url_not_allowed, headers=headers, json={"example": "data"})
    assert response.status_code == 403, f"Expected 403 for PUT on disallowed path, got {response.status_code}"

### Tests for Warehouse Manager Token
def test_warehouse_manager_token_put_limited_paths(_data):
    url_allowed = _data["URL"] + "Warehouses"
    url_not_allowed = _data["URL"] + "Clients"
    headers = get_headers(_data["WarehouseManagerApiToken"])

    # Warehouse Manager can PUT on allowed paths
    response = requests.put(url_allowed, headers=headers, json={"example": "data"})
    assert response.status_code == 200, f"PUT failed with status {response.status_code} on allowed path"

    # Warehouse Manager cannot PUT on disallowed paths
    response = requests.put(url_not_allowed, headers=headers, json={"example": "data"})
    assert response.status_code == 403, f"Expected 403 for PUT on disallowed path, got {response.status_code}"

### Tests for Invalid Tokens
def test_invalid_token(_data):
    url = _data["URL"] + "Orders"
    headers = get_headers("InvalidToken")

    body = {
  "SourceId": 125,
  "OrderDate": "2024-12-10T10:00:00Z",
  "RequestDate": "2024-12-12T10:00:00Z",
  "Reference": "ORD12347",
  "ReferenceExtra": "EXTRA125",
  "OrderStatus": "Pending",
  "Notes": "Handle with care",
  "ShippingNotes": "No special instructions",
  "PickingNotes": "Pick all items",
  "WarehouseId": 4,
  "ShipTo": 60,
  "BillTo": 100,
  "ShipmentId": 91,
  "TotalAmount": 2500.00,
  "TotalDiscount": 200.00,
  "TotalTax": 150.00,
  "TotalSurcharge": 30.00
}

    # Invalid tokens should be forbidden
    response = requests.get(url, headers=headers)
    assert response.status_code == 403, f"Expected 403 for GET with invalid token, got {response.status_code}"

    response = requests.post(url, headers=headers, json=body)
    assert response.status_code == 403, f"Expected 403 for POST with invalid token, got {response.status_code}"

### Tests for Missing Token
def test_missing_token(_data):
    url = _data["URL"] + "Orders"

    body = {
  "SourceId": 125,
  "OrderDate": "2024-12-10T10:00:00Z",
  "RequestDate": "2024-12-12T10:00:00Z",
  "Reference": "ORD12347",
  "ReferenceExtra": "EXTRA125",
  "OrderStatus": "Pending",
  "Notes": "Handle with care",
  "ShippingNotes": "No special instructions",
  "PickingNotes": "Pick all items",
  "WarehouseId": 4,
  "ShipTo": 60,
  "BillTo": 100,
  "ShipmentId": 91,
  "TotalAmount": 2500.00,
  "TotalDiscount": 200.00,
  "TotalTax": 150.00,
  "TotalSurcharge": 30.00
}

    # Requests without tokens should be unauthorized
    response = requests.get(url)
    assert response.status_code == 401, f"Expected 401 for GET without token, got {response.status_code}"

    response = requests.post(url, json=body)
    assert response.status_code == 401, f"Expected 401 for POST without token, got {response.status_code}"
