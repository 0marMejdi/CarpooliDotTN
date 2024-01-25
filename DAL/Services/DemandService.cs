using CarpooliDotTN.DAL.IRepositories;
using CarpooliDotTN.DAL.IServices;
using CarpooliDotTN.Models;

namespace CarpooliDotTN.DAL.Services
{
    public class DemandService : IDemandService
    {
        private readonly IDemandRepository _demandRepository;

        public DemandService(IDemandRepository demandRepository)
        {
            _demandRepository = demandRepository;
        }

        public async Task<Demand> GetDemandByIdAsync(Guid id)
        {
            return await _demandRepository.GetByIdAsync(id);
        }

        public async Task<List<Demand>> GetAllDemandsAsync()
        {
            return await _demandRepository.GetAllAsync();
        }

        public async Task<List<Demand>> GetDemandsByCarpoolAsync(Guid carpoolId)
        {
            return await _demandRepository.GetDemandsByCarpoolAsync(carpoolId);
        }

        public async Task<List<Demand>> GetDemandsBySenderAsync(string senderId)
        {
            return await _demandRepository.GetDemandsBySenderAsync(senderId);
        }

        public async Task AddDemandAsync(Demand demand)
        {
            // Additional business logic if needed before adding the demand
            await _demandRepository.AddAsync(demand);
        }

        public async Task UpdateDemandAsync(Demand demand)
        {
            // Additional business logic if needed before updating the demand
            await _demandRepository.UpdateAsync(demand);
        }

        public async Task DeleteDemandAsync(Guid id)
        {
            _demandRepository.DeleteAsync(id);
        }

        public async Task RemoveDemandAsync(string v, Guid id)
        {
            var demand = await _demandRepository.GetDemandByPassengerAndCarpoolAsync(v, id);
            if (demand != null)
            {
                await _demandRepository.DeleteAsync(demand.Id);
            }
            else
            {
                throw new InvalidOperationException("Demand not found or already removed.");
            }
        }
        public bool IsApplied(string userId, Guid carpoolId)
        {
            // Assuming you have a method in the repository that checks if the user applied to the carpool
            return _demandRepository.IsApplied(userId, carpoolId);
        }
        public async Task<List<Demand>> GetDemandsByOwnerIdAsync(string userId)
        {
            return await _demandRepository.GetDemandsByOwnerIdAsync(userId);
        }
    }
}
