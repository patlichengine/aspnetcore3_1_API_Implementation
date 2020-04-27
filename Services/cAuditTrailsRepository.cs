using AutoMapper;
using DocumentTracking.API.DBContext;
using DocumentTracking.API.Entities;
using DocumentTracking.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Services
{
    public class cAuditTrailsRepository : IAuditTrailsRepository, IDisposable
    {
        private readonly WDocumentTrackingContext _context;
        private readonly IMapper _mapper;

        public cAuditTrailsRepository(WDocumentTrackingContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        public async Task<AuditTrailsDto> CreateAudit(Guid userId, AuditTrailsCreateDto auditTrail)
        {
            return await Task.Run(async () =>
            {
                if (auditTrail == null)
                {
                    throw new ArgumentNullException(nameof(auditTrail));
                }
                var record = _mapper.Map<Entities.AuditTrail>(auditTrail);


                record.Id = Guid.NewGuid();
                record.UserId = userId;
                record.DateCreated = DateTime.Now;
                _context.AuditTrail.Add(record);

                //call the save method
                bool saveResult = await Save();

                //get the list of ids as string from the data
                return _mapper.Map<AuditTrailsDto>(record);
            });
        }

        public async Task<IEnumerable<AuditTrailsDto>> GetAuditTrails ()
        {
            return await Task.Run(async () =>
            {
                List<AuditTrail> result = await _context.AuditTrail.ToListAsync<AuditTrail>();
                return _mapper.Map<IEnumerable<AuditTrailsDto>>(result);

            });
        }

        public async Task<IEnumerable<AuditTrailsDto>> GetAuditTrails(Guid userId)
        {
            return await Task.Run(async () =>
            {
                List<AuditTrail> result = await _context.AuditTrail.Where(c => c.UserId == userId).ToListAsync();
                if (result == null)
                {
                    throw new ArgumentNullException(nameof(result));
                }
                return _mapper.Map<IEnumerable<AuditTrailsDto>>(result);

            });
        }

        public async Task<AuditTrailsDto> GetAuditTrail(Guid userId, Guid auditId)
        {
            return await Task.Run(async () =>
            {
                AuditTrail result = await _context.AuditTrail.FirstOrDefaultAsync(c => c.Id == auditId);
                if (result == null)
                {
                    throw new ArgumentNullException(nameof(result));
                }
                return _mapper.Map<AuditTrailsDto>(result);
            });
        }

        public async Task<bool> Save()
        {
            return await Task.Run(async () =>
            {
                return (await _context.SaveChangesAsync() >= 0);
            });

        }


        public async Task<AuditTrailsDto> UpdateAudit(Guid auditId, Guid userId, AuditTrailsUpdateDto auditTrail)
        {
            return await Task.Run(async () =>
            {
                if (auditTrail == null)
                {
                    throw new ArgumentNullException(nameof(auditTrail));
                }

                //Get the audit record from the existing audit
                var audit = await _context.AuditTrail.FirstOrDefaultAsync(c => c.Id == auditId && c.UserId == userId);

                //map the records with the data you are updating

                var record = _mapper.Map(auditTrail, audit);
                record.DateCreated = DateTime.Now;
                //call the save method
                bool saveResult = await Save();

                //get the list of ids as string from the data
                return _mapper.Map<AuditTrailsDto>(record);
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
