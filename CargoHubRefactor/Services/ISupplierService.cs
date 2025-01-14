using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync(int limit);
        Task<IEnumerable<Supplier>> GetSuppliersPagedAsync(int limit, int page);
        Task<Supplier> GetSupplierByIdAsync(int id);
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task<bool> UpdateSupplierAsync(int id, Supplier supplier);
        Task<bool> DeleteSupplierAsync(int id);
        Task<bool> DeleteAllSuppliersAsync();
        Task<int> GetLowestAvailableSupplierIdAsync();

    }
}
