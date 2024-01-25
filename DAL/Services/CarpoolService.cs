using CarpooliDotTN.DAL.IRepositories;
using CarpooliDotTN.DAL.IServices;
using CarpooliDotTN.Models;
using Microsoft.EntityFrameworkCore;

namespace CarpooliDotTN.DAL.Services
{

    public class CarpoolService : ICarpoolService
    {
        private readonly ICarpoolRepository _carpoolRepository;

        public CarpoolService(ICarpoolRepository carpoolRepository)
        {
            _carpoolRepository = carpoolRepository;
        }

        public async Task<IQueryable<Carpool>> GetQueryableCarpoolsAsync()
        {
            return await _carpoolRepository.GetQueryableCarpoolsAsync();
        }

        public async Task<IQueryable<Carpool>> GetQueryableCarpoolsWithDemandsAsync()
        {
            return await _carpoolRepository.GetQueryableCarpoolsWithDemandsAsync();
        }

        public async Task<Carpool> GetCarpoolByIdAsync(Guid id)
        {
            return await _carpoolRepository.GetCarpoolByIdAsync(id);
        }

        public async Task<IEnumerable<string>> GetDistinctCitiesAsync()
        {
            return await _carpoolRepository.GetDistinctCitiesAsync();
        }

        public async Task AddCarpoolAsync(Carpool carpool)
        {
            await _carpoolRepository.AddCarpoolAsync(carpool);
        }

        public async Task UpdateCarpoolAsync(Carpool carpool)
        {
            await _carpoolRepository.UpdateCarpoolAsync(carpool);
        }

        public async Task DeleteCarpoolAsync(Guid id)
        {
            await _carpoolRepository.DeleteCarpoolAsync(id);
        }

        public bool CarpoolExists(Guid id)
        {
            return _carpoolRepository.CarpoolExists(id);
        }
    }



}
