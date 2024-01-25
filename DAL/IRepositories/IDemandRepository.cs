using CarpooliDotTN.Models;

namespace CarpooliDotTN.DAL.IRepositories
{
    public interface IDemandRepository
    {
        Task<Demand> GetByIdAsync(Guid id);
        Task<List<Demand>> GetAllAsync();
        Task<List<Demand>> GetDemandsByCarpoolAsync(Guid carpoolId);
        Task<List<Demand>> GetDemandsBySenderAsync(string senderId);
        Task AddAsync(Demand demand);
        Task UpdateAsync(Demand demand);
        Task DeleteAsync(Guid id);
        Task<Demand> GetDemandByPassengerAndCarpoolAsync(string passengerId, Guid carpoolId);
        bool IsApplied(string userId, Guid carpoolId);

    }

}
