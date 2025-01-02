using System.Text.Json;
using Models;
namespace CargoHubRefactor.DbSetup {


    public class ResourceObjectReturns {
        public ItemGroup ReturnItemGroupObject(Dictionary<string, System.Text.Json.JsonElement> itemGroupJson) {
            ItemGroup returnItemGroupObject = new ItemGroup();
            string format = "yyyy-MM-dd HH:mm:ss";
            try {
                returnItemGroupObject = new ItemGroup {   
                    // GroupId = itemGroupJson["id"].GetInt32(),
                    Name = itemGroupJson["name"].GetString() ?? string.Empty,
                    
                    Description = itemGroupJson["description"].GetString() ?? string.Empty,

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
            } catch (Exception e) {
                Console.WriteLine($"GroupId: {itemGroupJson["id"].GetInt32()}\n {e}");
            }
            try {
                if (itemGroupJson["created_at"].ValueKind != JsonValueKind.Null)
                {
                    var createdAtString = itemGroupJson["created_at"].GetString();
                    if (!string.IsNullOrEmpty(createdAtString))
                    {
                        returnItemGroupObject.CreatedAt = DateTime.ParseExact(createdAtString, format, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                if (itemGroupJson["updated_at"].ValueKind != JsonValueKind.Null)
                {
                    var updatedAtString = itemGroupJson["updated_at"].GetString();
                    if (!string.IsNullOrEmpty(updatedAtString))
                    {
                        returnItemGroupObject.UpdatedAt = DateTime.ParseExact(updatedAtString, format, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            } catch (FormatException) {
                // Do nothing
            }

            if (string.IsNullOrEmpty(returnItemGroupObject.Name)) 
            {
                throw new ArgumentException("Name cannot be null or empty");
            }

            return returnItemGroupObject;
        }

        public ItemLine? ReturnItemLineObject(Dictionary<string, System.Text.Json.JsonElement> itemLineJson, Dictionary<int, List<int>> itemGroupRelations) {
            ItemLine returnItemLineObject = new ItemLine();
            int correspondingItemGroup = -1;
            Console.WriteLine($"ItemLine: {itemLineJson["id"].GetInt32()}\n corresponding Item Group: {correspondingItemGroup}");
            string format = "yyyy-MM-dd HH:mm:ss";
            foreach (KeyValuePair<int, List<int>> itemGroup in itemGroupRelations) {
                if(itemGroup.Value.Contains(itemLineJson["id"].GetInt32())) {
                    correspondingItemGroup = itemGroup.Key;
                    break;
                }
            }
            if (correspondingItemGroup == -1) return null;
                    Console.WriteLine($"ItemLine: {itemLineJson["id"].GetInt32()}\n corresponding Item Group: {correspondingItemGroup}");
            try {
                returnItemLineObject = new ItemLine {   
                    Name = itemLineJson["name"].GetString() ?? string.Empty,
                    ItemGroup = correspondingItemGroup,
                    Description = itemLineJson["description"].GetString() ?? string.Empty,

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
            } catch (Exception e) {
                Console.WriteLine($"GroupId: {itemLineJson["id"].GetInt32()}\n {e}");
            }
            try {
                if (itemLineJson["created_at"].ValueKind != JsonValueKind.Null)
                {
                    var createdAtString = itemLineJson["created_at"].GetString();
                    if (!string.IsNullOrEmpty(createdAtString))
                    {
                        returnItemLineObject.CreatedAt = DateTime.ParseExact(createdAtString, format, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                if (itemLineJson["updated_at"].ValueKind != JsonValueKind.Null)
                {
                    var updatedAtString = itemLineJson["updated_at"].GetString();
                    if (!string.IsNullOrEmpty(updatedAtString))
                    {
                        returnItemLineObject.UpdatedAt = DateTime.ParseExact(updatedAtString, format, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            } catch (FormatException) {
                // Do nothing
            }

            if (string.IsNullOrEmpty(returnItemLineObject.Name)) 
            {
                throw new ArgumentException("Name cannot be null or empty");
            }

            return returnItemLineObject;
        }

        public ItemType? ReturnItemTypeObject(Dictionary<string, System.Text.Json.JsonElement> itemTypeJson, Dictionary<int, List<int>> itemLineRelations) {
            ItemType returnItemTypeObject = new ItemType();
            int correspondingItemLine = -1;
            string format = "yyyy-MM-dd HH:mm:ss";
            foreach (KeyValuePair<int, List<int>> itemLine in itemLineRelations) {
                if(itemLine.Value.Contains(itemTypeJson["id"].GetInt32())) {
                    correspondingItemLine = itemLine.Key;
                    break;
                }
            }
            if (correspondingItemLine == -1) return null;
            try {
                returnItemTypeObject = new ItemType {   
                    Name = itemTypeJson["name"].GetString() ?? string.Empty,
                    ItemLine = correspondingItemLine,
                    Description = itemTypeJson["description"].GetString() ?? string.Empty,

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
            } catch (Exception e) {
                Console.WriteLine($"GroupId: {itemTypeJson["id"].GetInt32()}\n {e}");
            }
            try {
                if (itemTypeJson["created_at"].ValueKind != JsonValueKind.Null)
                {
                    var createdAtString = itemTypeJson["created_at"].GetString();
                    if (!string.IsNullOrEmpty(createdAtString))
                    {
                        returnItemTypeObject.CreatedAt = DateTime.ParseExact(createdAtString, format, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                if (itemTypeJson["updated_at"].ValueKind != JsonValueKind.Null)
                {
                    var updatedAtString = itemTypeJson["updated_at"].GetString();
                    if (!string.IsNullOrEmpty(updatedAtString))
                    {
                        returnItemTypeObject.UpdatedAt = DateTime.ParseExact(updatedAtString, format, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            } catch (FormatException) {
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
                    Code = supplierJson["code"].GetString() ?? string.Empty,
                    Name = supplierJson["name"].GetString() ?? string.Empty,
                    Address = supplierJson["address"].GetString() ?? string.Empty,
                    AddressExtra = supplierJson.ContainsKey("address_extra") && supplierJson["address_extra"].ValueKind != JsonValueKind.Null ? supplierJson["address_extra"].GetString() : string.Empty,
                    City = supplierJson["city"].GetString() ?? string.Empty,
                    ZipCode = supplierJson["zip_code"].GetString() ?? string.Empty,
                    Province = supplierJson["province"].GetString(),
                    Country = supplierJson["country"].GetString() ?? string.Empty,
                    ContactName = supplierJson["contact_name"].GetString() ?? string.Empty,
                    PhoneNumber = supplierJson["phonenumber"].GetString() ?? string.Empty,
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
                if (supplierJson["created_at"].ValueKind != JsonValueKind.Null)
                {
                    returnSupplierObject.CreatedAt = DateTime.ParseExact(
                        supplierJson["created_at"].GetString() ?? string.Empty,
                        format,
                        System.Globalization.CultureInfo.InvariantCulture
                    );
                }
                returnSupplierObject.UpdatedAt = DateTime.ParseExact(
                    supplierJson["updated_at"].GetString() ?? string.Empty,
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
                    Code = warehouseJson["code"].GetString() ?? string.Empty,
                    Name = warehouseJson["name"].GetString() ?? string.Empty,
                    Address = warehouseJson["address"].GetString() ?? string.Empty,
                    Zip = warehouseJson["zip"].GetString() ?? string.Empty, // Mapping the "zip" field from the JSON to "ZipCode" in Warehouse
                    City = warehouseJson["city"].GetString() ?? string.Empty,
                    Province = warehouseJson["province"].GetString() ?? string.Empty,
                    Country = warehouseJson["country"].GetString() ?? string.Empty,
                    
                    // Contact is a nested object, so we handle it separately
                    ContactName = warehouseJson.ContainsKey("contact") && warehouseJson["contact"].TryGetProperty("name", out var contactName) ? contactName.GetString() ?? string.Empty : string.Empty,
                    ContactPhone = warehouseJson.ContainsKey("contact") && warehouseJson["contact"].TryGetProperty("phone", out var contactPhone) ? contactPhone.GetString() ?? string.Empty : string.Empty,
                    ContactEmail = warehouseJson.ContainsKey("contact") && warehouseJson["contact"].TryGetProperty("email", out var contactEmail) ? contactEmail.GetString() ?? string.Empty : string.Empty,

                    // Date fields
                    CreatedAt = DateTime.ParseExact(warehouseJson["created_at"].GetString() ?? string.Empty, format, null),
                    UpdatedAt = DateTime.ParseExact(warehouseJson["updated_at"].GetString() ?? string.Empty, format, null),
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
                    warehouseJson["created_at"].GetString() ?? string.Empty,
                    format,
                    System.Globalization.CultureInfo.InvariantCulture
                );
                returnWarehouseObject.UpdatedAt = DateTime.ParseExact(
                    warehouseJson["updated_at"].GetString() ?? string.Empty,
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
                    Name = clientJson["name"].GetString() ?? string.Empty,
                    Address = clientJson["address"].GetString() ?? string.Empty,
                    ZipCode = clientJson["zip_code"].GetString() ?? string.Empty, // Mapping the "zip_code" field from the JSON to "ZipCode" in Client
                    City = clientJson["city"].GetString() ?? string.Empty,
                    Province = clientJson.ContainsKey("province") && !clientJson["province"].ValueKind.Equals(JsonValueKind.Null)
                        ? clientJson["province"].GetString() ?? "Unknown"
                        : "Unknown", // Default value if province is null or missing
                    Country = clientJson.ContainsKey("country") && clientJson["country"].ValueKind != JsonValueKind.Null ? clientJson["country"].GetString() ?? string.Empty : string.Empty,
                    
                    // Contact information mapping
                    ContactName = clientJson.ContainsKey("contact_name") && clientJson["contact_name"].ValueKind != JsonValueKind.Null ? clientJson["contact_name"].GetString() ?? string.Empty : string.Empty,
                    ContactPhone = clientJson.ContainsKey("contact_phone") && clientJson["contact_phone"].ValueKind != JsonValueKind.Null ? clientJson["contact_phone"].GetString() ?? string.Empty : string.Empty,
                    ContactEmail = clientJson.ContainsKey("contact_email") && clientJson["contact_email"].ValueKind != JsonValueKind.Null ? clientJson["contact_email"].GetString() ?? string.Empty : string.Empty,

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
                    clientJson["created_at"].GetString() ?? string.Empty,
                    format,
                    System.Globalization.CultureInfo.InvariantCulture
                );
                returnClientObject.UpdatedAt = DateTime.ParseExact(
                    clientJson["updated_at"].GetString() ?? string.Empty,
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

            try
            {
                returnItemObject = new Item
                {
                    // Mapping the JSON fields to Item properties
                    Uid = itemJson["uid"].GetString() ?? string.Empty,
                    Code = itemJson["code"].GetString() ?? string.Empty,
                    Description = itemJson["description"].GetString() ?? string.Empty,
                    ShortDescription = itemJson.ContainsKey("short_description") && itemJson["short_description"].ValueKind != JsonValueKind.Null ? itemJson["short_description"].GetString() ?? string.Empty : string.Empty,
                    UpcCode = itemJson.ContainsKey("upc_code") && itemJson["upc_code"].ValueKind != JsonValueKind.Null ? itemJson["upc_code"].GetString() ?? string.Empty : string.Empty,
                    ModelNumber = itemJson.ContainsKey("model_number") && itemJson["model_number"].ValueKind != JsonValueKind.Null ? itemJson["model_number"].GetString() ?? string.Empty : string.Empty,
                    CommodityCode = itemJson.ContainsKey("commodity_code") && itemJson["commodity_code"].ValueKind != JsonValueKind.Null ? itemJson["commodity_code"].GetString() ?? string.Empty : string.Empty,
                    ItemLine = itemJson.ContainsKey("item_line") && itemJson["item_line"].ValueKind != JsonValueKind.Null ? itemJson["item_line"].GetInt32() : 0,
                    ItemGroup = itemJson.ContainsKey("item_group") && itemJson["item_group"].ValueKind != JsonValueKind.Null ? itemJson["item_group"].GetInt32() : 0,
                    ItemType = itemJson.ContainsKey("item_type") && itemJson["item_type"].ValueKind != JsonValueKind.Null ? itemJson["item_type"].GetInt32() : 0,
                    UnitPurchaseQuantity = itemJson.ContainsKey("unit_purchase_quantity") && itemJson["unit_purchase_quantity"].ValueKind != JsonValueKind.Null ? itemJson["unit_purchase_quantity"].GetInt32() : 0,
                    UnitOrderQuantity = itemJson.ContainsKey("unit_order_quantity") && itemJson["unit_order_quantity"].ValueKind != JsonValueKind.Null ? itemJson["unit_order_quantity"].GetInt32() : 0,
                    PackOrderQuantity = itemJson["pack_order_quantity"].GetInt32(),
                    SupplierId = itemJson.ContainsKey("supplier_id") && itemJson["supplier_id"].ValueKind != JsonValueKind.Null ? itemJson["supplier_id"].GetInt32() : 0,
                    SupplierCode = itemJson.ContainsKey("supplier_code") && itemJson["supplier_code"].ValueKind != JsonValueKind.Null ? itemJson["supplier_code"].GetString() ?? string.Empty : string.Empty,
                    SupplierPartNumber = itemJson.ContainsKey("supplier_part_number") && itemJson["supplier_part_number"].ValueKind != JsonValueKind.Null ? itemJson["supplier_part_number"].GetString() ?? string.Empty : string.Empty,
                    CreatedAt = DateTime.UtcNow, // Default value; will be overridden below
                    UpdatedAt = DateTime.UtcNow, // Default value; will be overridden below
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing Item UID: {itemJson["uid"].GetString()}\n {e}");
            }

            // Parse created_at and updated_at with the specific format
            try {
                returnItemObject.CreatedAt = DateTime.ParseExact(itemJson["created_at"].GetString() ?? string.Empty, format, System.Globalization.CultureInfo.InvariantCulture);
                returnItemObject.UpdatedAt = DateTime.ParseExact(itemJson["updated_at"].GetString() ?? string.Empty, format, System.Globalization.CultureInfo.InvariantCulture);
            } catch (FormatException e)
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

        public Transfer? ReturnTransferObject(Dictionary<string, System.Text.Json.JsonElement> transferJson)
        {
            Transfer returnTransferObject = new Transfer();
            string format = "yyyy-MM-ddTHH:mm:ssZ"; // Define the expected date-time format for the JSON (ISO 8601)

            try
            {
                // Check if TransferFrom and TransferTo are null or missing, and return null if so
                if (!transferJson.ContainsKey("transfer_from") || !transferJson.ContainsKey("transfer_to") ||
                    transferJson["transfer_from"].ValueKind.Equals(JsonValueKind.Null) ||
                    transferJson["transfer_to"].ValueKind.Equals(JsonValueKind.Null))
                {
                    return null; // Return null if either transfer_from or transfer_to is missing or null
                }

                returnTransferObject = new Transfer
                {
                    // Mapping the JSON fields to Transfer properties
                    TransferId = transferJson["id"].GetInt32(),
                    Reference = transferJson["reference"].GetString() ?? string.Empty,
                    TransferFrom = transferJson["transfer_from"].GetInt32(),
                    TransferTo = transferJson["transfer_to"].GetInt32(),
                    TransferStatus = transferJson["transfer_status"].GetString() ?? string.Empty,
                    
                    // Date fields
                    CreatedAt = DateTime.UtcNow, // Default value; will be overridden below
                    UpdatedAt = DateTime.UtcNow // Default value; will be overridden below
                };
            }
            catch (Exception e)
            {
                // If an error occurs while processing the transfer object, log it
                Console.WriteLine($"Error processing Transfer ID: {transferJson["id"].GetInt32()}\n {e}");
            }

            try {
                returnTransferObject.CreatedAt = DateTime.ParseExact(transferJson["created_at"].GetString() ?? string.Empty, format, System.Globalization.CultureInfo.InvariantCulture);
                returnTransferObject.UpdatedAt = DateTime.ParseExact(transferJson["updated_at"].GetString() ?? string.Empty, format, System.Globalization.CultureInfo.InvariantCulture);
            } catch (FormatException e)
            {
                Console.WriteLine($"Date parsing error for Transfer ID: {returnTransferObject.TransferId}\n {e}");
            }
            

            return returnTransferObject;
        }

        public (Order orderObj, List<OrderItem> orderItems) ReturnOrderObject(Dictionary<string, System.Text.Json.JsonElement> orderJson)
        {
            (Order orderObj, List<OrderItem> orderItems) order;
            Order returnOrderObject = new Order();
            List<OrderItem> orderItems = new List<OrderItem>();
            try
            {
                returnOrderObject = new Order
                {
                    // Mapping the JSON fields to Order properties
                    Id = orderJson["id"].GetInt32(),
                    SourceId = orderJson["source_id"].ValueKind == JsonValueKind.Null ? 0 : orderJson["source_id"].GetInt32(),
                    Reference = orderJson["reference"].GetString() ?? string.Empty,
                    ReferenceExtra = orderJson["reference_extra"].GetString() ?? string.Empty,
                    OrderStatus = orderJson["order_status"].GetString() ?? string.Empty,
                    Notes = orderJson["notes"].GetString() ?? string.Empty,
                    ShippingNotes = orderJson["shipping_notes"].GetString() ?? string.Empty,
                    PickingNotes = orderJson["picking_notes"].GetString() ?? string.Empty,
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
                    UpdatedAt = DateTime.UtcNow // Default value; will be overridden below
                };
                            // Parse created_at and updated_at with the specific format
                // try {
                //     returnOrderObject.CreatedAt = DateTime.ParseExact(orderJson["created_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
                //     returnOrderObject.UpdatedAt = DateTime.ParseExact(orderJson["updated_at"].GetString(), format, System.Globalization.CultureInfo.InvariantCulture);
                // } catch (FormatException e)
                // {
                //     Console.WriteLine($"Date parsing error for Order UID: {returnOrderObject.Id}\n {e}");
                // }
                // Parse the 'items' array and map it to the OrderItems list
                if (orderJson.ContainsKey("items") && orderJson["items"].ValueKind == JsonValueKind.Array)
                {
                    foreach (var itemJson in orderJson["items"].EnumerateArray())
                    {
                        // Assuming each item in the array is an object with properties "item_id" and "amount"
                        var orderItem = new OrderItem
                        {
                            // Access item_id and amount properties
                            ItemId = itemJson.TryGetProperty("item_id", out var itemId) ? itemId.GetString() ?? string.Empty : string.Empty,
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
                    ItemId = inventoryJson["item_id"].GetString() ?? string.Empty,
                    Description = inventoryJson.ContainsKey("description") && !inventoryJson["description"].ValueKind.Equals(JsonValueKind.Null)
                        ? inventoryJson["description"].GetString() ?? string.Empty
                        : "No description available",  // Default value if description is missing or null
                    ItemReference = inventoryJson["item_reference"].GetString() ?? string.Empty,
                    
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
            try {
                returnInventoryObject.CreatedAt = DateTime.ParseExact(inventoryJson["created_at"].GetString() ?? string.Empty, format, System.Globalization.CultureInfo.InvariantCulture);
                returnInventoryObject.UpdatedAt = DateTime.ParseExact(inventoryJson["updated_at"].GetString() ?? string.Empty, format, System.Globalization.CultureInfo.InvariantCulture);
            } catch (FormatException e)
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
                    OrderId = shipmentJson["order_id"].GetInt32(),
                    SourceId = shipmentJson["source_id"].GetInt32(),
                    OrderDate = DateTime.Parse(shipmentJson["order_date"].GetString() ?? string.Empty),
                    RequestDate = DateTime.Parse(shipmentJson["request_date"].GetString() ?? string.Empty),
                    ShipmentDate = shipmentJson["shipment_date"].ValueKind != JsonValueKind.Null ? DateTime.Parse(shipmentJson["shipment_date"].GetString() ?? string.Empty) : DateTime.MinValue,
                    ShipmentType = shipmentJson["shipment_type"].GetString() ?? string.Empty,
                    ShipmentStatus = shipmentJson["shipment_status"].GetString() ?? string.Empty,
                    Notes = shipmentJson["notes"].GetString() ?? string.Empty,
                    CarrierCode = shipmentJson["carrier_code"].GetString() ?? string.Empty,
                    CarrierDescription = shipmentJson["carrier_description"].GetString() ?? string.Empty,
                    ServiceCode = shipmentJson["service_code"].GetString() ?? string.Empty,
                    PaymentType = shipmentJson["payment_type"].GetString() ?? string.Empty,
                    TransferMode = shipmentJson["transfer_mode"].GetString() ?? string.Empty,
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
                    shipmentJson["created_at"].GetString() ?? string.Empty,
                    dateTimeFormat,
                    System.Globalization.CultureInfo.InvariantCulture
                );
                returnShipmentObject.UpdatedAt = DateTime.ParseExact(
                    shipmentJson["updated_at"].GetString() ?? string.Empty,
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
                            ItemId = itemElement.GetProperty("item_id").GetString() ?? string.Empty,
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
                    ItemAmountsString = locationJson.ContainsKey("item_amounts_string") && locationJson["item_amounts_string"].ValueKind != JsonValueKind.Null ? locationJson["item_amounts_string"].GetString() : string.Empty,
                    Code = locationJson["code"].GetString() ?? string.Empty,
                    Name = locationJson["name"].GetString() ?? string.Empty,

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
                    locationJson["created_at"].GetString() ?? string.Empty,
                    format,
                    System.Globalization.CultureInfo.InvariantCulture
                );
                returnLocationObject.UpdatedAt = DateTime.ParseExact(
                    locationJson["updated_at"].GetString() ?? string.Empty,
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

