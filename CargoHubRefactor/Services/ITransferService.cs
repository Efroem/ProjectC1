using System.Collections.Generic;
using System.Threading.Tasks;

public interface ITransferService
{
    Task<(string message, Transfer? transfer)> AddTransferAsync(Transfer transfer);
    Task<string> UpdateTransferStatusAsync(int transferId, string status);
    Task<(string message, Transfer? transfer)> UpdateTransferAsync(int transferId, Transfer updatedTransfer); // New method
    Task<string> DeleteTransferAsync(int transferId);
    Task<List<Transfer>> GetAllTransfersAsync();
    Task<Transfer?> GetTransferByIdAsync(int transferId);
}
