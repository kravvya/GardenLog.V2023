using GardenLog.SharedKernel.Enum;
using GardenLog.SharedKernel.Interfaces;
using ImageCatalog.Api.Data;
using ImageCatalog.Api.Model;
using ImageCatalog.Contract.ViewModels;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenLogAdminConsole.Images
{
    internal class AdminImageRepository : ImageRepository
    {
        public AdminImageRepository(IUnitOfWork unitOfWork, ILogger<ImageRepository> logger)
            : base(unitOfWork, logger)
        {

        }

        internal async Task<List<Image>> GetAllImages()
        {
            return await Collection.Find<Image>(new BsonDocument()).ToListAsync();
        }
    }
}
