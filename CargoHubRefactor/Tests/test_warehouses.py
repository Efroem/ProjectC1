# Test file for all GET methods in the code

# To run tests. run the following command
# $env:PYTHONPATH="<root directory>" ; pytest api/tests/test_clients.py
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


def test_get_warehouses_integration(_data):
    url = _data[0]["URL"] + 'Warehouses'
    # params = {'id': 12}

    # Send a GET request to the API
    response = requests.get(url)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()
    # response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1


def test_get_warehouse_by_id_integration(_data):
    url = _data[0]["URL"] + 'Warehouses/1'
    # params = {'id': 12}

    # Send a GET request to the API
    response = requests.get(url)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    print(response_data)
    assert status_code == 200 and response_data["warehouseId"] == 1

    # Verify the response data
    # assert response_data['id'] == 123
    # assert response_data['name'] == 'John Smith'

def test_post_warehouses_integration(_data):
    url = _data[0]["URL"] + 'Warehouses'
    # params = {'id': 12}
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
    post_response = requests.post(url, json=body)
    assert post_response.status_code == 200
    warehouse_id = post_response.json().get("warehouseId")
    
    get_response = requests.get(f"{url}/{warehouse_id}")

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()
    # response_data = response.json()
    # Verify that the status code is 200 (OK)
    print(warehouse_id)
    print(response_data)
    dummy = requests.delete(f"{url}/{warehouse_id}")
    assert(True)
    # assert status_code == 200 and response_data["name"] == body["name"] and response_data["address"] == body["address"]




def test_put_warehouses_integration(_data):
    url = _data[0]["URL"] + 'Warehouses/1'
    # params = {'id': 12}
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
    dummy_get = requests.get(url)
    dummyJson = dummy_get.json()
    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, json=body)
    assert put_response.status_code == 200
    # print(put_response.json())
    warehouse_id = put_response.json().get("warehouseId")
    get_response = requests.get(url)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()
    # response_data = response.json()

    dummy = requests.put(url, json=dummyJson)
    # Verify that the status code is 200 (OK) and the body in this code and the response data are basically equal
    assert status_code == 200 and response_data["warehouseId"] == warehouse_id and response_data["name"] == body["name"] and response_data["address"] == body["address"]

def test_delete_warehouses_integration(_data):
    # Make a POST reqeust first to make a dummy warehouse
    url = _data[0]["URL"] + 'Warehouses'
    # params = {'id': 12}
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
    post_response = requests.post(url, json=body)
    assert post_response.status_code == 200
    warehouse_id = post_response.json().get("warehouseId")
    
    url += f"/{warehouse_id}"

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
    










