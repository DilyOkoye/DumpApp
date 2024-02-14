using DumpApp.BAL.Utilities;
using DumpApp.DAL.Implementation;
using DumpApp.DAL.Interface;
using DumpApp.DAL.Repositories;
using DumpApp.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DumpApp.BAL.AdminModel.ViewModel;

namespace DumpApp.BAL.AdminModel
{
    public class RoleModel
    {
        private readonly IUserProfileRepository repoUserProfile;
        private readonly IClientProfileRepository repoClientProfile;
        private readonly IStatusItemRepository repoStatus;
        private readonly RoleAssignmentRepo repoMenuAssignment;
        private readonly MenuControlRepo repoMenuControl;
        private readonly IRoleRepository repoRoles;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        public RoleModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoUserProfile = new UserProfileRepository(idbfactory);
            repoStatus = new StatusItemRepository(idbfactory);
            repoClientProfile = new ClientProfileRepository(idbfactory);
            repoRoles = new RoleRepository(idbfactory);
            repoMenuAssignment = new RoleAssignmentRepo(idbfactory);
            repoMenuControl = new MenuControlRepo(idbfactory);

        }

        public IEnumerable<SelectListItem> ListStatus()
        {

            IEnumerable<System.Web.Mvc.SelectListItem> items = repoStatus.GetAllNonAsync().AsEnumerable()
                 .Select(p => new System.Web.Mvc.SelectListItem
                 {
                     Text = p.Status,
                     Value = p.StatusValue

                 });
            return items;
        }


        public List<admRole> ListOfRoles()
        {

            var d = (from h in repoRoles.GetAllNonAsync()
                     select new admRole
                     {
                         RoleId = h.RoleId,
                         RoleName = h.RoleName,
                         Status = h.Status,
                     }).ToList();

            return d;
        }

        public async Task<string> GetFullname(int id)
        {
            var use = await repoUserProfile.Get(o => o.UserId == id);
            if (use != null)
            {
                return use.FullName;
            }
            return null;
        }

        public async Task<string> GetRoleName(int id)
        {
            var use = await repoRoles.Get(o => o.RoleId == id);
            if (use != null)
            {
                return use.RoleName;
            }
            return null;
        }

        public async Task<admRole> ViewDetails(int roleId)
        {
            try
            {
                var y = await repoRoles.Get(p => p.RoleId == roleId);
                if (y != null)
                {
                    return y;
                }

            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public async Task<ReturnValues> AddRoles(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();

            var t = await repoRoles.Get(c => c.RoleName.ToUpper() == p.admRole.RoleName.ToUpper());
            if (t != null)
            {
                returnVal.nErrorCode = -2;
                returnVal.sErrorText = "Role Name Already Exist.";
                return returnVal;
            }

            p.admRole.DateCreated = DateTime.Now;
            p.admRole.RoleName = p.admRole.RoleName;
            p.admRole.Status = "Active";
            p.admRole.UserId = LoginUserId;
            repoRoles.Add(p.admRole);
            try
            {
                var retV = await unitOfWork.Commit(LoginUserId) > 0 ? true : false;

                if (retV)
                {

                    returnVal.nErrorCode = 0;
                    returnVal.sErrorText = "Record Added Succesfully";
                    return returnVal;
                }
            }
            catch (Exception ex)
            {
                returnVal.nErrorCode = -1;
                returnVal.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;

                return returnVal;
            }

            return returnVal;
        }

        public async Task<ReturnValues> EditRoles(AdminViewModel p, int LoginUserId)
        {
            var returnVal = new ReturnValues();

            var y = await repoRoles.Get(a => a.RoleId == p.admRole.RoleId);
            if (y != null)
            {

                y.RoleName = p.admRole.RoleName;
                y.Status = p.admRole.Status;
                y.UserId = LoginUserId;
                repoRoles.Update(y);
                try
                {
                    var retV = await unitOfWork.Commit(LoginUserId) > 0 ? true : false;

                    if (retV)
                    {
                        returnVal.nErrorCode = 0;
                        returnVal.sErrorText = "Record Updated Succesfully";
                        return returnVal;
                    }
                }
                catch (Exception ex)
                {
                    returnVal.nErrorCode = -1;
                    returnVal.sErrorText = ex.Message == null ? ex.InnerException.Message : ex.Message;

                    return returnVal;
                }

            }

            return returnVal;
        }


        public List<RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem> AssignValuesSystemAdmin(int roleid)
        {
            var cv = new List<RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem>();
            string qry = string.Format("EXEC Isp_MenuSystemAdmin {0}", roleid);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            //SqlParameter sl = new SqlParameter("roleid", roleid);
            using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionProc"].ToString()))
            {
                con.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "Isp_MenuSystemAdmin";
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter role = new SqlParameter("role_id", SqlDbType.Int) { Value = roleid };
                    cmd.Parameters.Add(role);
                    SqlDataReader reader = cmd.ExecuteReader();
                    ds.Load(reader, LoadOption.OverwriteChanges, "Results");
                    dt = ds.Tables[0];
                    reader.Close();
                    con.Close();
                }
            }
            int count = 0;
            RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem chk = new RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem();
            var omenu = dt.Rows;
            foreach (DataRow dr in omenu)
            {


                cv.Add(new RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem()
                {

                    CanView = (bool)dr["can_view"],
                    MenuName = dr["menu_name"].ToString(),
                    MenuId = (int)dr["menu_id"],
                    CanAdd = (bool)dr["can_add"],
                    CanAuth = (bool)dr["can_auth"],
                    CanEdit = (bool)dr["can_edit"],
                    CanDelete = (bool)dr["can_delete"],
                    IsGlobalSupervisor = (bool)dr["Is_Global_Supervisor"],
                });
            }
            return cv;
        }

        public List<RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem> AssignValuesOperations(int roleid)
        {
            var cv = new List<RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem>();
            string qry = string.Format("EXEC Isp_MenuSystemOperations {0}", roleid);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            //SqlParameter sl = new SqlParameter("roleid", roleid);
            using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionProc"].ToString()))
            {
                con.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "Isp_MenuSystemOperations";
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter role = new SqlParameter("role_id", SqlDbType.Int) { Value = roleid };
                    cmd.Parameters.Add(role);
                    SqlDataReader reader = cmd.ExecuteReader();
                    ds.Load(reader, LoadOption.OverwriteChanges, "Results");
                    dt = ds.Tables[0];
                    reader.Close();
                    con.Close();
                }
            }
            int count = 0;
            RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem chk = new RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem();
            var omenu = dt.Rows;
            foreach (DataRow dr in omenu)
            {


                cv.Add(new RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem()
                {
                    CanView = (bool)dr["can_view"],
                    MenuName = dr["menu_name"].ToString(),
                    MenuId = (int)dr["menu_id"],
                    CanAdd = (bool)dr["can_add"],
                    CanAuth = (bool)dr["can_auth"],
                    CanEdit = (bool)dr["can_edit"],
                    CanDelete = (bool)dr["can_delete"],
                    IsGlobalSupervisor = (bool)dr["Is_Global_Supervisor"],
                });
            }
            return cv;
        }

        public List<RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem> AssignValuesReports(int roleid)
        {
            var cv = new List<RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem>();
            string qry = string.Format("EXEC Isp_MenuSystemReports {0}", roleid);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            //SqlParameter sl = new SqlParameter("roleid", roleid);
            using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionProc"].ToString()))
            {
                con.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "Isp_MenuSystemReports";
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter role = new SqlParameter("role_id", SqlDbType.Int) { Value = roleid };
                    cmd.Parameters.Add(role);
                    SqlDataReader reader = cmd.ExecuteReader();
                    ds.Load(reader, LoadOption.OverwriteChanges, "Results");
                    dt = ds.Tables[0];
                    reader.Close();
                    con.Close();
                }
            }
            // List<Isp_BankroleMenu_Result> omenu = repoaIsp_BankroleMenu_Result.LoadViaStockProc(qry, null).ToList();
            int count = 0;
            RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem chk = new RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem();
            var omenu = dt.Rows;
            foreach (DataRow dr in omenu)
            {


                cv.Add(new RolePriviledgeReturnValues.CheckBoxRoleAssignmentDetailsSystem()
                {

                    CanView = (bool)dr["can_view"],
                    MenuName = dr["menu_name"].ToString(),
                    MenuId = (int)dr["menu_id"],
                    CanAdd = (bool)dr["can_add"],
                    CanAuth = (bool)dr["can_auth"],
                    CanEdit = (bool)dr["can_edit"],
                    CanDelete = (bool)dr["can_delete"],
                    IsGlobalSupervisor = (bool)dr["Is_Global_Supervisor"],
                });
            }
            return cv;
        }

        public async Task<ReturnValues> AddMenusForUser(AdminViewModel crm, int rolId, int loginid)
        {
            var rtv = new ReturnValues();
            try
            {
                int CanAdd = 0;
                int IsGlobalSupervisor = 0;
                int CanEdit = 0;
                int canView = 0;

                var RoleAssig = await repoMenuAssignment.GetMany(r => r.role_id == rolId);

                foreach (var i in RoleAssig)
                {
                    repoMenuAssignment.Delete(i);
                    var retV = await unitOfWork.Commit((int)loginid) > 0 ? true : false;
                }

                #region // System Admin
                if (crm.AssignAdmin != null)
                {
                    foreach (var i in crm.AssignAdmin)
                    {
                        CanAdd = i.CanAdd ? 1 : 0;
                        CanEdit = i.CanEdit ? 1 : 0;
                        IsGlobalSupervisor = i.IsGlobalSupervisor ? 1 : 0;
                        canView = i.CanView ? 1 : 0;

                        var rAssign = new admRoleAssignment()
                        {
                            is_global_supervisor = (byte)IsGlobalSupervisor,
                            can_add = (byte)CanAdd,
                            can_edit = (byte)CanEdit,
                            can_view = (byte)canView,
                            menu_id = i.MenuId,
                            role_id = rolId,
                        };

                        if (canView == 1)
                            repoMenuAssignment.Add(rAssign);
                        var retV = await unitOfWork.Commit((int)loginid) > 0 ? true : false;

                        CanAdd = 0;
                        IsGlobalSupervisor = 0;
                        CanEdit = 0;
                        canView = 0;
                    }



                }
                #endregion

                #region // Operation
                if (crm.AssignOperations != null)
                {
                    foreach (var i in crm.AssignOperations)
                    {
                        CanAdd = i.CanAdd ? 1 : 0;
                        CanEdit = i.CanEdit ? 1 : 0;
                        IsGlobalSupervisor = i.IsGlobalSupervisor ? 1 : 0;
                        canView = i.CanView ? 1 : 0;
                        var rAssign = new admRoleAssignment()
                        {
                            is_global_supervisor = (byte)IsGlobalSupervisor,
                            can_add = (byte)CanAdd,
                            can_edit = (byte)CanEdit,
                            can_view = (byte)canView,
                            menu_id = i.MenuId,
                            role_id = rolId,// crm.RoleId, crm.RoleId,
                        };

                        if (canView == 1)
                            repoMenuAssignment.Add(rAssign);
                        var retV = await unitOfWork.Commit((int)loginid) > 0 ? true : false;

                        CanAdd = 0;
                        IsGlobalSupervisor = 0;
                        CanEdit = 0;
                        canView = 0;
                    }


                }
                #endregion

                #region // Reports
                if (crm.AssignReports != null)
                {
                    foreach (var i in crm.AssignReports)
                    {
                        CanAdd = i.CanAdd ? 1 : 0;
                        CanEdit = i.CanEdit ? 1 : 0;
                        IsGlobalSupervisor = i.IsGlobalSupervisor ? 1 : 0;
                        canView = i.CanView ? 1 : 0;
                        var rAssign = new admRoleAssignment()
                        {
                            is_global_supervisor = (byte)IsGlobalSupervisor,
                            can_add = (byte)CanAdd,
                            can_edit = (byte)CanEdit,
                            can_view = (byte)canView,
                            menu_id = i.MenuId,
                            role_id = rolId,// crm.RoleId, crm.RoleId,
                        };

                        if (canView == 1)
                            repoMenuAssignment.Add(rAssign);
                        var retV = await unitOfWork.Commit((int)loginid) > 0 ? true : false;

                        CanAdd = 0;
                        IsGlobalSupervisor = 0;
                        CanEdit = 0;
                        canView = 0;
                    }

                }
                #endregion



                rtv.nErrorCode = 0;
                rtv.sErrorText = "Menu(s) Assigned successfully.";
                return rtv;
            }
            catch (Exception ex)
            {

                return rtv;
            }

        }


    }
}
