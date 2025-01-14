import pytest
import requests
import sys
import os
import json
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

# Fixture to provide URL and AdminApiToken
@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:5000/api/v1/', 'AdminApiToken': 'A1B2C3D4'}]

# Helper function to get headers with AdminApiToken
def get_headers(admin_api_token):
    return {"ApiToken": admin_api_token}

def test_get_warehouses_integration(_data):
    url = _data[0]["URL"] + 'Warehouses'
    admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
    headers = get_headers(admin_api_token)
    
    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1

def test_get_warehouse_by_id_integration(_data):
    url = _data[0]["URL"] + 'Warehouses/1'
    admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
    headers = get_headers(admin_api_token)
    
    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK) and the warehouseId matches
    assert status_code == 200 and response_data["warehouseId"] == 1

def test_post_warehouses_integration(_data):
    url = _data[0]["URL"] + 'Warehouses'
    admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
    headers = get_headers(admin_api_token)
    
    body = {
        "code": "WH0010JNvKJSDVJJVNSKJV",
        "name": "Test WarehousevLNSVNSV",
        "address": "123 Test LanevSVNS",
        "zip": "1234349854289672",
        "city": "TestCityieargeiajgoeag",
        "province": "TestProviceegaegeogapeo",
        "country": "TestCountryelgkleagl",
        "contactName": "John Doeambmfmb",
        "contactPhone": "555-1234lbla",
        "contactEmail": "testmail@example.com;mBLDMB:"
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 200
    warehouse_id = post_response.json().get("warehouseId")
    
    get_response = requests.get(f"{url}/{warehouse_id}", headers=headers)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()

    # Verify the warehouse was created and response data matches
    assert status_code == 200 and response_data["name"] == body["name"] and response_data["address"] == body["address"]

    # Clean up by deleting the created warehouse
    delete_response = requests.delete(f"{url}/{warehouse_id}", headers=headers)
    assert delete_response.status_code == 200

def test_put_warehouses_integration(_data):
    url = _data[0]["URL"] + 'Warehouses/1'
    admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
    headers = get_headers(admin_api_token)
    
    body = {
        "code": "WH0010",
        "name": "Test Warehouse",
        "address": "123 Test Lane",
        "zip": "12345",
        "city": "TestCity",
        "province": "TestProvice",
        "country": "TestCountry",
        "contactName": "John Doe",
        "contactPhone": "555-1234",
        "contactEmail": "testmail@example.com"
    }

    # Get the original warehouse data before PUT
    dummy_get = requests.get(url, headers=headers)
    dummyJson = dummy_get.json()

    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, json=body, headers=headers)
    assert put_response.status_code == 200
    warehouse_id = put_response.json().get("warehouseId")
    
    get_response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()

    # Restore the original warehouse data
    requests.put(url, json=dummyJson, headers=headers)
    
    # Verify that the PUT request succeeded
    assert status_code == 200 and response_data["warehouseId"] == warehouse_id and response_data["name"] == body["name"] and response_data["address"] == body["address"]

def test_delete_warehouses_integration(_data):
    # Make a POST request first to create a dummy warehouse
    url = _data[0]["URL"] + 'Warehouses'
    admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
    headers = get_headers(admin_api_token)
    
    body = {
        "code": "WH9999",
        "name": "dummy",
        "address": "dummy",
        "zip": "12345",
        "city": "dummy",
        "province": "dummy",
        "country": "dummy",
        "contactName": "John Doe",
        "contactPhone": "555-1234",
        "contactEmail": "testmail@example.com"
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 200
    warehouse_id = post_response.json().get("warehouseId")
    
    url += f"/{warehouse_id}"

    # Send a DELETE request to the API and check if it was successful
    delete_response = requests.delete(url, headers=headers)
    assert delete_response.status_code == 200

    get2_response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = get2_response.status_code
    response_data = None 
    try:
        response_data = get2_response.json()
    except:
        pass

    # Verify that the warehouse does not exist anymore (404 response)
    assert status_code == 404 and response_data == None
