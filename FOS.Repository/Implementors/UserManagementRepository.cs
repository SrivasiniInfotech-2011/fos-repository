using Dapper;
using FOS.Models;
using FOS.Models.Constants;
using FOS.Models.Entities;
using FOS.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FOS.Models.Constants.Constants;

namespace FOS.Repository.Implementors
{

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="connectionString"></param>
    public class UserManagementRepository(string connectionString) : IUsermanagementRepository
    {

        /// <summary>
        /// Get List of UserLevelLookup.
        /// <param name="companyId">Company Id</param>
        /// <param name="lobId">Lob Id</param>
        /// <param name="isActive"> Is Active</param>
        /// <param name="userId">User Id</param>
        /// </summary>
        /// <returns>List of <see cref="Location"/></returns>
        public async Task<List<Lookup>> GetUserLevelLookup(int? companyId, int? userId)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                try
                {
                    //var lstLocations = await connection.QueryAsync<Lookup>(SqlCommandConstants.GetUserLevellookup);
                    var lstlookup = await connection.QueryAsync<Lookup>(SqlCommandConstants.GetUserLevellookup, new
                    {
                        CompanyId = companyId,
                        UserId = userId,

                    });
                    return lstlookup.ToList();
                }
                catch ( Exception ex) { throw; }
                
                
            }
        }



        /// <summary>
        /// Get List of Userdesignationlevel Lookupp.
        /// <param name="companyId">Company Id</param>
        /// </summary>
        /// <returns>List of <see cref="Lookup"/></returns>
        public async Task<List<Lookup>> GetUserdesignationlevelLookup(int? companyId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var lstLookups = await connection.QueryAsync<Lookup>(SqlCommandConstants.GetUserDesignationlookup, new
                {
                    CompanyId = companyId,
                });
                return lstLookups.ToList();
                }
                catch (Exception ex) { throw; }
            }
        }


        /// <summary>
        /// Get List of Userreportinglevel Lookupp.
        /// 
        /// </summary>
        /// <returns>List of <see cref="ReportingLevel"/></returns>
        public async Task<List<ReportingLevel>> getUserReportingLevel(int? companyId,int? userId,string? prefixText, int? lobId, int? locationId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var lstLookups = await connection.QueryAsync<ReportingLevel>(SqlCommandConstants.FOS_ORG_GET_UserNameAGT, new
                    {
                        Company_ID = companyId,
                        User_ID = userId,
                        PrefixText = prefixText,
                        LOB_ID = lobId,
                    location_ID = locationId

                });
                    return lstLookups.ToList();
                }
                catch (Exception ex) { throw; }
            }
        }


        /// <summary>
        /// Get the User Details.
        /// </summary>
        /// <returns>List of <see cref="InsertUserDetailsModel"/></returns>

        public async Task<GetInsertUserDetailsModel> GetExistingUserDetails(int? companyId, int? userId)
        {
            var userdetails = new GetInsertUserDetailsModel();
            using (var connection = new SqlConnection(connectionString))
            {

                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SqlCommandConstants.FOS_SYSAD_GET_UserViewDetails;
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
            
                var dataAdapter = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                dataAdapter.Fill(ds);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {

                        try
                        {
                            var dr = ds.Tables[0].Rows[0];
                            userdetails = new GetInsertUserDetailsModel



                            {
                                UserID = Convert.ToInt16(dr[SqlColumnNames.UserID]),
                                UserCode = Convert.ToString(dr[SqlColumnNames.UserCode]),
                                UserName = Convert.ToString(dr[SqlColumnNames.UserName]),
                                GenderId = Convert.ToInt32(dr[SqlColumnNames.Gender_Id]),
                                //GenderName = Convert.ToString(dr[SqlColumnNames.Gender_Id]),
                                Password = Convert.ToString(dr[SqlColumnNames.Password]),
                                DOJ = Convert.ToDateTime(dr[SqlColumnNames.DOJ]),
                                MobileNumber = Convert.ToString(dr[SqlColumnNames.MobileNumber]),
                                EmergencycontactNumber = Convert.ToString(dr[SqlColumnNames.EmergencycontactNumber]),
                                Designation = Convert.ToInt32(dr[SqlColumnNames.Designationid]),
                                UserLevelID = Convert.ToInt32(dr[SqlColumnNames.UserLevelID]),
                                ReportingNextlevel = Convert.ToInt32(dr[SqlColumnNames.ReportingNextlevel]),
                                ReportingGHigherLevel = Convert.ToString(dr[SqlColumnNames.REPORTING_HIGHER_LEVEL]),
                                MarutialStatusDiscription = Convert.ToString(dr[SqlColumnNames.marutialStatusDiscription]),                       
                                UserLevel = Convert.ToString(dr[SqlColumnNames.UserLevelID]),                                                                                      
                                EmailID = Convert.ToString(dr[SqlColumnNames.Email]),
                                Dateofbirth = Convert.ToDateTime(dr[SqlColumnNames.DoB]),
                                FatherName = Convert.ToString(dr[SqlColumnNames.FatherName]),
                                MotherName = Convert.ToString(dr[SqlColumnNames.MotherName]),
                                SpouseName = Convert.ToString(dr[SqlColumnNames.Spouse_Name]),
                                MaritialID = Convert.ToInt32(dr[SqlColumnNames.MaritialID]),
                                //MaritiaStatuslID= Convert.ToInt32(dr[SqlColumnNames.MaritialID]),
                                AadharNumber = Convert.ToString(dr[SqlColumnNames.AadharNumber]),
                                PanNumber = Convert.ToString(dr[SqlColumnNames.PanNumber]),
                                Address = Convert.ToString(dr[SqlColumnNames.Address]),
                                UserImagepath = Convert.ToString(dr[SqlColumnNames.UserImagepath])



                            };
                        }
                        catch ( Exception ex)
                        {
                            throw;
                        }
                    }

                }
                return userdetails;
            }
        }




        /// <summary>
        /// GetUserTranslander.
        /// </summary>
        /// <returns>List of <see cref="GetUserTranslanderModel"/></returns>

        public async Task<List<GetUserTranslanderModel>> getUsertranslander(int? companyId, int? userId)
        {
            var userdetailsList = new List<GetUserTranslanderModel>(); // List to store all user details

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(); // Use async for non-blocking call
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SqlCommandConstants.FOS_Sysad_Get_UserMaster;
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.USER_MANAGEMENT_COMPANY_ID, companyId));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.USER_MANAGEMENT_USER_ID, userId));

                var dataAdapter = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                dataAdapter.Fill(ds);

                if (ds != null && ds.Tables.Count > 0)
                {
                    var table = ds.Tables[0]; // Assuming the first table holds the relevant data
                    foreach (DataRow dr in table.Rows)
                    {
                        var userdetails = new GetUserTranslanderModel
                        {
                            UserID = Convert.ToInt32(dr[SqlColumnNames.UserID]),
                            UserCode = Convert.ToString(dr[SqlColumnNames.UserCode]),
                            UserName = Convert.ToString(dr[SqlColumnNames.UserName]),
                            MobileNumber = Convert.ToString(dr[SqlColumnNames.MobileNumberr]),
                            Designation = Convert.ToString(dr[SqlColumnNames.Designation]),
                            Userlevel = Convert.ToString(dr[SqlColumnNames.UserLevel]),
                            EmailID = Convert.ToString(dr[SqlColumnNames.Email]),
                        };
                        userdetailsList.Add(userdetails); // Add each row to the list
                    }
                }

                return userdetailsList; // Return the list of all user details
            }
        }





        /// <summary>
        /// Inserts a UserDetails.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="User_ID">User Id</param>
        /// <param name="UserCode">UserCode</param>
        /// <param name="UserName">User Name </param>
        /// <param name="genderId">Gender Id</param>
        /// <param name="Password">@Password</param>
        /// <param name="mobileNumber">Mobile Number</param>
        /// <param name="EmergencycontactNumber">EmergencycontactNumber</param>
        /// <param name="DOJ">DOJ</param>
        /// <param name="Designation">Designation</param>
        /// <param name="UserLevelID">UserLevelID  </param>
        /// <param name="ReportingNextlevel">ReportingNextlevel  </param>
        /// <param name="User_Group">User_Group </param>
        /// <param name="EmailID">EmailID</param>
        /// <param name="Dateofbirth">Dateofbirth </param>
        /// <param name="FatherName">FatherName</param>
        /// <param name="MotherName">MotherName </param>
        /// <param name="SpouseName">SpouseName</param>
        /// <param name="Maritial_ID">Maritial_ID</param>
        /// <param name="Aadhar_Number">Aadhar_Number</param>
        /// <param name="PAN_Number">PAN_Number</param>
        /// <param name="Address"> Address</param>
        /// <param name="User_Imagepath">User_Imagepath </param>
        /// <param name="Is_Active">Is_Active </param>
        /// <param name="createdBy">Created By User Id.</param>
        /// <param name="errorCode">Error Code</param>
        /// <returns>Integer Value Indicating if the record got saved.</returns>


        public async Task<int> InsertUserDetails(
                       int companyId, int User_ID, string UserCode, string UserName, int? genderId,
                                           string Password, DateTime? DOJ, string mobileNumber, string? EmergencycontactNumber, int? Designation,
                                           int UserLevelID, int ReportingNextlevel, int? User_Group, string EmailID, DateTime? Dateofbirth, string FatherName, string MotherName
                                          , string SpouseName, int Maritial_ID, string Aadhar_Number, string PAN_Number, string Address, string User_Imagepath, int Is_Active,
                                           int createdBy, int errorCode)
        {
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_COMPANY_ID, companyId, DbType.Int32, ParameterDirection.Input);
                    if (User_ID != 0)
                        parameters.Add(SqlParameterConstants.USER_MANAGEMENT_USER_ID, User_ID, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_USER_CODE, UserCode, DbType.String, ParameterDirection.Input, 50);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_PASSWORD, Password, DbType.String, ParameterDirection.Input, 50);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_GENDER_ID, genderId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_USER_NAME, UserName, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_DOJ, DOJ, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_DATEOFBIRTH, Dateofbirth, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_MOBILE_NUMBER, mobileNumber, DbType.String, ParameterDirection.Input, 20);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_EMERGENCY_CONTACT_NUMBER, EmergencycontactNumber, DbType.String, ParameterDirection.Input, 20);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_DESIGNATION, Designation, DbType.String, ParameterDirection.Input, 40);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_USER_LEVEL_ID, UserLevelID, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_REPORTING_NEXT_LEVEL, ReportingNextlevel, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_USER_GROUP, User_Group, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_MARITIAL_ID, Maritial_ID, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_EMAIL_ID, EmailID, DbType.String, ParameterDirection.Input, 20);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_FATHER_NAME, FatherName, DbType.String, ParameterDirection.Input, 20);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_MOTHER_NAME, MotherName, DbType.String, ParameterDirection.Input, 150);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_SPOUSE_NAME, SpouseName, DbType.String, ParameterDirection.Input, 20);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_AADHAR_NUMBER, Aadhar_Number, DbType.String, ParameterDirection.Input, 150);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_PAN_NUMBER, PAN_Number, DbType.String, ParameterDirection.Input, 150);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_USER_IMAGE_PATH, User_Imagepath, DbType.String, ParameterDirection.Input, 150);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_CREATED_BY, createdBy, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_IS_ACTIVE, Is_Active, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_Address, Address, DbType.String, ParameterDirection.Input, 500);
                    parameters.Add(SqlParameterConstants.USER_MANAGEMENT_ERROR_CODE, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await connection.ExecuteAsync(SqlCommandConstants.FOS_SYSAD_Insert_UserDetails, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                    transaction.Commit();
                    return parameters.Get<int?>(SqlParameterConstants.USER_MANAGEMENT_ERROR_CODE).GetValueOrDefault();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

    }
}
