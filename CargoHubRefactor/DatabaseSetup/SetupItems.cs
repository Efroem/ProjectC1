using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Testing.Platform.Extensions.Messages;
namespace CargoHubRefactor.DbSetup {
    public class SetupItems
    {
        private readonly CargoHubDbContext _context;
        private readonly ResourceObjectReturns objectReturns = new ResourceObjectReturns();
        private readonly string logFilePath = "transfer_log.txt";
        private Dictionary<int, Dictionary<string, int>> ItemAmountLocations = new Dictionary<int, Dictionary<string, int>>();
        public SetupItems(CargoHubDbContext context)
        {
            _context = context;
        }
        private void LogMessage(string message)
        {
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
        public async Task GetItemCategoryRelations()
        {
            List<Dictionary<int, List<int>>> ItemRelationsLists = new List<Dictionary<int, List<int>>>();
            Dictionary<int, List<int>> ItemLineRelations = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> ItemGroupRelations = new Dictionary<int, List<int>>();
            
            // Correct path to the data file
            string dataFilePath = "../CargoHub/data/";
            string itemDataString = File.ReadAllText($"{dataFilePath}items.json");
            string itemGroupDataString = File.ReadAllText($"{dataFilePath}item_groups.json");
            string itemLineDataString = File.ReadAllText($"{dataFilePath}item_lines.json");
            string itemTypeDataString = File.ReadAllText($"{dataFilePath}item_types.json");
            string supplierDataString = File.ReadAllText($"{dataFilePath}suppliers.json");
            string warehouseDataString = File.ReadAllText($"{dataFilePath}warehouses.json");
            string clientDataString = File.ReadAllText($"{dataFilePath}clients.json");
            string transferDataString = File.ReadAllText($"{dataFilePath}transfers.json");
            string orderDataString = File.ReadAllText($"{dataFilePath}orders.json");
            string inventoryDataString = File.ReadAllText($"{dataFilePath}inventories.json");
            string shipmentDataString = File.ReadAllText($"{dataFilePath}shipments.json");
            string locationDataString = File.ReadAllText($"{dataFilePath}locations.json");

            // Deserialize the JSON string into a List of Dictionaries with JsonElement values
            List<Dictionary<string, JsonElement>> itemData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(itemDataString);
            List<Dictionary<string, JsonElement>> itemGroupData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(itemGroupDataString);
            List<Dictionary<string, JsonElement>> itemLineData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(itemLineDataString);
            List<Dictionary<string, JsonElement>> itemTypeData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(itemTypeDataString);
            List<Dictionary<string, JsonElement>> supplierData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(supplierDataString);
            List<Dictionary<string, JsonElement>> warehouseData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(warehouseDataString);
            List<Dictionary<string, JsonElement>> clientData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(clientDataString);
            List<Dictionary<string, JsonElement>> transferData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(transferDataString);
            List<Dictionary<string, JsonElement>> orderData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(orderDataString);
            List<Dictionary<string, JsonElement>> inventoryData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(inventoryDataString);
            List<Dictionary<string, JsonElement>> shipmentData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(shipmentDataString);
            List<Dictionary<string, JsonElement>> locationData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(locationDataString);
            foreach (var item in itemData)
            {
                try
                {
                    // Ensure the correct conversion from JsonElement to int using GetInt32
                    int itemType = item["item_type"].GetInt32();  // GetInt32() method to safely convert
                    int itemLine = item["item_line"].GetInt32();  // GetInt32() method to safely convert
                    int itemGroup = item["item_group"].GetInt32();  // GetInt32() method to safely convert

                    // Handling ItemTypeRelations
                    if (!ItemGroupRelations.ContainsKey(itemGroup))
                    {
                        ItemGroupRelations.Add(itemGroup, new List<int> { itemLine });
                    }
                    else
                    {
                        ItemGroupRelations[itemGroup].Add(itemLine);
                    }

                    // Handling ItemLineRelations
                    if (!ItemLineRelations.ContainsKey(itemLine))
                    {
                        ItemLineRelations.Add(itemLine, new List<int> { itemType });
                    }
                    else
                    {
                        ItemLineRelations[itemLine].Add(itemType);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Error converting JSON element: {ex.Message}");
                }
            }

            // _context.Database.EnsureDeleted();
            // _context.Database.EnsureCreated();
            
            // Load in Item Group
            foreach (var itemGroupJsonObject in itemGroupData) {
                ItemGroup itemGroup = objectReturns.ReturnItemGroupObject(itemGroupJsonObject);
                if (_context.ItemGroups.Any(x => x.GroupId == itemGroupJsonObject["id"].GetInt32())) {
                    break;
                }
                // PrintAllValues(itemGroup);
                try{
                    await _context.ItemGroups.AddAsync(itemGroup);
                    Console.WriteLine(itemGroup.Name != null ? itemGroup.Name : "null");
                    // Console.WriteLine(_context.ItemGroups.FirstOrDefault(x => x.GroupId == itemGroup.GroupId));
                } catch (Exception ex) {
                    // Console.WriteLine(ex);
                }
                
            }
            await _context.SaveChangesAsync();

            // Load in Item Line
            foreach (var itemLineJsonObject in itemLineData) {
                ItemLine itemLine = objectReturns.ReturnItemLineObject(itemLineJsonObject, ItemGroupRelations);
                if (_context.ItemLines.Any(x => x.LineId == itemLineJsonObject["id"].GetInt32())) {
                    break;
                }
                if (itemLine == null) continue;
                try{
                    await _context.ItemLines.AddAsync(itemLine);
                    // Console.WriteLine(_context.ItemGroups.FirstOrDefault(x => x.GroupId == itemGroup.GroupId));
                } catch (Exception ex) {
                    // Console.WriteLine(ex);
                }
                // if (itemLineJsonObject["id"] == 20) break;
                
            }
            await _context.SaveChangesAsync();

            // Load in Item Type
            foreach (var itemTypeJsonObject in itemTypeData) {
                ItemType itemType = objectReturns.ReturnItemTypeObject(itemTypeJsonObject, ItemLineRelations);
                if (_context.ItemTypes.Any(x => x.TypeId == itemTypeJsonObject["id"].GetInt32())) {
                    break;
                }
                if (itemType == null) continue;
                PrintAllValues(itemType);
                try{
                    await _context.ItemTypes.AddAsync(itemType);
                    Console.WriteLine(itemType.Name != null ? itemType.Name : "null");
                    
                    // Console.WriteLine(_context.ItemGroups.FirstOrDefault(x => x.GroupId == itemGroup.GroupId));
                } catch (Exception ex) {
                    // Console.WriteLine(ex);
                }
                // if (itemTypeJsonObject["id"] == 20) break;
                
            }
            await _context.SaveChangesAsync();

            foreach (var supplierJsonObject in supplierData) {
                if (_context.Suppliers.Any(x => x.SupplierId == supplierJsonObject["id"].GetInt32())) {
                    break;
                }
                Supplier supplier = objectReturns.ReturnSupplierObject(supplierJsonObject);
                if (supplier == null) continue;
                // PrintAllValues(supplier);
                try{
                    await _context.Suppliers.AddAsync(supplier);
                    Console.WriteLine(supplier.Name != null ? supplier.Name : "null");
                } catch (Exception ex) {
                    // Console.WriteLine(ex);
                }
    
            }
            await _context.SaveChangesAsync();

            List<Warehouse> Warehouses = new List<Warehouse>();
            foreach (var warehouseJsonObject in warehouseData) {
                if (_context.Warehouses.Any(x => x.WarehouseId == warehouseJsonObject["id"].GetInt32())) {
                    break;
                }
                Warehouse warehouse = objectReturns.ReturnWarehouseObject(warehouseJsonObject);
                if (warehouse == null) continue;
                // PrintAllValues(supplier);
                try{
                   
                    await _context.Warehouses.AddAsync(warehouse);
                    Warehouses.Add(warehouse);

                } catch (Exception ex) {
                    // Console.WriteLine(ex);
                }

            }
            await _context.SaveChangesAsync();
            
            foreach (var clientJsonObject in clientData) {
                if (_context.Clients.Any(x => x.ClientId == clientJsonObject["id"].GetInt32())) {
                    break;
                }
                Client client = objectReturns.ReturnClientObject(clientJsonObject);
                if (client == null) continue;
                try{
                    await _context.Clients.AddAsync(client);

                } catch (Exception ex) {
                    PrintAllValues(client);
                    Console.WriteLine(ex);
                }

            }
            await _context.SaveChangesAsync();


            foreach (var itemJsonObject in itemData) {
                var itemLineExists = _context.ItemLines.Any(x => x.LineId == itemJsonObject["item_line"].GetInt32());
                var itemGroupExists = _context.ItemGroups.Any(x => x.GroupId == itemJsonObject["item_group"].GetInt32());
                var itemTypeExists = _context.ItemTypes.Any(x => x.TypeId == itemJsonObject["item_type"].GetInt32());
                var supplierExists = _context.Suppliers.Any(x => x.SupplierId == itemJsonObject["supplier_id"].GetInt32());

                if (!itemLineExists || !itemGroupExists || !itemTypeExists || !supplierExists) 
                {
                    Console.WriteLine("One or more foreign key constraints are invalid.");
                    continue;
                }
                if (_context.Items.Any(x => x.Uid == itemJsonObject["uid"].GetString())) {
                    break;
                }
                Item item = objectReturns.ReturnItemObject(itemJsonObject);
                if (item == null) continue;
                try{
                    await _context.Items.AddAsync(item);
                    // Console.WriteLine(supplier.Name != null ? supplier.Name : "null");


                } catch (Exception ex) {
                    PrintAllValues(item);
                    Console.WriteLine(ex);
                }
    
            }

            await _context.SaveChangesAsync();


            foreach (var inventoryJsonObject in inventoryData) {
                var itemExists = _context.Items.Any(x => x.Uid == inventoryJsonObject["item_id"].GetString());
                var inventoryExists = _context.Inventories.Any(x => x.InventoryId == inventoryJsonObject["id"].GetInt32());
                if (inventoryExists) {
                    break;
                }
                if (!itemExists) {
                    continue;
                }

                Inventory inventory = objectReturns.ReturnInventoryObject(inventoryJsonObject);

                if (inventory == null) continue;
                // PrintAllValues(supplier);
                try{
                    await _context.Inventories.AddAsync(inventory);
                    int amountPerLocation = inventory.TotalOnHand / inventory.LocationsList.Count;
                    int remainder = inventory.TotalOnHand % inventory.LocationsList.Count;
                    
                    for (int i = 0; i < inventory.LocationsList.Count; i++) {
                        int locationId = inventory.LocationsList[i];
                        if (!ItemAmountLocations.ContainsKey(locationId))
                        {
                            ItemAmountLocations[locationId] = new Dictionary<string, int>();
                        }

                        if (ItemAmountLocations[locationId].ContainsKey(inventory.ItemId))
                        {
                            ItemAmountLocations[locationId][inventory.ItemId] += amountPerLocation + (remainder > 0 ? remainder : 0);
                        }
                        else
                        {
                            ItemAmountLocations[locationId].Add(inventory.ItemId, amountPerLocation + (remainder > 0 ? remainder : 0));
                        }
                    }
                } catch (Exception ex) {
                    PrintAllValues(inventory);
                    Console.WriteLine(ex);
                }
    
            }
            await _context.SaveChangesAsync();

            foreach (var shipmentJsonObject in shipmentData) {
                if (_context.Shipments.Any(x => x.ShipmentId == shipmentJsonObject["id"].GetInt32())) {
                    break;
                }
                Boolean leaveCode = false;
                (Shipment shipmentObj, List<ShipmentItem> shipmentItems) shipment = objectReturns.ReturnShipmentObject(shipmentJsonObject);
                if (shipment.shipmentObj == null || shipment.shipmentItems.Count == 0) continue;
                try{
                    if (!_context.Shipments.Any(x => x.ShipmentId == shipmentJsonObject["id"].GetInt32())) {
                        await _context.Shipments.AddAsync(shipment.shipmentObj);
                    }
                    foreach (var item in shipment.shipmentItems) {
                        try {
                            if (_context.ShipmentItems.Any(x => x.ItemId == item.ItemId && x.Amount == item.Amount)) break;  
                            else if (!_context.Items.Any(x => x.Uid == item.ItemId)) continue;

                            await _context.ShipmentItems.AddAsync(item);
                        } catch (Exception ex) {
                            PrintAllValues(item);
                            Console.WriteLine(ex);
                            leaveCode = true;
                            break;
                        }

                    }

                } catch (Exception ex) {
                    PrintAllValues(shipment);
                    Console.WriteLine(ex);
                }
                if (leaveCode) break;

            }
            await _context.SaveChangesAsync();

            foreach (var locationJsonObject in locationData) {
                if (_context.Locations.Any(x => x.LocationId == locationJsonObject["id"].GetInt32())) {
                    break;
                }
                Location location = objectReturns.ReturnLocationObject(locationJsonObject);
                if (location == null) continue;
                try{
                    if (ItemAmountLocations.ContainsKey(location.LocationId)) {
                        location.ItemAmountsString = JsonSerializer.Serialize(ItemAmountLocations[location.LocationId]);
                    }

                    await _context.Locations.AddAsync(location);

                } catch (Exception ex) {
                    PrintAllValues(location);
                    Console.WriteLine(ex);
                }

            }

            await _context.SaveChangesAsync();

            int NextLocationId = _context.Locations.Max(l => l.LocationId) + 1;


            // ADD WAREHOUSE DOCKS TO LOCATIONS
            if (Warehouses.Count > 0) {
                foreach (Warehouse warehouse in Warehouses) {
                    NextLocationId++;
                    // await File.AppendAllTextAsync("log.txt", $"NextId: {NextLocationId}\n");
                    Location location = new Location() {
                        LocationId = NextLocationId,
                        WarehouseId = warehouse.WarehouseId,
                        Code = $"D1WH{String.Concat(Enumerable.Repeat("0", 3 - warehouse.WarehouseId.ToString().Count()))}{warehouse.WarehouseId}",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Name = $"Dock 1 Warehouse{String.Concat(Enumerable.Repeat("0", 3 - warehouse.WarehouseId.ToString().Count()))}{warehouse.WarehouseId}",
                        ItemAmounts = new Dictionary<string, int>(),
                        ItemAmountsString = {},
                        IsDock = true
                    };
                    try {
                        await _context.Locations.AddAsync(location);
                    } catch (Exception ex) {
                        PrintAllValues(location);
                        Console.WriteLine(ex);
                    } 
                }
                await _context.SaveChangesAsync();
            }
            


            List<int> existingOrder = new List<int>();
            foreach (var orderJsonObject in orderData) {
                if (_context.Orders.Any(x => x.Id == orderJsonObject["id"].GetInt32())) {
                    break;
                }
                Boolean leaveCode = false;
                (Order orderObj, List<OrderItem> orderItems) order = objectReturns.ReturnOrderObject(orderJsonObject);
                bool OrderExists = await _context.Orders.AnyAsync(x => x.Id == orderJsonObject["id"].GetInt32());
                if (order.orderObj == null || order.orderItems.Count == 0) continue;
                try{
                    if (!OrderExists && !existingOrder.Contains(order.orderObj.Id)) {
                        await _context.Orders.AddAsync(order.orderObj);
                        existingOrder.Add(order.orderObj.Id);
                        foreach (var item in order.orderItems) {
                            try {
                                // Check if ItemId exists in Items table
                                if (!_context.Items.Any(x => x.Uid == item.ItemId)) {
                                    Console.WriteLine($"Item with Uid {item.ItemId} does not exist. Skipping.");
                                    continue; // Skip this item
                                }

                                // Check if the combination of ItemId and Amount already exists in OrderItems
                                if (_context.OrderItems.Any(x => x.ItemId == item.ItemId && x.Amount == item.Amount)) {
                                    Console.WriteLine($"Duplicate OrderItem found for ItemId {item.ItemId} and Amount {item.Amount}. Skipping.");
                                    continue; // Skip duplicate entries
                                }

                                // Assign the valid OrderId before inserting
                                item.OrderId = order.orderObj.Id; 

                                // Add to context (but don’t save immediately)
                                await _context.OrderItems.AddAsync(item);

                            } catch (Exception ex) {
                                Console.WriteLine($"Error processing item with Id {item.ItemId}: {ex.Message}");
                                leaveCode = true;
                                break;
                            }
                        }

                    }
                } catch (Exception ex) {
                    PrintAllValues(order.orderObj);
                    Console.WriteLine(ex);
                    break;
                }
                if (leaveCode) break;

            }
            await _context.SaveChangesAsync();

            var transfersToAdd = new List<Transfer>();
            var transferItemsToAdd = new List<TransferItem>();

            foreach (var transferJson in transferData)
            {
                try
                {
                    // Check if Database already has data and if so, skip filling the database
                    if (_context.Transfers.Any(x => x.TransferId == transferJson["id"].GetInt32())) {
                        break;
                    }
                    // Convert JSON to Transfer object
                    Transfer transfer = objectReturns.ReturnTransferObject(transferJson);

                    // Validate Transfer object
                    if (transfer == null)
                    {
                        LogMessage($"Transfer object creation failed. Transfer JSON: {JsonSerializer.Serialize(transferJson)}");
                        continue;
                    }

                    if (transfer.TransferId == 0)
                    {
                        LogMessage($"Transfer has an invalid or null ID. Transfer JSON: {JsonSerializer.Serialize(transferJson)}");
                        continue;
                    }

                    if (_context.Transfers.Any(x => x.TransferId == transfer.TransferId))
                    {
                        LogMessage($"Transfer with ID {transfer.TransferId} already exists. Skipping.");
                        continue;
                    }

                    // Add Transfer to context
                    await _context.Transfers.AddAsync(transfer);
                    LogMessage($"Added Transfer with ID {transfer.TransferId}");

                    // Process and add TransferItems
                    if (transferJson.ContainsKey("items") && transferJson["items"].ValueKind == JsonValueKind.Array)
                    {
                        foreach (var itemJson in transferJson["items"].EnumerateArray())
                        {
                            try
                            {
                                string itemId = itemJson.GetProperty("item_id").GetString();
                                if (!_context.Items.Any(i => i.Uid == itemId))
                                {
                                    LogMessage($"Item with ID {itemId} does not exist. Skipping TransferItem for TransferId: {transfer.TransferId}");
                                    continue;
                                }

                                TransferItem transferItem = objectReturns.ReturnTransferItemObject(itemJson, transfer.TransferId);

                                if (transferItem == null || _context.TransferItems.Any(x => x.TransferItemId == transferItem.TransferItemId))
                                    continue;

                                await _context.TransferItems.AddAsync(transferItem);
                            }
                            catch (Exception itemEx)
                            {
                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Error processing transfer: {ex.Message}\nTransfer JSON: {JsonSerializer.Serialize(transferJson)}");
                }
            }

            // Save all transfers and transfer items at once
            try
            {
                await _context.Transfers.AddRangeAsync(transfersToAdd);
                await _context.TransferItems.AddRangeAsync(transferItemsToAdd);
                await _context.SaveChangesAsync();
                LogMessage($"Successfully saved {transfersToAdd.Count} Transfers and {transferItemsToAdd.Count} TransferItems.");
            }
            catch (Exception saveEx)
            {
                LogMessage($"Error saving transfers or transfer items: {saveEx.Message}");
            }


            return;
        }

        

        public static void PrintAllValues(object obj)
        {
            if (obj == null)
            {
                Console.WriteLine("Object is null.");
                return;
            }

            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (var property in properties)
            {
                object value = property.GetValue(obj);
                Console.WriteLine($"{property.Name}: {value}");
            }
        }
    }
}