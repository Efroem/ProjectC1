import pytest
import requests
import sys
import os
import json
from dotenv import load_dotenv

load_dotenv()

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

# Fixture to provide URL and AdminApiToken
@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:5000/api/v1/', 'AdminApiToken': os.getenv('AdminApiToken')}]

# Helper function to get headers with AdminApiToken
def get_headers(admin_api_token):
    return {"ApiToken": admin_api_token}

# Test to fetch all transfers
def test_get_all_transfers_integration(_data):
    url = _data[0]["URL"] + 'transfers'
    admin_api_token = _data[0]["AdminApiToken"]
    headers = get_headers(admin_api_token)

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)

def test_get_transfer_by_id_integration(_data):
    url = _data[0]["URL"] + 'transfers/1'
    admin_api_token = _data[0]["AdminApiToken"]
    headers = get_headers(admin_api_token)

    response = requests.get(url, headers=headers)

    if response.status_code == 200:
        assert response.json().get("transferId") == 1
    else:
        assert response.status_code == 404

def test_add_transfer_integration(_data):
    url = _data[0]["URL"] + 'transfers'
    admin_api_token = _data[0]["AdminApiToken"]
    headers = get_headers(admin_api_token)

    body = {
        "reference": "REF123",
        "transferFrom": 1,
        "transferTo": 2,
        "items": [
            {"itemId": "P000451", "amount": 5},
            {"itemId": "P002520", "amount": 3}
        ]
    }

    response = requests.post(url, json=body, headers=headers)

    if response.status_code == 201:
        transfer_id = response.json().get("transferId")
        assert transfer_id is not None

        get_response = requests.get(f"{url}/{transfer_id}", headers=headers)
        assert get_response.status_code == 200

        delete_response = requests.delete(f"{url}/{transfer_id}", headers=headers)
        assert delete_response.status_code == 200
    else:
        assert response.status_code == 400

def test_delete_transfer_integration(_data):
    # Make a POST request first to make a dummy client
    url = _data[0]["URL"] + 'transfers/1'
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
    assert response_data["softDeleted"] == True

# def test_update_transfer_logic_integration(_data):
#     url = _data[0]["URL"] + 'transfers'
#     admin_api_token = _data[0]["AdminApiToken"]
#     headers = get_headers(admin_api_token)

#     # Create a transfer
#     body = {
#         "reference": "REF456",
#         "transferFrom": 1,
#         "transferTo": 2,
#         "items": [
#             {"itemId": "P000451", "amount": 2}
#         ]
#     }

#     post_response = requests.post(url, json=body, headers=headers)
#     assert post_response.status_code == 201

#     transfer_id = post_response.json().get("transferId")

#     # Attempt to update while status is pending
#     update_body = {
#         "reference": "REF456-UPDATED",
#         "transferFrom": 1,
#         "transferTo": 2,
#         "items": [
#             {"itemId": "P002520", "amount": 1}
#         ]
#     }

#     update_response = requests.put(f"{url}/{transfer_id}", json=update_body, headers=headers)
#     assert update_response.status_code == 200

#     # Change status to InProgress
#     status_url = f"{url}/{transfer_id}/status"
#     status_body = "InProgress"
#     status_response = requests.put(status_url, json=status_body, headers=headers)
#     assert status_response.status_code == 200

#     # Attempt to update after status change
#     update_response = requests.put(f"{url}/{transfer_id}", json=update_body, headers=headers)
#     assert update_response.status_code == 400

#     # Delete transfer
#     delete_response = requests.delete(f"{url}/{transfer_id}", headers=headers)
#     assert delete_response.status_code == 200

def test_id_reuse_logic_integration(_data):
    url = _data[0]["URL"] + 'transfers'
    admin_api_token = _data[0]["AdminApiToken"]
    headers = get_headers(admin_api_token)

    # Create and delete a transfer
    body = {
        "reference": "REF789",
        "transferFrom": 1,
        "transferTo": 2,
        "items": [
            {"itemId": "P002520", "amount": 1}
        ]
    }

    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 201

    transfer_id = post_response.json().get("transferId")
    delete_response = requests.delete(f"{url}/{transfer_id}", headers=headers)
    assert delete_response.status_code == 200

    # Create another transfer and ensure ID reuse
    new_post_response = requests.post(url, json=body, headers=headers)
    assert new_post_response.status_code == 201
    new_transfer_id = new_post_response.json().get("transferId")
    assert new_transfer_id == transfer_id  # Verify ID reuse
