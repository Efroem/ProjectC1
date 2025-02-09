import pytest
import requests
import os
from dotenv import load_dotenv

load_dotenv()

BASE_URL = "http://localhost:5000/api/v1/reporting"

@pytest.fixture
def admin_api_token():
    return os.getenv('AdminApiToken')

def get_headers(admin_api_token):
    return {"ApiToken": admin_api_token}

@pytest.mark.parametrize(
    "entity,from_date,to_date,warehouse_id,expected_status",
    [
        ("clients", "2018-01-01", "2023-12-31", None, 200),
        ("suppliers", "2018-01-01", "2023-12-31", None, 200),
        ("warehouses", "2003-02-13", "2023-12-31", 50, 200),
        ("invalidEntity", "2023-01-01", "2023-12-31", None, 400),
        ("clients", "INVALID_DATE", "2023-12-31", None, 400),  # Expected fail due to invalid date
        ("clients", "2018-01-01", "INVALID_DATE", None, 400),  # Expected fail due to invalid date
        ("clients", "2023-12-31", "2018-01-01", None, 400),    # Expected fail due to invalid date range
    ],
)
def test_reporting_endpoint(admin_api_token, entity, from_date, to_date, warehouse_id, expected_status):
    params = {
        "entity": entity,
        "fromDate": from_date,
        "toDate": to_date,
    }
    if warehouse_id:
        params["warehouseId"] = warehouse_id

    headers = get_headers(admin_api_token)

    response = requests.get(BASE_URL, params=params, headers=headers)
    assert response.status_code == expected_status

def test_reporting_no_data(admin_api_token):
    params = {
        "entity": "clients",
        "fromDate": "1900-01-01",
        "toDate": "1900-12-31",
    }
    
    headers = get_headers(admin_api_token)
    response = requests.get(BASE_URL, params=params, headers=headers)
    
    assert response.status_code == 400
    assert "No report data" in response.text

def test_reporting_missing_entity(admin_api_token):
    params = {
        "fromDate": "2023-01-01",
        "toDate": "2023-12-31",
    }
    headers = get_headers(admin_api_token)
    response = requests.get(BASE_URL, params=params, headers=headers)
    
    assert response.status_code == 400

def test_reporting_invalid_token():
    params = {
        "entity": "clients",
        "fromDate": "2023-01-01",
        "toDate": "2023-12-31",
    }
    headers = {"ApiToken": "INVALID_TOKEN"}
    response = requests.get(BASE_URL, params=params, headers=headers)
    
    assert response.status_code == 403  # Assuming invalid tokens return 403
