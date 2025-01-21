import pytest
import requests
import sys
import os
import json
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))) 

@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:5000/api/v1/', 'AdminApiToken': 'A1B2C3D4', 'EmployeeApiToken': 'H8I9J10'}]

# Helper function to get headers with AdminApiToken
def get_headers(admin_api_token):
    return {"ApiToken": admin_api_token}

def test_get_inventory_integration(_data):
    url = _data[0]["URL"] + 'Inventories'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json().get("result")

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1

def test_get_inventory_by_id_integration(_data):
    url = _data[0]["URL"] + 'Inventories/1'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json().get("result")

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and response_data["inventoryId"] == 1

def test_inventory_invalid_apikey(_data):
    url = _data[0]["URL"] + 'Inventories/1'
    
    invalid_token = "INVALID_API_KEY"
    headers = {
        "ApiToken": invalid_token,
        "Content-Type": "application/json"
    }

    response = requests.get(url, headers=headers)

    assert response.status_code == 403

def test_post_inventory_integration(_data):
    url = _data[0]["URL"] + 'Inventories'
    headers = get_headers(_data[0]["AdminApiToken"])
    body = {
        "itemId": "P000001",
        "description": "Dummy inventory item",
        "itemReference": "REF-999",
        "locations": [
            1,
            2,
            3
        ],
        "totalOnHand": 100,
        "totalExpected": 150,
        "totalOrdered": 50,
        "totalAllocated": 30,
        "totalAvailable": 70
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 200
    inventoryId = post_response.json().get("inventoryId")
    
    # Get the created inventory to verify
    get_response = requests.get(f"{url}/{inventoryId}", headers=headers)
    status_code = get_response.status_code
    response_data = get_response.json().get("result")

    # Verify that the status code is 200 and the data matches
    assert status_code == 200 and response_data["itemReference"] == body["itemReference"] and response_data["description"] == body["description"]

    # Cleanup by deleting the created inventory
    requests.delete(f"{url}/{inventoryId}", headers=headers)

def test_put_inventory_integration(_data):
    url = _data[0]["URL"] + 'Inventories/1'
    headers = get_headers(_data[0]["AdminApiToken"])
    body = {
        "itemId": "P000001",
        "description": "Changed inventory item",
        "itemReference": "REF-999",
        "locations": [
            1,
            2,
            3
        ],
        "totalOnHand": 100,
        "totalExpected": 150,
        "totalOrdered": 50,
        "totalAllocated": 30,
        "totalAvailable": 70
    }

    # Get the current inventory data
    dummy_get = requests.get(url, headers=headers)
    assert dummy_get.status_code == 200
    dummy_json = dummy_get.json().get("result")

    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, json=body, headers=headers)
    assert put_response.status_code == 200
    inventoryId = put_response.json().get("inventoryId")

    # Verify the updated inventory
    get_response = requests.get(url, headers=headers)
    status_code = get_response.status_code
    response_data = get_response.json().get("result")

    # Verify the updated data
    assert status_code == 200 and response_data["inventoryId"] == inventoryId and response_data["itemReference"] == body["itemReference"] and response_data["description"] == body["description"]

    # Revert changes to original data
    requests.put(url, json=dummy_json, headers=headers)

def test_delete_inventory_integration(_data):
    # Make a POST request first to make a dummy client
    url = _data[0]["URL"] + 'Inventories/1'
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
        response_data = get_response.json().get("result")
    except:
        pass
    
    print(response_data)
    # Verify that the status code is 404 (Not Found) and the client no longer exists
    assert status_code == 200 and response_data["softDeleted"] == True

# def test_delete_inventory_integration(_data):
#     # Make a POST request first to create an inventory item
#     url = _data[0]["URL"] + 'Inventories'
#     headers = get_headers(_data[0]["AdminApiToken"])
#     body = {
#         "itemId": "P000001",
#         "description": "Dummy Test",
#         "itemReference": "REF-999999999",
#         "locations": [
#             1,
#             2,
#             3
#         ],
#         "totalOnHand": 100,
#         "totalExpected": 150,
#         "totalOrdered": 50,
#         "totalAllocated": 30,
#         "totalAvailable": 70
#     }

#     # Send a POST request to the API and check if it was successful
#     post_response = requests.post(url, json=body, headers=headers)
#     assert post_response.status_code == 200
#     inventoryId = post_response.json().get("inventoryId")
    
#     url += f"/{inventoryId}"

#     # Send a DELETE request to the API and check if it was successful
#     delete_response = requests.delete(url, headers=headers)
#     assert delete_response.status_code == 200

#     # Verify that the inventory is deleted by checking the response status
#     get_response = requests.get(url, headers=headers)
#     status_code = get_response.status_code
#     response_data = None
#     try:
#         response_data = get_response.json()
#     except:
#         pass

#     # Verify that the status code is 404 (Not Found) and the inventory is deleted
#     assert status_code == 404 and response_data is None
