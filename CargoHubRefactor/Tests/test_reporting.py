import pytest
import requests

BASE_URL = "http://localhost:5000/api/v1/reporting"

@pytest.mark.parametrize(
    "entity,from_date,to_date,warehouse_id,expected_status",
    [
        ("clients", "2018-01-01", "2023-12-31", None, 200),
        ("suppliers", "2018-01-01", "2023-12-31", None, 200),
        ("warehouses", "2003-02-13", "2023-12-31", 50, 200),
        ("invalidEntity", "2023-01-01", "2023-12-31", None, 400),
    ],
)
def test_reporting_endpoint(entity, from_date, to_date, warehouse_id, expected_status):
    params = {
        "entity": entity,
        "fromDate": from_date,
        "toDate": to_date
    }
    if warehouse_id:
        params["warehouseId"] = warehouse_id

    response = requests.get(BASE_URL, params=params)
    assert response.status_code == expected_status

def test_reporting_no_data():
    params = {
        "entity": "clients",
        "fromDate": "1900-01-01",
        "toDate": "1900-12-31"
    }
    response = requests.get(BASE_URL, params=params)
    assert response.status_code == 400
    assert "No report data" in response.text
