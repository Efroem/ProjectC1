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
    return [{'URL': 'http://localhost:5000/api/v1/', 'AdminApiToken': 'A1B2C3D4'}]


# Helper function to get headers with AdminApiToken
def get_headers(admin_api_token):
    return {"ApiToken": admin_api_token}

# Test to get suppliers integration
def test_get_suppliers_integration(_data):
    url = _data[0]["URL"] + 'suppliers'
    admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
    
    headers = get_headers(admin_api_token)
    
    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and len(response_data) >= 1


# Test to get supplier by id
def test_get_supplier_by_id_integration(_data):
    url = _data[0]["URL"] + 'suppliers/1'
    admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
    
    headers = get_headers(admin_api_token)
    
    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify that the status code is 200 (OK)
    assert status_code == 200 and response_data["supplierId"] == 1


# Test to post suppliers
def test_post_suppliers_integration(_data):
    url = _data[0]["URL"] + 'suppliers'
    admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
    
    headers = get_headers(admin_api_token)
    
    body = {
        "name": "Lee, Parks and Johnson",
        "code": "SUP9999",
        "address": "5989 Sullivan Drives",
        "addressExtra": "Apt. 996",
        "city": "Port Anitaburgh",
        "zipCode": "91688",
        "province": "Illinois",
        "country": "Czech Republic",
        "contactName": "Toni Barnett",
        "phonenumber": "3635417282",
        "reference": "LPaJ-SUP0001"
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, json=body, headers=headers)
    supplierDummy = post_response.json()

    assert post_response.status_code == 201
    supplier_id = supplierDummy.get("supplierId")
    
    get_response = requests.get(f"{url}/{supplier_id}", headers=headers)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()

    # Delete the supplier after verification
    delete_response = requests.delete(f"{url}/{supplier_id}", headers=headers)
    assert delete_response.status_code == 200


# Test to put suppliers (update supplier)
def test_put_suppliers_integration(_data):
    url = _data[0]["URL"] + 'suppliers/1'
    admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
    
    headers = get_headers(admin_api_token)
    
    body = {
        "supplierId": 1,
        "code": "SUP0001",
        "name": "Lee, Parks and Johnson",
        "address": "5989 Sullivan Drives",
        "addressExtra": "Apt. 996",
        "city": "Port Anitaburgh",
        "zipCode": "91688",
        "province": "Illinois",
        "country": "Czech Republic",
        "contactName": "Toni Barnett",
        "phonenumber": "3635417282",
        "reference": "LPaJ-SUP0001"
    }
    
    # Get the original data before PUT
    dummy_get = requests.get(url, headers=headers)
    dummyJson = dummy_get.json()
    
    # Send a PUT request to the API and check if it was successful
    put_response = requests.put(url, json=body, headers=headers)
    assert put_response.status_code == 200
    supplier_id = put_response.json().get("supplierId")
    
    get_response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = get_response.status_code
    response_data = get_response.json()
    
    # Restore the original data
    requests.put(url, json=dummyJson, headers=headers)
    
    # Verify that the status code is 200 (OK) and the body in this code and the response data are basically equal
    assert status_code == 200 and response_data["supplierId"] == supplier_id and response_data["name"] == body["name"] and response_data["address"] == body["address"]


# Test to delete suppliers
def test_delete_suppliers_integration(_data):
    # Make a POST request first to make a dummy supplier
    url = _data[0]["URL"] + 'suppliers'
    admin_api_token = _data[0]["AdminApiToken"]  # Extract token from the fixture
    
    headers = get_headers(admin_api_token)
    
    body = {
        "supplierId": 99999,
        "code": "SUP9999",
        "name": "Lee, Parks and Johnson",
        "address": "5989 Sullivan Drives",
        "addressExtra": "Apt. 996",
        "city": "Port Anitaburgh",
        "zipCode": "91688",
        "province": "Illinois",
        "country": "Czech Republic",
        "contactName": "Toni Barnett",
        "phonenumber": "3635417282",
        "reference": "LPaJ-SUP0001"
    }

    # Send a POST request to the API and check if it was successful
    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 201
    supplier_id = post_response.json().get("supplierId")
    
    url += f"/{supplier_id}"

    # Send a DELETE request to the API and check if it was successful
    delete_response = requests.delete(url, headers=headers)
    assert delete_response.status_code == 200

    get2_response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = get2_response.status_code
    response_data = None 
    try:
        response_data = get2_response.json()
    except:
        pass

    # Verify that the status code is 404 (Not Found) as the supplier doesn't exist anymore
    assert status_code == 404
