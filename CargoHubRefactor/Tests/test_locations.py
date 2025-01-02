import sys
import os
import pytest
import requests

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))) 

@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:5000/api/v1/'}]

def test_get_locations_integration(_data):
    url = _data[0]["URL"] + 'locations'

    # stuurt GET request naar API
    response = requests.get(url)

    # krijg status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify dat de status code 200 is (OK)
    assert status_code == 200 and len(response_data) >= 1

def test_post_locations_integration(_data):
    url = _data[0]["URL"] + 'locations'
    body = {
        "name": "Row: A, Rack: 10, Shelf: 0",
        "code": "LOC1241231",
        "warehouseId": 1
    }

    # stuurt POST request naar API
    post_response = requests.post(url, json=body)

    location_id = post_response.json().get("supplierId")


    # Print the response for debugging
    print("Response status:", post_response.status_code)
    print("Response body:", post_response.text)

    dummyDelete = requests.delete(f"{url}/{location_id}")

    # Checkt of status code 201 is (Created)
    assert post_response.status_code == 200


def test_put_locations_integration(_data):
    url = _data[0]["URL"] + 'locations/1'
    body = {
        "locationId": 1,
        "name": "Row: B, Rack: 10, Shelf: 1",
        "code": "LOC001-UPDATED",
        "warehouseId": 1
    }
    dummy_get = requests.get(url)
    dummyJson = dummy_get.json()


    # stuurt PUT request naar API
    put_response = requests.put(url, json=body)

    ## dit is voor debuggen als t niet werkt
    print("Response status:", put_response.status_code)
    print("Response body:", put_response.text)


    dummy_response = requests.put(url, json=dummyJson)

    # Checkt of status code 200 is (OK)
    assert put_response.status_code == 200

def test_delete_locations_integration(_data):

    url = _data[0]["URL"] + 'locations'
    body = {
        "name": "Row: A, Rack: 10, Shelf: 0",
        "code": "LOC1241231",
        "warehouseId": 1
    }
    post_response = requests.post(url, json=body)
    assert post_response.status_code == 200
    location_id = post_response.json().get("locationId")

    get1_response = requests.get(f"{url}/{location_id}")

    

    # Als recourse bestaat, stuurt hij delete request
    if get1_response.status_code == 200:

        delete_response = requests.delete(f"{url}/{location_id}") 

        # Check if the DELETE request was successful
        assert delete_response.status_code == 200
    else:
        print("Resource with ID 3 not found.")

def test_get_location_invalid_id_integration(_data):
    url = _data[0]["URL"] + 'locations/999999999'
    
    # stuurt een GET request naar de API
    response = requests.get(url)
    
    # Verify dat de status code 404 is (Not Found)
    assert response.status_code == 404
