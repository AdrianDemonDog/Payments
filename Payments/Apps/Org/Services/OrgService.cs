using ErrorOr;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Payments.Apps.Org.Interfaces;
using Payments.Apps.Org.Models;
using Payments.Apps.User.Models;
using Payments.DTOs;

namespace Payments.Apps.Org.Services
{
    public class OrgService : IOrgService
    {
        private readonly IMongoCollection<OrgModel> _orgCollection;

        public OrgService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("PaymentsDB");
            _orgCollection = database.GetCollection<OrgModel>("Orgs");
        }

        public async Task<ErrorOr<OrgModel>> CreateOrg(OrgDto.CreateOrgDto createOrgDto)
        {
            var maxVisualId = await _orgCollection
                .Find(_ => true)
                .SortByDescending(o => o.VisualId)
                .Limit(1)
                .Project(o => o.VisualId)
                .FirstOrDefaultAsync();

            var newOrg = new OrgModel
            {
                VisualId = maxVisualId + 1,
                Name = createOrgDto.Name,
                Type = createOrgDto.Type,
                ParentId = createOrgDto.ParentId,
                ParentName = createOrgDto.ParentName,
                Region = createOrgDto.Region,
                CostPlace = createOrgDto.CostPlace,
                CreatedAt = DateTime.UtcNow,
                // CreatedBy = "System"
            };

            await _orgCollection.InsertOneAsync(newOrg);
            return newOrg;
        }

        public async Task<ErrorOr<bool>> DeleteOrg(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Error.Validation("InvalidId", "The organization ID cannot be null or empty.");
            }

            var deleteResult = await _orgCollection.DeleteOneAsync(o => o.Id == id);
            if (deleteResult.DeletedCount == 0)
            {
                return Error.NotFound("OrgNotFound", "Organization not found.");
            }

            return true;
        }

        public async Task<ErrorOr<List<OrgModel>>> GetAllOrgs()
        {
            var orgs = await _orgCollection.Find(_ => true).ToListAsync();
            return orgs;
        }

        public async Task<ErrorOr<OrgModel>> GetOrgById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Error.Validation("InvalidId", "The organization ID cannot be null or empty.");
            }

            var org = await _orgCollection.Find(o => o.Id == id).FirstOrDefaultAsync();
            if (org == null)
            {
                return Error.NotFound("OrgNotFound", "Organization not found.");
            }

            return org;
        }

        public async Task<ErrorOr<OrgModel>> UpdateOrg(string id, OrgDto.UpdateOrgDto updateOrgDto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Error.Validation("InvalidId", "The organization ID cannot be null or empty.");
            }

            var existingOrg = await _orgCollection.Find(o => o.Id == id).FirstOrDefaultAsync();
            if (existingOrg == null)
            {
                return Error.NotFound("OrgNotFound", "Organization not found.");
            }

            // Update only allowed fields
            if (!string.IsNullOrWhiteSpace(updateOrgDto.Name))
                existingOrg.Name = updateOrgDto.Name;

            if (!string.IsNullOrWhiteSpace(updateOrgDto.Type))
                existingOrg.Type = updateOrgDto.Type;

            if (!string.IsNullOrWhiteSpace(updateOrgDto.ParentId))
                existingOrg.ParentId = updateOrgDto.ParentId;

            if (!string.IsNullOrWhiteSpace(updateOrgDto.ParentName))
                existingOrg.ParentName = updateOrgDto.ParentName;

            if (!string.IsNullOrWhiteSpace(updateOrgDto.Region))
                existingOrg.Region = updateOrgDto.Region;

            if (!string.IsNullOrWhiteSpace(updateOrgDto.CostPlace))
                existingOrg.CostPlace = updateOrgDto.CostPlace;

            existingOrg.UpdatedAt = DateTime.UtcNow;
            // existingOrg.UpdatedBy = "System";

            var updateResult = await _orgCollection.ReplaceOneAsync(
                filter: o => o.Id == id,
                replacement: existingOrg
            );

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return Error.Failure("UpdateFailed", "Failed to update the organization.");
            }

            return existingOrg;
        }
    }
}
