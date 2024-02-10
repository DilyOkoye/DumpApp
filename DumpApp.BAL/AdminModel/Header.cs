using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;
using System;

namespace DumpApp.BAL.AdminModel
{
    public class Header
    {

        private readonly IUserProfileRepository repoUserProfile;
        private readonly IClientProfileRepository repoClientProfile;
        private readonly IRoleRepository repoRoles;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;

        public Header()
        {

            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoUserProfile = new UserProfileRepository(idbfactory);
            repoClientProfile = new ClientProfileRepository(idbfactory);
            repoRoles = new RoleRepository(idbfactory);
        }


        public string GetFullName(int ItbId)
        {
            return repoUserProfile.GetById(ItbId).FullName;
        }

        public string GetRole(int ItbId)
        {
            try
            {
                return repoRoles.GetById(ItbId).RoleName;
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}
