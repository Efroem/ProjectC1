using System.Text.Json;
using Models;
namespace CargoHubRefactor.DbSetup
{


    public class ResourceObjectReturns
    {
        public ItemGroup ReturnItemGroupObject(Dictionary<string, System.Text.Json.JsonElement> itemGroupJson)
        {
            ItemGroup returnItemGroupObject = new ItemGroup();
            string format = "yyyy-MM-dd HH:mm:ss";
            try
            {
                returnItemGroupObject = new ItemGroup
                {
                    // GroupId = itemGroupJson["id"].GetInt32(),
                    Name = itemGroupJson["name"].GetString(),

                    Description = itemGroupJson["description"].GetString(),

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"GroupId: {itemGroupJson["id"].GetInt32()}\n {e}");
            }
            try
            {
                returnItemGroupObject.CreatedAt = DateTime.ParseExact(itemGroupJson["created_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
                returnItemGroupObject.UpdatedAt = DateTime.ParseExact(itemGroupJson["updated_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                // Do nothing
            }

            if (string.IsNullOrEmpty(returnItemGroupObject.Name))
            {
                throw new ArgumentException("Name cannot be null or empty");
            }

            return returnItemGroupObject;
        }

        public ItemLine ReturnItemLineObject(Dictionary<string, System.Text.Json.JsonElement> itemLineJson, Dictionary<int, List<int>> itemGroupRelations)
        {
            ItemLine returnItemLineObject = new ItemLine();
            int correspondingItemGroup = -1;
            Console.WriteLine($"ItemLine: {itemLineJson["id"].GetInt32()}\n corresponding Item Group: {correspondingItemGroup}");
            string format = "yyyy-MM-dd HH:mm:ss";
            foreach (KeyValuePair<int, List<int>> itemGroup in itemGroupRelations)
            {
                if (itemGroup.Value.Contains(itemLineJson["id"].GetInt32()))
                {
                    correspondingItemGroup = itemGroup.Key;
                    break;
                }
            }
            if (correspondingItemGroup == -1) return null;
            Console.WriteLine($"ItemLine: {itemLineJson["id"].GetInt32()}\n corresponding Item Group: {correspondingItemGroup}");
            try
            {
                returnItemLineObject = new ItemLine
                {
                    Name = itemLineJson["name"].GetString(),
                    ItemGroup = correspondingItemGroup,
                    Description = itemLineJson["description"].GetString(),

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"GroupId: {itemLineJson["id"].GetInt32()}\n {e}");
            }
            try
            {
                returnItemLineObject.CreatedAt = DateTime.ParseExact(itemLineJson["created_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
                returnItemLineObject.UpdatedAt = DateTime.ParseExact(itemLineJson["updated_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                // Do nothing
            }

            if (string.IsNullOrEmpty(returnItemLineObject.Name))
            {
                throw new ArgumentException("Name cannot be null or empty");
            }

            return returnItemLineObject;
        }

        public ItemType ReturnItemTypeObject(Dictionary<string, System.Text.Json.JsonElement> itemTypeJson, Dictionary<int, List<int>> itemLineRelations)
        {
            ItemType returnItemTypeObject = new ItemType();
            int correspondingItemLine = -1;
            string format = "yyyy-MM-dd HH:mm:ss";
            foreach (KeyValuePair<int, List<int>> itemLine in itemLineRelations)
            {
                if (itemLine.Value.Contains(itemTypeJson["id"].GetInt32()))
                {
                    correspondingItemLine = itemLine.Key;
                    break;
                }
            }
            if (correspondingItemLine == -1) return null;
            try
            {
                returnItemTypeObject = new ItemType
                {
                    Name = itemTypeJson["name"].GetString(),
                    ItemLine = correspondingItemLine,
                    Description = itemTypeJson["description"].GetString(),

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"GroupId: {itemTypeJson["id"].GetInt32()}\n {e}");
            }
            try
            {
                returnItemTypeObject.CreatedAt = DateTime.ParseExact(itemTypeJson["created_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
                returnItemTypeObject.UpdatedAt = DateTime.ParseExact(itemTypeJson["updated_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                // Do nothing
            }

            if (string.IsNullOrEmpty(returnItemTypeObject.Name))
            {
                throw new ArgumentException("Name cannot be null or empty");
            }

            return returnItemTypeObject;
        }

        public Supplier ReturnSupplierObject(Dictionary<string, System.Text.Json.JsonElement> supplierJson)
        {
            Supplier returnSupplierObject = new Supplier();
            string format = "yyyy-MM-dd HH:mm:ss"; // Define the expected date-time format

            try
            {
                returnSupplierObject = new Supplier
                {
                    // Mapping the JSON fields to Supplier properties
                    SupplierId = supplierJson["id"].GetInt32(),
                    Code = supplierJson["code"].GetString(),
                    Name = supplierJson["name"].GetString(),
                    Address = supplierJson["address"].GetString(),
                    AddressExtra = supplierJson["address_extra"].GetString(),
                    City = supplierJson["city"].GetString(),
                    ZipCode = supplierJson["zip_code"].GetString(),
                    Province = supplierJson["province"].GetString(),
                    Country = supplierJson["country"].GetString(),
                    ContactName = supplierJson["contact_name"].GetString(),
                    PhoneNumber = supplierJson["phonenumber"].GetString(),
                    Reference = supplierJson["reference"].GetString(),
                    CreatedAt = DateTime.UtcNow, // Default values; will be overridden below
                    UpdatedAt = DateTime.UtcNow, // Default values; will be overridden below
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing Supplier ID: {supplierJson["id"].GetInt32()}\n {e}");
            }

            // Parse created_at and updated_at with the specific format
            try
            {
                returnSupplierObject.CreatedAt = DateTime.ParseExact(
                    supplierJson["created_at"].GetString(),
                    format,
                    System.Globalization.CultureInfo.InvariantCulture
                );
                returnSupplierObject.UpdatedAt = DateTime.ParseExact(
                    supplierJson["updated_at"].GetString(),
                    format,
                    System.Globalization.CultureInfo.InvariantCulture
                );
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Date parsing error for Supplier ID: {returnSupplierObject.SupplierId}\n {e}");
            }

            return returnSupplierObject;
        }

        public Warehouse ReturnWarehouseObject(Dictionary<string, System.Text.Json.JsonElement> warehouseJson)
        {
            Warehouse returnWarehouseObject = new Warehouse();
            string format = "yyyy-MM-dd HH:mm:ss"; // Define the expected date-time format

            try
            {
                returnWarehouseObject = new Warehouse
                {
                    // Mapping the JSON fields to Warehouse properties
                    WarehouseId = warehouseJson["id"].GetInt32(),
                    Code = warehouseJson["code"].GetString(),
                    Name = warehouseJson["name"].GetString(),
                    Address = warehouseJson["address"].GetString(),
                    Zip = warehouseJson["zip"].GetString(), // Mapping the "zip" field from the JSON to "ZipCode" in Warehouse
                    City = warehouseJson["city"].GetString(),
                    Province = warehouseJson["province"].GetString(),
                    Country = warehouseJson["country"].GetString(),

                    // Contact is a nested object, so we handle it separately
                    ContactName = warehouseJson["contact"].GetProperty("name").GetString(),
                    ContactPhone = warehouseJson["contact"].GetProperty("phone").GetString(),
                    ContactEmail = warehouseJson["contact"].GetProperty("email").GetString(),

                    // Date fields
                    CreatedAt = DateTime.ParseExact(warehouseJson["created_at"].GetString(), format, null),
                    UpdatedAt = DateTime.ParseExact(warehouseJson["updated_at"].GetString(), format, null),
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing Warehouse ID: {warehouseJson["id"].GetInt32()}\n {e}");
            }
            // Parse created_at and updated_at with the specific format
            try
            {
                returnWarehouseObject.CreatedAt = DateTime.ParseExact(
                    warehouseJson["created_at"].GetString(),
                    format,
                    System.Globalization.CultureInfo.InvariantCulture
                );
                returnWarehouseObject.UpdatedAt = DateTime.ParseExact(
                    warehouseJson["updated_at"].GetString(),
                    format,
                    System.Globalization.CultureInfo.InvariantCulture
                );
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Date parsing error for Supplier ID: {returnWarehouseObject.WarehouseId}\n {e}");
            }


            return returnWarehouseObject;
        }

        public Client ReturnClientObject(Dictionary<string, System.Text.Json.JsonElement> clientJson)
        {
            Client returnClientObject = new Client();
            string format = "yyyy-MM-dd HH:mm:ss"; // Define the expected date-time format

            try
            {
                returnClientObject = new Client
                {
                    // Mapping the JSON fields to Client properties
                    ClientId = clientJson["id"].GetInt32(),
                    Name = clientJson["name"].GetString(),
                    Address = clientJson["address"].GetString(),
                    ZipCode = clientJson["zip_code"].GetString(), // Mapping the "zip_code" field from the JSON to "ZipCode" in Client
                    City = clientJson["city"].GetString(),
                    Province = clientJson.ContainsKey("province") && !clientJson["province"].ValueKind.Equals(JsonValueKind.Null)
                        ? clientJson["province"].GetString()
                        : "Unknown", // Default value if province is null or missing
                    Country = clientJson["country"].GetString(),

                    // Contact information mapping
                    ContactName = clientJson["contact_name"].GetString(),
                    ContactPhone = clientJson["contact_phone"].GetString(),
                    ContactEmail = clientJson["contact_email"].GetString(),

                    // Date fields
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing Client ID: {clientJson["id"].GetInt32()}\n {e}");
            }

            try
            {
                returnClientObject.CreatedAt = DateTime.ParseExact(
                    clientJson["created_at"].GetString(),
                    format,
                    System.Globalization.CultureInfo.InvariantCulture
                );
                returnClientObject.UpdatedAt = DateTime.ParseExact(
                    clientJson["updated_at"].GetString(),
                    format,
                    System.Globalization.CultureInfo.InvariantCulture
                );
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Date parsing error for Supplier ID: {returnClientObject.ClientId}\n {e}");
            }

            return returnClientObject;
        }



        public Item ReturnItemObject(Dictionary<string, System.Text.Json.JsonElement> itemJson)
        {
            Item returnItemObject = new Item();
            string format = "yyyy-MM-dd HH:mm:ss"; // Define the expected date-time format

            Random random = new Random();

            double minValue = 4.0;
            double maxValue = 6.0;

            // Generate random value in the range [4, 6]
            double randomValue = minValue + (random.NextDouble() * (maxValue - minValue));

            // Round to 2 decimal places
            randomValue = Math.Round(randomValue, 2);

            Console.WriteLine(randomValue);

            try
            {
                returnItemObject = new Item
                {
                    // Mapping the JSON fields to Item properties
                    Uid = itemJson["uid"].GetString(),
                    Code = itemJson["code"].GetString(),
                    Description = itemJson["description"].GetString(),
                    ShortDescription = itemJson["short_description"].GetString(),
                    UpcCode = itemJson["upc_code"].GetString(),
                    ModelNumber = itemJson["model_number"].GetString(),
                    CommodityCode = itemJson["commodity_code"].GetString(),
                    ItemLine = itemJson["item_line"].GetInt32(),
                    ItemGroup = itemJson["item_group"].GetInt32(),
                    ItemType = itemJson["item_type"].GetInt32(),
                    Price = randomValue,
                    UnitPurchaseQuantity = itemJson["unit_purchase_quantity"].GetInt32(),
                    UnitOrderQuantity = itemJson["unit_order_quantity"].GetInt32(),
                    PackOrderQuantity = itemJson["pack_order_quantity"].GetInt32(),
                    SupplierId = itemJson["supplier_id"].GetInt32(),
                    SupplierCode = itemJson["supplier_code"].GetString(),
                    SupplierPartNumber = itemJson["supplier_part_number"].GetString(),
                    CreatedAt = DateTime.UtcNow, // Default value; will be overridden below
                    UpdatedAt = DateTime.UtcNow, // Default value; will be overridden below
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing Item UID: {itemJson["uid"].GetString()}\n {e}");
            }

            // Parse created_at and updated_at with the specific format
            try
            {
                returnItemObject.CreatedAt = DateTime.ParseExact(itemJson["created_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
                returnItemObject.UpdatedAt = DateTime.ParseExact(itemJson["updated_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Date parsing error for Item UID: {returnItemObject.Uid}\n {e}");
            }

            // Validate mandatory fields
            if (string.IsNullOrEmpty(returnItemObject.Uid))
            {
                throw new ArgumentException("Item UID cannot be null or empty");
            }

            return returnItemObject;
        }

        public Transfer ReturnTransferObject(Dictionary<string, JsonElement> transferJson)
        {
            try
            {
                return new Transfer
                {
                    TransferId = transferJson["id"].GetInt32(),
                    Reference = transferJson["reference"].GetString(),
                    TransferFrom = transferJson["transfer_from"].ValueKind == JsonValueKind.Null ? null : (int?)transferJson["transfer_from"].GetInt32(),
                    TransferTo = transferJson["transfer_to"].ValueKind == JsonValueKind.Null ? null : (int?)transferJson["transfer_to"].GetInt32(),
                    TransferStatus = transferJson["transfer_status"].GetString(),
                    CreatedAt = DateTime.Parse(transferJson["created_at"].GetString()),
                    UpdatedAt = DateTime.Parse(transferJson["updated_at"].GetString())
                };
            }
            catch (Exception ex)
            {
                File.AppendAllText("transfer_log.txt", $"{DateTime.Now}: Error converting transfer JSON: {ex.Message}\nStack Trace: {ex.StackTrace}\nTransfer JSON: {JsonSerializer.Serialize(transferJson)}{Environment.NewLine}");
                return null;
            }
        }

        public TransferItem ReturnTransferItemObject(JsonElement itemJson, int transferId)
        {
            try
            {
                return new TransferItem
                {
                    TransferId = transferId,
                    ItemId = itemJson.GetProperty("item_id").GetString(),
                    Amount = itemJson.GetProperty("amount").GetInt32()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting transfer item JSON: {ex.Message}\nItem JSON: {itemJson}");
                return null;
            }
        }

        public (Order orderObj, List<OrderItem> orderItems) ReturnOrderObject(Dictionary<string, System.Text.Json.JsonElement> orderJson)
        {
            (Order orderObj, List<OrderItem> orderItems) order;
            Order returnOrderObject = new Order();
            List<OrderItem> orderItems = new List<OrderItem>();
            string format = "yyyy-MM-ddTHH:mm:ssZ"; // Define the expected date-time format for the JSON (ISO 8601)

            try
            {
                returnOrderObject = new Order
                {
                    // Mapping the JSON fields to Order properties
                    Id = orderJson["id"].GetInt32(),
                    SourceId = orderJson["source_id"].ValueKind == JsonValueKind.Null ? 0 : orderJson["source_id"].GetInt32(),
                    Reference = orderJson["reference"].GetString(),
                    ReferenceExtra = orderJson["reference_extra"].GetString(),
                    OrderStatus = orderJson["order_status"].GetString(),
                    Notes = orderJson["notes"].GetString(),
                    ShippingNotes = orderJson["shipping_notes"].GetString(),
                    PickingNotes = orderJson["picking_notes"].GetString(),
                    WarehouseId = orderJson["warehouse_id"].GetInt32(),
                    ShipTo = orderJson["ship_to"].ValueKind == JsonValueKind.Null ? 0 : orderJson["ship_to"].GetInt32(), // Handle null case
                    BillTo = orderJson["bill_to"].ValueKind == JsonValueKind.Null ? 0 : orderJson["bill_to"].GetInt32(), // Handle null case
                    ShipmentId = orderJson["shipment_id"].ValueKind == JsonValueKind.Null ? 0 : orderJson["shipment_id"].GetInt32(),
                    TotalAmount = orderJson["total_amount"].GetDouble(),
                    TotalDiscount = orderJson["total_discount"].GetDouble(),
                    TotalTax = orderJson["total_tax"].GetDouble(),
                    TotalSurcharge = orderJson["total_surcharge"].GetDouble(),

                    // Date fields
                    CreatedAt = DateTime.UtcNow, // Default value; will be overridden below
                    UpdatedAt = DateTime.UtcNow, // Default value; will be overridden below
                    OrderDate = DateTime.UtcNow, // Default value; will be overridden below
                    RequestDate = DateTime.UtcNow // Default value; will be overridden below
                };
                // Parse created_at and updated_at with the specific format
                try
                {
                    returnOrderObject.CreatedAt = DateTime.ParseExact(orderJson["created_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
                    returnOrderObject.UpdatedAt = DateTime.ParseExact(orderJson["updated_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
                    returnOrderObject.OrderDate = DateTime.ParseExact(orderJson["order_date"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
                    returnOrderObject.RequestDate = DateTime.ParseExact(orderJson["request_date"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (FormatException e)
                {
                    Console.WriteLine($"Date parsing error for Order UID: {returnOrderObject.Id}\n {e}");
                }
                // Parse the 'items' array and map it to the OrderItems list
                if (orderJson.ContainsKey("items") && orderJson["items"].ValueKind == JsonValueKind.Array)
                {
                    foreach (var itemJson in orderJson["items"].EnumerateArray())
                    {
                        // Assuming each item in the array is an object with properties "item_id" and "amount"
                        var orderItem = new OrderItem
                        {
                            // Access item_id and amount properties
                            ItemId = itemJson.TryGetProperty("item_id", out var itemId) ? itemId.GetString() : null,
                            Amount = itemJson.TryGetProperty("amount", out var amount) ? amount.GetInt32() : 0
                        };

                        // Add the order item to the Items collection
                        orderItems.Add(orderItem);
                    }
                }

            }
            catch (Exception e)
            {
                // If an error occurs while processing the order object, log it
                Console.WriteLine($"Error processing Order ID: {orderJson["id"].GetInt32()}\n {e}");
            }
            order.orderObj = returnOrderObject;
            order.orderItems = orderItems;
            return order;
        }

        public Inventory ReturnInventoryObject(Dictionary<string, System.Text.Json.JsonElement> inventoryJson)
        {
            Inventory returnInventoryObject = new Inventory();
            string format = "yyyy-MM-dd HH:mm:ss"; // Define the expected date-time format

            try
            {
                returnInventoryObject = new Inventory
                {
                    // Mapping the JSON fields to Inventory properties
                    InventoryId = inventoryJson["id"].GetInt32(),
                    ItemId = inventoryJson["item_id"].GetString(),
                    Description = inventoryJson.ContainsKey("description") && !inventoryJson["description"].ValueKind.Equals(JsonValueKind.Null)
                        ? inventoryJson["description"].GetString()
                        : "No description available",  // Default value if description is missing or null
                    ItemReference = inventoryJson["item_reference"].GetString(),

                    // Handling Locations as JSON array and converting it to List<int>
                    LocationsList = inventoryJson["locations"].EnumerateArray()
                        .Select(location => location.GetInt32())
                        .ToList(),

                    TotalOnHand = inventoryJson["total_on_hand"].GetInt32(),
                    TotalExpected = inventoryJson["total_expected"].GetInt32(),
                    TotalOrdered = inventoryJson["total_ordered"].GetInt32(),
                    TotalAllocated = inventoryJson["total_allocated"].GetInt32(),
                    TotalAvailable = inventoryJson["total_available"].GetInt32(),
                    CreatedAt = DateTime.UtcNow, // Default value; will be overridden below
                    UpdatedAt = DateTime.UtcNow // Default value; will be overridden below
                };

                // Explicitly set LocationsString to JSON representation of Locations list
                // returnInventoryObject.Locations = JsonSerializer.Serialize(returnInventoryObject.Locations);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing Inventory ID: {inventoryJson["id"].GetInt32()}\n {e}");
            }
            // Parse created_at and updated_at with the specific format
            try
            {
                returnInventoryObject.CreatedAt = DateTime.ParseExact(inventoryJson["created_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
                returnInventoryObject.UpdatedAt = DateTime.ParseExact(inventoryJson["updated_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Date parsing error for Inventory ID: {returnInventoryObject.InventoryId}\n {e}");
            }

            return returnInventoryObject;
        }



        public (Shipment shipment, List<ShipmentItem> shipmentItems) ReturnShipmentObject(Dictionary<string, JsonElement> shipmentJson)
        {
            (Shipment shipmentObj, List<ShipmentItem> shipmentItems) shipment;
            Shipment returnShipmentObject = new Shipment();
            List<ShipmentItem> shipmentItems = new List<ShipmentItem>();
            string dateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ"; // Define the expected date-time format

            try
            {
                returnShipmentObject = new Shipment
                {
                    ShipmentId = shipmentJson["id"].GetInt32(),
                    OrderId = shipmentJson["order_id"].GetInt32().ToString(),
                    SourceId = shipmentJson["source_id"].GetInt32(),
                    OrderDate = DateTime.Parse(shipmentJson["order_date"].GetString()),
                    RequestDate = DateTime.Parse(shipmentJson["request_date"].GetString()),
                    ShipmentDate = DateTime.Parse(shipmentJson["shipment_date"].GetString()),
                    ShipmentType = shipmentJson["shipment_type"].GetString(),
                    ShipmentStatus = shipmentJson["shipment_status"].GetString(),
                    Notes = shipmentJson["notes"].GetString(),
                    CarrierCode = shipmentJson["carrier_code"].GetString(),
                    CarrierDescription = shipmentJson["carrier_description"].GetString(),
                    ServiceCode = shipmentJson["service_code"].GetString(),
                    PaymentType = shipmentJson["payment_type"].GetString(),
                    TransferMode = shipmentJson["transfer_mode"].GetString(),
                    TotalPackageCount = shipmentJson["total_package_count"].GetInt32(),
                    TotalPackageWeight = shipmentJson["total_package_weight"].GetDouble(),

                    // Default values in case parsing fails
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing Shipment ID: {shipmentJson["id"].GetInt32()}\n {e}");
            }

            try
            {
                // Parsing "created_at" and "updated_at" date fields
                returnShipmentObject.CreatedAt = DateTime.ParseExact(
                    shipmentJson["created_at"].GetString(),
                    dateTimeFormat,
                    System.Globalization.CultureInfo.InvariantCulture
                );
                returnShipmentObject.UpdatedAt = DateTime.ParseExact(
                    shipmentJson["updated_at"].GetString(),
                    dateTimeFormat,
                    System.Globalization.CultureInfo.InvariantCulture
                );
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Date parsing error for Shipment ID: {returnShipmentObject.ShipmentId}\n {e}");
            }

            // Parse "items" into a list of ShipmentItem objects
            try
            {
                if (shipmentJson.ContainsKey("items"))
                {
                    foreach (JsonElement itemElement in shipmentJson["items"].EnumerateArray())
                    {
                        ShipmentItem shipmentItem = new ShipmentItem
                        {
                            ShipmentId = returnShipmentObject.ShipmentId,
                            ItemId = itemElement.GetProperty("item_id").GetString(),
                            Amount = itemElement.GetProperty("amount").GetInt32(),
                        };
                        shipmentItems.Add(shipmentItem);
                    }

                    // Assuming a property or method exists to associate items with Shipment
                    shipment.shipmentItems = shipmentItems;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing items for Shipment ID: {returnShipmentObject.ShipmentId}\n {e}");
            }
            shipment.shipmentObj = returnShipmentObject;
            shipment.shipmentItems = shipmentItems;
            return shipment;
        }

        public Location ReturnLocationObject(Dictionary<string, JsonElement> locationJson)
        {
            Location returnLocationObject = new Location();
            string format = "yyyy-MM-dd HH:mm:ss"; // Define the expected date-time format

            try
            {
                returnLocationObject = new Location
                {
                    LocationId = locationJson["id"].GetInt32(),
                    WarehouseId = locationJson["warehouse_id"].GetInt32(),
                    ItemAmountsString = "",
                    Code = locationJson["code"].GetString(),
                    Name = locationJson["name"].GetString(),

                    // Default values in case parsing fails
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing Location ID: {locationJson["id"].GetInt32()}\n {e}");
            }

            try
            {
                // Parsing "created_at" and "updated_at" date fields
                returnLocationObject.CreatedAt = DateTime.ParseExact(
                    locationJson["created_at"].GetString(),
                    format,
                    System.Globalization.CultureInfo.InvariantCulture
                );
                returnLocationObject.UpdatedAt = DateTime.ParseExact(
                    locationJson["updated_at"].GetString(),
                    format,
                    System.Globalization.CultureInfo.InvariantCulture
                );
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Date parsing error for Location ID: {returnLocationObject.LocationId}\n {e}");
            }

            return returnLocationObject;
        }


    }
}

