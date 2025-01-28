using ErrorOr;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using Payments.Apps.kyc.Interfaces;
using Payments.Apps.Org.Models;
using static Payments.DTOs.OrgDto;

namespace Payments.Apps.kyc.Services
{
    public class KycService : IKycService
    {
        private readonly IMongoCollection<KycModel> _kycCollection;
        private readonly IMongoCollection<OrgModel> _orgCollection;

        public KycService(IMongoDatabase database)
        {
            _kycCollection = database.GetCollection<KycModel>("kyc");
            _orgCollection = database.GetCollection<OrgModel>("company");
        }

        public async Task<ErrorOr<KycModel>> CreateKycAsync(CreateKycDto createKycDto)
        {
            if (createKycDto == null || createKycDto.CompanyId <= 0 || string.IsNullOrWhiteSpace(createKycDto.Status))
            {
                return Error.Validation("InvalidKycData", "KYC data is invalid or incomplete.");
            }

            var kycModel = new KycModel
            {
                CompanyId = createKycDto.CompanyId,
                Status = createKycDto.Status,
                CompanyData = createKycDto.CompanyData != null ? Newtonsoft.Json.JsonConvert.SerializeObject(createKycDto.CompanyData) : null,
                GroupData = createKycDto.GroupData != null ? Newtonsoft.Json.JsonConvert.SerializeObject(createKycDto.GroupData) : null,
                StatusChanged = DateTime.UtcNow
            };

            await _kycCollection.InsertOneAsync(kycModel);

            return kycModel;
        }

        public async Task<ErrorOr<KycModel>> UpdateKycStatusAsync(string kycId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(kycId) || string.IsNullOrWhiteSpace(newStatus))
            {
                return Error.Validation("InvalidInput", "KYC ID and new status are required.");
            }

            if (!ObjectId.TryParse(kycId, out var objectId))
            {
                return Error.Validation("InvalidKycId", "KYC ID is not a valid ObjectId.");
            }

            var filter = Builders<KycModel>.Filter.Eq(k => k.Id, kycId);
            var update = Builders<KycModel>.Update
                .Set(k => k.Status, newStatus)
                .Set(k => k.StatusChanged, DateTime.UtcNow);

            var result = await _kycCollection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<KycModel> { ReturnDocument = ReturnDocument.After });

            if (result == null)
            {
                return Error.NotFound("KycNotFound", "KYC record not found.");
            }

            // Update the status in the corresponding company record
            var orgFilter = Builders<OrgModel>.Filter.Eq(o => o.Id, result.CompanyId.ToString());
            var orgUpdate = Builders<OrgModel>.Update.Set(o => o.KYC, newStatus);
            await _orgCollection.UpdateOneAsync(orgFilter, orgUpdate);

            return result;
        }

    }

}
