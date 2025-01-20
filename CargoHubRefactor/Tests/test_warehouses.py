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
    return [{'URL': 'http://localhost:5000/api/v1/', 'AdminApiToken': 'A1B2C3D4', 'WarehouseMangerToken': 'K11L12M13'}]

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
        "contactEmail": "testmail@example.com;mBLDMB:",
        "restrictedClassificationsList": ["4.1", "5.2"]
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

def test_post_invalid_classifications_integration(_data):
    url = _data[0]["URL"] + 'Warehouses'
    admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
    headers = get_headers(admin_api_token)
    
    body = {
        "code": "WH_INVALID",
        "name": "Invalid Classification Test",
        "address": "123 Invalid Lane",
        "zip": "12345",
        "city": "InvalidCity",
        "province": "InvalidProvince",
        "country": "InvalidCountry",
        "contactName": "Invalid Tester",
        "contactPhone": "555-6789",
        "contactEmail": "invalidtester@example.com",
        "restrictedClassificationsList": ["InvalidClass"]
    }

    # Send a POST request to the API and expect it to fail
    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 400

    # Verify the response contains the expected plain text message
    expected_message = "Invalid classification 'InvalidClass'."
    assert post_response.text == expected_message, f"Expected: {expected_message}, Got: {post_response.text}"

def test_put_warehouses_integration(_data):
    url = _data[0]["URL"] + 'Warehouses/1'
    admin_api_token = _data[0]["AdminApiToken"] 
    warehousemanager_api_token = _data[0]["WarehouseMangerToken"]
    headers = get_headers(admin_api_token)
    headers2 = get_headers(warehousemanager_api_token)

    
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
        "contactEmail": "testmail@example.com",
        "restrictedClassificationsList": ["4.1", "5.2"]
    }

    # Get the original warehouse data before PUT
    original_response = requests.get(url, headers=headers)
    assert original_response.status_code == 200, "Failed to fetch the original warehouse data"
    original_data = original_response.json()
    print(f"Original Data: {original_data}")

    # Send a PUT request to update the warehouse
    put_response = requests.put(url, json=body, headers=headers2)
    assert put_response.status_code == 200, "PUT request failed"
    updated_data = put_response.json()
    print(f"Updated Data: {updated_data}")

    # Fetch the updated warehouse data
    get_response = requests.get(url, headers=headers)
    assert get_response.status_code == 200, "Failed to fetch updated warehouse data"
    fetched_data = get_response.json()
    print(f"Fetched Data: {fetched_data}")

    # Restore the original warehouse data
    restore_response = requests.put(url, json=original_data, headers=headers2)
    assert restore_response.status_code == 200, "Failed to restore original warehouse data"
    restored_data = restore_response.json()
    print(f"Restored Data: {restored_data}")

    # Verify the updated fields
    assert fetched_data["warehouseId"] == 1, "Warehouse ID mismatch"
    assert fetched_data["name"] == body["name"], f"Name mismatch: {fetched_data['name']} != {body['name']}"
    assert fetched_data["address"] == body["address"], f"Address mismatch: {fetched_data['address']} != {body['address']}"


def test_delete_warehouses_integration(_data):
    # Make a POST request first to make a dummy client
    url = _data[0]["URL"] + 'Warehouses/1'
    headers = get_headers(_data[0]["AdminApiToken"])

    dummy_get = requests.get(url, headers=headers)
    dummyJson = dummy_get.json()

    # Send a DELETE request to the API and check if it was successful
    delete_response = requests.delete(url + "/test", headers=headers)
    assert delete_response.status_code == 200

    get_response = requests.get(url, headers=headers)

    dummy_response = requests.put(url, json=dummyJson, headers=headers)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = None 
    try:
        response_data = get_response.json()
    except:
        pass
    
    print(response_data)
    # Verify that the status code is 404 (Not Found) and the client no longer exists
    assert status_code == 200 and response_data["softDeleted"] == True

# def test_delete_warehouses_integration(_data):
#     # Make a POST request first to create a dummy warehouse
#     url = _data[0]["URL"] + 'Warehouses'
#     admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
#     headers = get_headers(admin_api_token)
    
#     body = {
#         "code": "WH9999",
#         "name": "dummy",
#         "address": "dummy",
#         "zip": "12345",
#         "city": "dummy",
#         "province": "dummy",
#         "country": "dummy",
#         "contactName": "John Doe",
#         "contactPhone": "555-1234",
#         "contactEmail": "testmail@example.com",
#         "restrictedClassificationsList": ["4.1", "5.2"]
#     }

#     # Send a POST request to the API and check if it was successful
#     post_response = requests.post(url, json=body, headers=headers)
#     assert post_response.status_code == 200
#     warehouse_id = post_response.json().get("warehouseId")
    
#     url += f"/{warehouse_id}"

#     # Send a DELETE request to the API and check if it was successful
#     delete_response = requests.delete(url, headers=headers)
#     assert delete_response.status_code == 200

#     get2_response = requests.get(url, headers=headers)

#     # Get the status code and response data
#     status_code = get2_response.status_code
#     response_data = None 
#     try:
#         response_data = get2_response.json()
#     except:
#         pass

#     # Verify that the warehouse does not exist anymore (404 response)
#     assert status_code == 404 and response_data == None
