using DocumentTracking.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Services
{
    public interface IAuditTrailsRepository
    {
        public Task<AuditTrailsDto> CreateAudit(Guid userId, AuditTrailsCreateDto auditTrail);

        public Task<IEnumerable<AuditTrailsDto>> GetAuditTrails();

        public Task<IEnumerable<AuditTrailsDto>> GetAuditTrails(Guid userId);

        public Task<AuditTrailsDto> GetAuditTrail(Guid userId, Guid auditId);

        public Task<bool> Save();

        public Task<AuditTrailsDto> UpdateAudit(Guid auditId, Guid userId, AuditTrailsUpdateDto auditTrail);
    }
}
