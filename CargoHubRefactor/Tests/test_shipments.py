import sys
import os
import pytest
import requests

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))


@pytest.fixture
def _data():
    return [{'URL': 'http://localhost:5000/api/v1/', 'AdminApiToken': 'A1B2C3D4', "FloorManagerApiToken": "E5F6G7", }]


def get_headers(admin_api_token):
    return {"ApiToken": admin_api_token}


def test_get_shipments_integration(_data):
    url = _data[0]["URL"] + 'shipments'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify status code is 200 (OK) and at least 1 shipment exists
    assert status_code == 200 and len(response_data) >= 1


def test_get_shipment_by_id_integration(_data):
    url = _data[0]["URL"] + 'shipments/1'
    headers = get_headers(_data[0]["AdminApiToken"])

    # Send a GET
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code
    response_data = response.json()

    # Verify status code 200 (OK) and shipment id match
    assert status_code == 200 and response_data["shipmentId"] == 1


def test_invalid_get_shipments_integration_FloorManagerAPIKey(_data):
    url = _data[0]["URL"] + 'orders'
    headers = get_headers(_data[0]["FloorManagerApiToken"])

    # Send a GET request to the API
    response = requests.get(url, headers=headers)

    # Get the status code and response data
    status_code = response.status_code

    # Verify that the status code is 401 (Unauthorized)
    assert status_code == 403, f"Expected status code 403, got {status_code}"


def test_post_shipment_integration(_data):
    url = _data[0]["URL"] + 'shipments'
    headers = get_headers(_data[0]["AdminApiToken"])
    body = {
        "sourceId": 1255,
        "orderIdsList": ["101", "102"],
        "shipmentDate": "2024-12-15T10:00:00Z",
        "shipmentStatus": "Pending",
        "shipmentType": "Standard",
        "carrierCode": "Carrier001",
        "carrierDescription": "Carrier Description",
        "serviceCode": "Service01",
        "paymentType": "Prepaid",
        "transferMode": "Air",
        "totalPackageCount": 2,
        "totalPackageWeight": 5.0,
        "notes": "Handle with care"
    }

    # Send a POST request
    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 200
    shipment_id = post_response.json().get("shipmentId")

    # Get created shipment
    get_response = requests.get(f"{url}/{shipment_id}", headers=headers)
    status_code = get_response.status_code
    response_data = get_response.json()

    # Delete the created shipment
    requests.delete(f"{url}/{shipment_id}", headers=headers)

    # Verify created successfully
    assert status_code == 200 and response_data["shipmentStatus"] == body["shipmentStatus"]


def test_delete_shipment_integration(_data):
    # Create a shipment
    url = _data[0]["URL"] + 'shipments'
    headers = get_headers(_data[0]["AdminApiToken"])
    body = {
        "sourceId": 1255,
        "orderIdsList": ["104", "105"],
        "shipmentDate": "2024-12-15T10:00:00Z",
        "shipmentStatus": "Pending",
        "shipmentType": "Standard",
        "carrierCode": "Carrier003",
        "carrierDescription": "Carrier Description",
        "serviceCode": "Service03",
        "paymentType": "Prepaid",
        "transferMode": "Sea",
        "totalPackageCount": 2,
        "totalPackageWeight": 7.5,
        "notes": "Handle with care"
    }

    post_response = requests.post(url, json=body, headers=headers)
    shipment_id = post_response.json().get("shipmentId")

    # Send a DELETE
    delete_response = requests.delete(f"{url}/{shipment_id}", headers=headers)
    assert delete_response.status_code == 200

    # Verify deleted
    get_response = requests.get(f"{url}/{shipment_id}", headers=headers)
    assert get_response.status_code == 404


def test_put_shipment_integration(_data):
    url = _data[0]["URL"] + 'shipments'
    headers = get_headers(_data[0]["AdminApiToken"])

    body = {
        "sourceId": 1255,
        "orderIdsList": ["201", "202"],
        "shipmentDate": "2024-12-20T10:00:00Z",
        "shipmentStatus": "Pending",
        "shipmentType": "Express",
        "carrierCode": "Carrier005",
        "carrierDescription": "Priority Shipping",
        "serviceCode": "Service05",
        "paymentType": "Prepaid",
        "transferMode": "Truck",
        "totalPackageCount": 4,
        "totalPackageWeight": 10.5,
        "notes": "Urgent shipment"
    }

    post_response = requests.post(url, json=body, headers=headers)
    assert post_response.status_code == 200
    shipment_id = post_response.json().get("shipmentId")

    put_body = {
        "shipmentStatus": "In Transit"
    }
    put_response = requests.put(f"{url}/{shipment_id}/status", json=put_body, headers=headers)

    assert put_response.status_code == 200
    put_response_data = put_response.json()
    assert put_response_data["shipmentId"] == shipment_id
    assert put_response_data["shipmentStatus"] == "In Transit"

    get_response = requests.get(f"{url}/{shipment_id}", headers=headers)
    assert get_response.status_code == 200
    get_response_data = get_response.json()
    assert get_response_data["shipmentStatus"] == "In Transit"

    delete_response = requests.delete(f"{url}/{shipment_id}", headers=headers)
    assert delete_response.status_code == 200
