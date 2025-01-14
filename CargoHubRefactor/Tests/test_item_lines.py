import pytest
import requests
import sys
import os
import json
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))) 

@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:5000/api/v1/', 'AdminApiToken': 'A1B2C3D4'}]

# Helper function to get headers with AdminApiToken
def get_headers(admin_api_token):
    return {"ApiToken": admin_api_token}

def test_get_item_lines_integration(_data):
    url = _data[0]["URL"] + 'Item_Lines'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1

def test_get_item_lines_by_id_integration(_data):
    url = _data[0]["URL"] + 'Item_Lines/1'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and response_data["lineId"] == 1

def test_post_item_lines_integration(_data):
    url = _data[0]["URL"] + 'Item_Lines'
    headers = get_headers(_data[0]["AdminApiToken"])
    body = {
        "name": "Test-Test",
        "description": "Test-Test-Test",
        "itemGroup": 1
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 200
    lineId = post_response.json().get("lineId")
    
    get_response = requests.get(f"{url}/{lineId}", headers=headers)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and response_data["name"] == body["name"] and response_data["description"] == body["description"]

    # Cleanup by deleting the created item line
    requests.delete(f"{url}/{lineId}", headers=headers)

def test_put_item_lines_integration(_data):
    url = _data[0]["URL"] + 'Item_Lines/1'
    headers = get_headers(_data[0]["AdminApiToken"])
    body = {
        "name": "Test-Test",
        "description": "Test-Test-Test",
        "itemGroup": 1
    }
    
    # Get the current item line data
    dummy_get = requests.get(url, headers=headers)
    assert dummy_get.status_code == 200
    dummyJson = dummy_get.json()

    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, json=body, headers=headers)
    assert put_response.status_code == 200
    lineId = put_response.json().get("lineId")

    # Get the status code and response data
    get_response = requests.get(url, headers=headers)
    status_code = get_response.status_code
    response_data = get_response.json()

    # Verify that the status code is 200 (OK) and the body matches
    assert status_code == 200 and response_data["lineId"] == lineId and response_data["name"] == body["name"] and response_data["description"] == body["description"]

    # Revert changes to the original data
    requests.put(url, json=dummyJson, headers=headers)

def test_delete_item_lines_integration(_data):
    # Make a POST request first to make a dummy client
    url = _data[0]["URL"] + 'Item_Lines/1'
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

# def test_delete_item_lines_integration(_data):
#     url = _data[0]["URL"] + 'Item_Lines'
#     headers = get_headers(_data[0]["AdminApiToken"])
#     body = {
#         "name": "dummy",
#         "description": "dummy",
#         "itemGroup": 1
#     }

#     # Send a POST request to the API and check if it was successful
#     post_response = requests.post(url, json=body, headers=headers)
#     assert post_response.status_code == 200
#     lineId = post_response.json().get("lineId")
    
#     url += f"/{lineId}"

#     # Send a DELETE request to the API and check if it was successful
#     delete_response = requests.delete(url, headers=headers)
#     assert delete_response.status_code == 200

#     # Verify that the item line is deleted
#     get2_response = requests.get(url, headers=headers)
#     status_code = get2_response.status_code
#     response_data = None
#     try:
#         response_data = get2_response.json()
#     except:
#         pass

#     # Verify that the status code is 404 (Not Found) and the item line no longer exists
#     assert status_code == 404 and response_data is None
