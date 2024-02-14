using System.Collections.Generic;
using System.Linq;
using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;

namespace DumpApp.BAL.OperationsModel
{
    public class DumpModel
    {
        private readonly IUserProfileRepository repoUserProfileRepository;
        private readonly IDumpTypeRepository repoDumpTypeRepository;
        private readonly IDumpRepository repoDumpRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        
        public DumpModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoUserProfileRepository = new UserProfileRepository(idbfactory);
            repoDumpTypeRepository = new DumpTypeRepository(idbfactory);
            repoDumpRepository = new DumpRepository(idbfactory);
            
        }

        public List<Dumps> ListOfDumps()
        {

            var d = (from h in repoDumpRepository.GetAllNonAsync()
                select new Dumps()
                {
                   Name = h.Name,
                   Status = h.Status,
                   Description = h.Description,
                   DumpType = h.DumpType == null? "": GetDumpTypeName((int)h.DumpType),
                   DumpDate = h.DumpDate == null?"": $"{h.DumpDate:F}",
                   TapeType = h.TapeType,
                   DateCreated = h.DateCreated == null ? "" : $"{h.DateCreated:F}",
                   TapeIdentifier = h.TapeIdentifier,
                   Id = h.Id,
                   CreatedBy = h.CreatedBy == null ? "" : GetUserById((int)h.CreatedBy),
                }).ToList();

            return d;
        }


        public string GetUserById(int id)
        {
            var use =  repoUserProfileRepository.GetNonAsync(o => o.UserId == id);
            if (use != null)
            {
                return use.FullName;
            }
            return null;
        }


        public string GetDumpTypeName(int id)
        {
            var dumpType =  repoDumpTypeRepository.GetNonAsync(o => o.Id == id);
            if (dumpType != null)
            {
                return dumpType.Name;
            }
            return null;
        }
    }
}
