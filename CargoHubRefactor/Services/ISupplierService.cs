using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier> GetSupplierByIdAsync(int id);
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task<bool> UpdateSupplierAsync(int id, Supplier supplier);
        Task<bool> DeleteSupplierAsync(int id);
        Task<bool> SoftDeleteSupplierAsync(int id);
        Task<bool> DeleteAllSuppliersAsync();
        Task<int> GetLowestAvailableSupplierIdAsync();

    }
}
