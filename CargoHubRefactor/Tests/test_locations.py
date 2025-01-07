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

def test_get_locations_integration(_data):
    url = _data[0]["URL"] + 'locations'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1

def test_post_locations_integration(_data):
    url = _data[0]["URL"] + 'locations'
    headers = get_headers(_data[0]["AdminApiToken"])
    body = {
        "name": "Row: A, Rack: 10, Shelf: 0",
        "code": "LOC1241231",
        "warehouseId": 1
    }

    # Send a POST request to the API
    post_response = requests.post(url, json=body, headers=headers)

    location_id = post_response.json().get("supplierId")

    # Print the response for debugging
    print("Response status:", post_response.status_code)
    print("Response body:", post_response.text)

    # Cleanup: Delete the location created
    dummyDelete = requests.delete(f"{url}/{location_id}", headers=headers)

    # Verify that the status code is 200 (OK)
    assert post_response.status_code == 200

def test_put_locations_integration(_data):
    url = _data[0]["URL"] + 'locations/1'
    headers = get_headers(_data[0]["AdminApiToken"])
    body = {
        "locationId": 1,
        "name": "Row: B, Rack: 10, Shelf: 1",
        "code": "LOC001-UPDATED",
        "warehouseId": 1
    }

    # Get current location data for comparison
    dummy_get = requests.get(url, headers=headers)
    dummyJson = dummy_get.json()

    # Send a PUT request to the API
    put_response = requests.put(url, json=body, headers=headers)

    # Debug information
    print("Response status:", put_response.status_code)
    print("Response body:", put_response.text)

    # Revert changes to original data
    dummy_response = requests.put(url, json=dummyJson, headers=headers)

    # Verify that the status code is 200 (OK)
    assert put_response.status_code == 200

def test_delete_locations_integration(_data):
    url = _data[0]["URL"] + 'locations'
    headers = get_headers(_data[0]["AdminApiToken"])
    body = {
        "name": "Row: A, Rack: 10, Shelf: 0",
        "code": "LOC1241231",
        "warehouseId": 1
    }

    # Send a POST request to create a new location
    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 200
    location_id = post_response.json().get("locationId")

    # Verify the resource exists
    get1_response = requests.get(f"{url}/{location_id}", headers=headers)

    if get1_response.status_code == 200:
        # Send a DELETE request to remove the location
        delete_response = requests.delete(f"{url}/{location_id}", headers=headers)

        # Verify that the DELETE request was successful
        assert delete_response.status_code == 200
    else:
        print("Resource with ID not found.")

def test_get_location_invalid_id_integration(_data):
    url = _data[0]["URL"] + 'locations/999999999'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Verify that the status code is 404 (Not Found)
    assert response.status_code == 404
