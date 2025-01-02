# Test file for all GET methods in the code


import pytest
import requests
import sys
import os
import json
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))) 

@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:5000/api/v1/'}]


def test_get_inventory_integration(_data):
    url = _data[0]["URL"] + 'Inventories'
    # params = {'id': 12}

    # Send a GET request to the API
    response = requests.get(url)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json().get("result")
    # response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1


def test_get_inventory_by_id_integration(_data):
    url = _data[0]["URL"] + 'Inventories/1'
    # params = {'id': 12}

    # Send a GET request to the API
    response = requests.get(url)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json().get("result")

    # Verify that the status code is 200 (OK)
    print(response_data)
    assert status_code == 200 and response_data["inventoryId"] == 1

    # Verify the response data
    # assert response_data['id'] == 123
    # assert response_data['ItemReference'] == 'John Smith'

def test_post_inventory_integration(_data):
    url = _data[0]["URL"] + 'Inventories'
    # params = {'id': 12}
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
    post_response = requests.post(url, json=body)
    assert post_response.status_code == 200
    inventoryId = post_response.json().get("inventoryId")
    
    get_response = requests.get(f"{url}/{inventoryId}")

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()["result"]
    # response_data = response.json()
    # Verify that the status code is 200 (OK)
    print(inventoryId)
    print(response_data)
    dummy = requests.delete(f"{url}/{inventoryId}")
    assert status_code == 200 and response_data["itemReference"] == body["itemReference"] and response_data["description"] == body["description"]


def test_put_inventory_integration(_data):
    url = _data[0]["URL"] + 'Inventories/1'
    # params = {'id': 12}
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
    dummy_get = requests.get(url)
    assert dummy_get.status_code == 200
    dummyJson = dummy_get.json().get("result")
    # print(dummyJson)

    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, json=body)
    assert put_response.status_code == 200
    inventoryId = put_response.json().get("inventoryId")

    # Get the status code and response data
    get_response = requests.get(url)
    status_code = get_response.status_code
    response_data = get_response.json()["result"]
    # response_data = response.json()
    # Change the data back for cleanup
    dummy = requests.put(url, json=dummyJson)
    # Verify that the status code is 200 (OK) and the body in this code and the response data are basically equal
    assert status_code == 200 and response_data["inventoryId"] == inventoryId and response_data["itemReference"] == body["itemReference"] and response_data["description"] == body["description"]


def test_delete_inventory_integration(_data):
    # Make a POST reqeust first to make a dummy warehouse
    url = _data[0]["URL"] + 'Inventories'
    # params = {'id': 12}
    body = {
        "itemId": "P000001",
        "description": "Dummy Test",
        "itemReference": "REF-999999999",
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
    post_response = requests.post(url, json=body)
    assert post_response.status_code == 200
    inventoryId = post_response.json().get("inventoryId")
    
    url += f"/{inventoryId}"

    get2_response = requests.get(url)
    print(get2_response.json())

    # Send a DELETE request to the API and check if it was successful
    delete_response = requests.delete(url)
    assert delete_response.status_code == 200

    get2_response = requests.get(url)


    # Get the status code and response data
    status_code = get2_response.status_code
    response_data = None 
    try:
        response_data = get2_response.json()
    except:
        pass

    # Verify that the status code is 200 (OK) and that the warehouse doesn't exist anymore
    assert status_code == 404 and response_data == None









