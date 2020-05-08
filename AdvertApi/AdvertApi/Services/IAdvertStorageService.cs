using AdvertApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvertApi.Services
{
    public interface IAdvertStorageService
    {
        Task<string> Add(AdvertModel model);

        //Task<bool> Confirm(ConfirmAdvertModel model);
        Task Confirm(ConfirmAdvertModel model);

        Task<AdvertModel> GetById(string id);

        Task<bool> CheckHealthAsync();

        Task<List<AdvertModel>> GetAllAsync();
    }
}
