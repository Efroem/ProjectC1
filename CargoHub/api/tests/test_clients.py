# Test file for all GET methods in the code

# To run tests. run the following command
# $env:PYTHONPATH="<root directory>" ; pytest api/tests/test_clients.py
import pytest
import requests
import sys
import os
import json

sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))) 

from api.models import clients
from api.models import orders
from api.providers import data_provider

data_provider.init()

# print(data_provider.fetch_client_pool().get_clients())


@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:3000/api/v1/'}, {"API_KEY": "a1b2c3d4e5"}]


def test_clients_response_no_key_integration(_data):
    url = _data[0]["URL"] + 'clients'
    # params = {'id': 12}

    # Send a GET request to the API
    response = requests.get(url)

    # Get the status code and response data
    status_code = response.status_code
    # response_data = response.json()

    # Verify that the status code is 401 (Unauthorized)
    assert status_code == 401

    # Verify the response data
    # assert response_data['id'] == 123
    # assert response_data['name'] == 'John Smith'


def test_get_clients_integration(_data):
    url = _data[0]["URL"] + 'clients'
    # params = {'id': 12}
    header = _data[1]

    # Send a GET request to the API
    response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()
    # response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1


def test_get_client_by_id_integration(_data):
    url = _data[0]["URL"] + 'clients/12'
    # params = {'id': 12}
    header = _data[1]

    # Send a GET request to the API
    response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and response_data["id"] == 12

    # Verify the response data
    # assert response_data['id'] == 123
    # assert response_data['name'] == 'John Smith'

def test_post_clients_integration(_data):
    url = _data[0]["URL"] + 'clients'
    # params = {'id': 12}
    header = _data[1]
    body = {
        "id": 9999999,
        "name": "Test-Test",
        "address": "12345 Test Suite 420",
        "city": "South Anthonymouth",
        "zip_code": "12345",
        "province": "Test-Province",
        "country": "United States",
        "contact_name": "Testy Testra",
        "contact_phone": "431-688-3019",
        "contact_email": "test@example.net",
        "created_at": "2013-04-06 08:29:08",
        "updated_at": "2016-11-24 21:12:24"
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, headers=header, json=body)
    assert post_response.status_code == 201

    get_response = requests.get(url + "/9999999", headers=header)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()
    # response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and response_data["id"] == 9999999
    dummy = requests.delete(url + "/999999", headers=header)


def test_put_clients_integration(_data):
    url = _data[0]["URL"] + 'clients/24'
    # params = {'id': 12}
    header = _data[1]
    body = {
        "id": 24,
        "name": "Test-Test",
        "address": "12345 Test Suite 420",
        "city": "South Anthonymouth",
        "zip_code": "12345",
        "province": "Test-Province",
        "country": "United States",
        "contact_name": "Testy Testra",
        "contact_phone": "431-688-3019",
        "contact_email": "test@example.net",
        "created_at": "2013-04-06 08:29:08",
        "updated_at": "2016-11-24 21:12:24"
    }

    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, headers=header, json=body)
    assert put_response.status_code == 200

    get_response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()
    # response_data = response.json()

    # Verify that the status code is 200 (OK) and the body in this code and the response data are basically equal
    assert status_code == 200 and response_data["id"] == body["id"] and response_data["name"] == body["name"] and response_data["address"] == body["address"]

def test_delete_clients_integration(_data):
    url = _data[0]["URL"] + 'clients/35'
    header = _data[1]

    get1_response = requests.get(url, headers=header)

    # Send a DELETE request to the API and check if it was successful
    delete_response = requests.delete(url, headers=header)
    assert delete_response.status_code == 200

    get2_response = requests.get(url, headers=header)

    # Get the status code and response data
    status_code = get2_response.status_code
    response_data = get2_response.json()

    # Verify that the status code is 200 (OK) and that the client doesn't exist anymore
    assert status_code == 200 and response_data == None
    
    post_response = requests.post(url, headers=header, json=get1_response.json())









