using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class SupplierService : ISupplierService
    {
        private readonly CargoHubDbContext _context;

        public SupplierService(CargoHubDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            if (supplier.Code == null)
                return null;

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<bool> UpdateSupplierAsync(int id, Supplier supplier)
        {
            var existingSupplier = await _context.Suppliers.FindAsync(id);
            if (existingSupplier == null)
                return false;

            existingSupplier.Code = supplier.Code;
            existingSupplier.Name = supplier.Name;
            existingSupplier.Address = supplier.Address;
            existingSupplier.AddressExtra = supplier.AddressExtra;
            existingSupplier.City = supplier.City;
            existingSupplier.ZipCode = supplier.ZipCode;
            existingSupplier.Province = supplier.Province;
            existingSupplier.Country = supplier.Country;
            existingSupplier.ContactName = supplier.ContactName;
            existingSupplier.PhoneNumber = supplier.PhoneNumber;
            existingSupplier.Reference = supplier.Reference;
            existingSupplier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return false;

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAllSuppliersAsync()
        {
            var suppliers = _context.Suppliers.ToList();
            if (suppliers.Count == 0)
                return false;

            _context.Suppliers.RemoveRange(suppliers);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetLowestAvailableSupplierIdAsync()
        {
            // Get all used IDs
            var usedIds = await _context.Suppliers
                                         .Select(s => s.SupplierId)
                                         .OrderBy(id => id)
                                         .ToListAsync();

            int lowestId = 1;
            foreach (var id in usedIds)
            {
                if (id == lowestId)
                    lowestId++;
                else
                    break;
            }

            return lowestId;
        }

    }
}