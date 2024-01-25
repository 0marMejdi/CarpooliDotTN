using CarpooliDotTN.Models;

namespace CarpooliDotTN.DAL.IServices
{
    public interface IDemandService
    {
        Task<Demand> GetDemandByIdAsync(Guid id);
        Task<List<Demand>> GetAllDemandsAsync();
        Task<List<Demand>> GetDemandsByCarpoolAsync(Guid carpoolId);
        Task<List<Demand>> GetDemandsBySenderAsync(string senderId);
        Task AddDemandAsync(Demand demand);
        Task UpdateDemandAsync(Demand demand);
        Task DeleteDemandAsync(Guid id);
        Task RemoveDemandAsync(string v, Guid id);
        bool IsApplied(string userId, Guid carpoolId);

    }
}
