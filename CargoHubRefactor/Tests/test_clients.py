import pytest
import requests
import sys
import os
import json

# Ensure the test can locate and import required modules
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))) 

@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:5000/api/v1/', 'AdminApiToken': 'A1B2C3D4', 'EmployeeApiToken': 'H8I9J10'}]

def get_headers(AdminApiToken):
    return {"ApiToken": AdminApiToken}

def test_get_clients_integration_AdminKey(_data):
    url = _data[0]["URL"] + 'Clients'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1

def test_get_clients_integration_EmployeeKey(_data):
    url = _data[0]["URL"] + 'Clients'
    headers = get_headers(_data[0]["EmployeeApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1

def test_get_client_by_id_integration_AdminKey(_data):
    url = _data[0]["URL"] + 'Clients/1'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    print(response_data)
    assert status_code == 200 and response_data["clientId"] == 1

def test_get_client_by_id_integration_EmployeeKey(_data):
    url = _data[0]["URL"] + 'Clients/1'
    headers = get_headers(_data[0]["EmployeeApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    print(response_data)
    assert status_code == 200 and response_data["clientId"] == 1
    
def test_post_clients_integration_AdminKey(_data):
    url = _data[0]["URL"] + 'Clients'
    headers = get_headers(_data[0]["AdminApiToken"])
    body = {
        "name": "Test-Test",
        "address": "12345 Test Suite 420",
        "city": "South Anthonymouth",
        "zipCode": "12345",
        "province": "Test-Province",
        "country": "United States",
        "contactName": "Testy Testra",
        "contactPhone": "431-688-3019",
        "contactEmail": "testtest@example.net"
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, headers=headers, json=body)
    assert post_response.status_code == 200
    client_id = post_response.json().get("clientId")
    
    get_response = requests.get(f"{url}/{client_id}", headers=headers)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()

    # Verify that the status code is 200 (OK)
    print(client_id)
    print(response_data)
    assert status_code == 200 and response_data["name"] == body["name"] and response_data["address"] == body["address"]

    requests.delete(f"{url}/{client_id}", headers=headers)
    
def test_post_clients_invalid_email(_data):
    url = _data[0]["URL"] + 'Clients'
    headers = get_headers(_data[0]["AdminApiToken"])
    
    body = {
        "name": "Test-Test",
        "address": "12345 Test Suite 420",
        "city": "South Anthonymouth",
        "zipCode": "12345",
        "province": "Test-Province",
        "country": "United States",
        "contactName": "Testy Testra",
        "contactPhone": "431-688-3019",
        "contactEmail": "testtestexample.net"  
    }

    post_response = requests.post(url, headers=headers, json=body)
    
    assert post_response.status_code == 400

def test_post_clients_integration_EmployeeKey(_data):
    url = _data[0]["URL"] + 'Clients'
    headers = get_headers(_data[0]["EmployeeApiToken"])
    body = {
        "name": "Test-Test",
        "address": "12345 Test Suite 420",
        "city": "South Anthonymouth",
        "zipCode": "12345",
        "province": "Test-Province",
        "country": "United States",
        "contactName": "Testy Testra",
        "contactPhone": "431-688-3019",
        "contactEmail": "testtest@example.net"
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, headers=headers, json=body)
    
    assert post_response.status_code == 403, f"Expected 403, got {post_response.status_code}"

def test_put_clients_integration_AdminKey(_data):
    url = _data[0]["URL"] + 'Clients/1'
    headers = get_headers(_data[0]["AdminApiToken"])
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

    # Get the current client data
    dummy_get = requests.get(url, headers=headers)
    dummy_json = dummy_get.json()

    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, headers=headers, json=body)
    assert put_response.status_code == 200
    client_id = put_response.json().get("clientId")

    get_response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()

    # Verify that the status code is 200 (OK) and the body in this code and the response data are basically equal
    assert (
        status_code == 200
        and response_data["clientId"] == client_id
        and response_data["name"] == body["name"]
        and response_data["address"] == body["address"]
    )

    # Revert changes for cleanup
    requests.put(url, headers=headers, json=dummy_json)

def test_put_clients_integration_EmployeeKey(_data):
    url = _data[0]["URL"] + 'Clients/1'
    headers = get_headers(_data[0]["EmployeeApiToken"])
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

    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, headers=headers, json=body)
    
    assert put_response.status_code == 403, f"Expected 403, got {put_response.status_code}"

def test_delete_clients_integration(_data):
    # Make a POST request first to make a dummy client
    url = _data[0]["URL"] + 'Clients/1'
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




# def test_delete_clients_integration(_data):
#     # Make a POST request first to make a dummy client
#     url = _data[0]["URL"] + 'Clients'
#     headers = get_headers(_data[0]["AdminApiToken"])
#     body = {
#         "name": "Jane Doe",
#         "address": "456 Secondary St",
#         "city": "New City",
#         "zipCode": "54321",
#         "province": "Berlin",
#         "country": "Germany",
#         "contactName": "Jane Doe",
#         "contactPhone": "+987654321",
#         "contactEmail": "jane.doe@example.com"
#     }

#     # Send a POST request to the API and check if it was successful
#     post_response = requests.post(url, headers=headers, json=body)
#     assert post_response.status_code == 200
#     client_id = post_response.json().get("clientId")
    
#     url += f"/{client_id}"

#     # Send a DELETE request to the API and check if it was successful
#     delete_response = requests.delete(url, headers=headers)
#     assert delete_response.status_code == 200

#     get_response = requests.get(url, headers=headers)

#     # Get the status code and response data
#     status_code = get_response.status_code
#     response_data = None 
#     try:
#         response_data = get_response.json()
#     except:
#         pass

#     # Verify that the status code is 404 (Not Found) and the client no longer exists
#     assert status_code == 404 and response_data is None

