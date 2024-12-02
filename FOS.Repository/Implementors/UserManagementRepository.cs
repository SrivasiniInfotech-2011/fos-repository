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
                //var lstLocations = await connection.QueryAsync<Lookup>(SqlCommandConstants.GetUserLevellookup);
                var lstlookup = await connection.QueryAsync<Lookup>(SqlCommandConstants.GetUserLevellookup, new
                {
                    CompanyId = companyId,
                    UserId = userId,
                    
                });
                return lstlookup.ToList();
            }
        }



        /// <summary>
        /// Get List of Userdesignationlevel Lookupp.
        /// </summary>
        /// <returns>List of <see cref="Lookup"/></returns>
        public async Task<List<Lookup>> GetUserdesignationlevelLookup()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var lstLookups = await connection.QueryAsync<Lookup>(SqlCommandConstants.GetUserDesignationlookup);
                return lstLookups.ToList();
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


        //public async Task<int> InsertUserDetails(int companyId, int User_ID, string UserCode, string UserName, int? genderId,
        //                                   string Password, DateTime? DOJ, string mobileNumber, string? EmergencycontactNumber, string Designation,
        //                                   int UserLevelID, int ReportingNextlevel, int User_Group, string EmailID, DateTime? Dateofbirth, string FatherName, string MotherName
        //                                  , string SpouseName, int Maritial_ID, string Aadhar_Number, string PAN_Number, string Address, string User_Imagepath, int Is_Active,
        //                                   int createdBy, int errorCode)
        //{
        //    var prospect = new UserInsertDetails();
        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        var transaction = connection.BeginTransaction();
        //        try
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId, DbType.Int32, ParameterDirection.Input);;
        //            parameters.Add(SqlParameterConstants.PROSPECT_CUSTOMER_CODE, UserCode, DbType.String, ParameterDirection.Input, 50);
        //            parameters.Add(SqlParameterConstants.PROSPECT_CUSTOMER_CODE, Password, DbType.String, ParameterDirection.Input, 50);
        //            parameters.Add(SqlParameterConstants.PROSPECT_GENDER_ID, genderId, DbType.Int32, ParameterDirection.Input);
        //            parameters.Add(SqlParameterConstants.PROSPECT_NAME, UserName, DbType.String, ParameterDirection.Input, 100);
        //            parameters.Add(SqlParameterConstants.PROSPECT_DATEOFBIRTH, DOJ, DbType.DateTime, ParameterDirection.Input);
        //            parameters.Add(SqlParameterConstants.PROSPECT_DATEOFBIRTH, Dateofbirth, DbType.DateTime, ParameterDirection.Input);
        //            parameters.Add(SqlParameterConstants.PROSPECT_MOBILE_NUMBER, mobileNumber, DbType.String, ParameterDirection.Input, 20);
        //            parameters.Add(SqlParameterConstants.PROSPECT_ALTERNATE_MOBILENUMBER, EmergencycontactNumber, DbType.String, ParameterDirection.Input, 20);
        //            parameters.Add(SqlParameterConstants.PROSPECT_EMAIL, Designation, DbType.String, ParameterDirection.Input, 40);
        //            parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_STATE_ID, UserLevelID, DbType.Int32, ParameterDirection.Input);
        //            parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_COUNTRY_ID, ReportingNextlevel, DbType.Int32, ParameterDirection.Input);
        //            parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_STATE_ID, User_Group, DbType.Int32, ParameterDirection.Input);
        //            parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_COUNTRY_ID, Maritial_ID, DbType.Int32, ParameterDirection.Input);
        //            parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_PINCODE, EmailID, DbType.String, ParameterDirection.Input, 20);
        //            parameters.Add(SqlParameterConstants.PROSPECT_AADHAR_NUMBER, FatherName, DbType.String, ParameterDirection.Input, 20);
        //            parameters.Add(SqlParameterConstants.PROSPECT_AADHAR_IMPAGEPATH, MotherName, DbType.String, ParameterDirection.Input, 150);
        //            parameters.Add(SqlParameterConstants.PROSPECT_PAN_NUMBER, SpouseName, DbType.String, ParameterDirection.Input, 20);
        //            parameters.Add(SqlParameterConstants.PROSPECT_PAN_IMAGEPATH, Aadhar_Number, DbType.String, ParameterDirection.Input, 150);
        //            parameters.Add(SqlParameterConstants.PROSPECT_IMAGEPATH, PAN_Number, DbType.String, ParameterDirection.Input, 150);
        //            parameters.Add(SqlParameterConstants.PROSPECT_IMAGEPATH, User_Imagepath, DbType.String, ParameterDirection.Input, 150);
        //            parameters.Add(SqlParameterConstants.PROSPECT_CREATED_BY, createdBy, DbType.Int32, ParameterDirection.Input);
        //            parameters.Add(SqlParameterConstants.PROSPECT_ID, Is_Active, DbType.Int32, ParameterDirection.Input);
        //            parameters.Add(SqlParameterConstants.PROSPECT_CODE, Address, DbType.String, ParameterDirection.Input, 500);
        //            parameters.Add(SqlParameterConstants.PROSPECT_ERROR_CODE, dbType: DbType.Int32, direction: ParameterDirection.Output);
        //            await connection.ExecuteAsync(SqlCommandConstants.FOS_SYSAD_Insert_UserDetails, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
        //            transaction.Commit();
        //            return parameters.Get<int?>(SqlParameterConstants.PROSPECT_ERROR_CODE).GetValueOrDefault();
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }
        //}
    
}
}
