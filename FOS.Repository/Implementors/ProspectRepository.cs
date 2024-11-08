using Dapper;
using FOS.Models;
using FOS.Models.Constants;
using FOS.Models.Entities;
using FOS.Repository.Interfaces;
using System.Data;
using System.Data.SqlClient;
using static FOS.Models.Constants.Constants;

namespace FOS.Repository.Implementors
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="connectionString"></param>
    public class ProspectRepository(string connectionString) : IProspectRepository
    {
        /// <summary>
        /// Get List of Branch Location.
        /// <param name="companyId">Company Id</param>
        /// <param name="lobId">Lob Id</param>
        /// <param name="isActive"> Is Active</param>
        /// <param name="userId">User Id</param>
        /// </summary>
        /// <returns>List of <see cref="Location"/></returns>
        public async Task<List<Location>> GetBranchLocations(int? companyId, int? userId, int? lobId, bool? isActive)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var lstLocations = await connection.QueryAsync<Location>(SqlCommandConstants.FOS_GET_BRANCH_LIST, new
                {
                    CompanyId = companyId,
                    UserId = userId,
                    LobId = lobId,
                    IsActive = isActive.GetValueOrDefault()
                });
                return lstLocations.ToList();
            }
        }

        /// <summary>
        /// Get the Customer Details for a Prospect.
        /// </summary>
        /// <returns>List of <see cref="Prospect"/></returns>
        public async Task<Prospect> GetExistingProspectCustomerDetails(int? companyId, int? userId, string mobileNumber, string aadharNumber, string panNumber, int? prospectId)
        {
            var prospect = new Prospect();
            using (var connection = new SqlConnection(connectionString))
            {

                var parameters = new DynamicParameters();
                parameters.Add(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId, DbType.Int32, ParameterDirection.Input, 20);
                parameters.Add(SqlParameterConstants.PROSPECT_USER_ID, userId, DbType.Int32, ParameterDirection.Input, 20);
                parameters.Add(SqlParameterConstants.PROSPECT_MOBILE_NUMBER, mobileNumber, DbType.String, ParameterDirection.Input, 20);
                parameters.Add(SqlParameterConstants.PROSPECT_AADHAR_NUMBER, aadharNumber, DbType.String, ParameterDirection.Input, 30);
                parameters.Add(SqlParameterConstants.PROSPECT_PAN_NUMBER, panNumber, DbType.String, ParameterDirection.Input, 30);
                parameters.Add(SqlParameterConstants.PROSPECT_ID, prospectId, DbType.Int32, ParameterDirection.Input, 20);
                connection.Open();
                var dataReader = await connection.ExecuteReaderAsync(SqlCommandConstants.FOS_ORG_GET_EXISTING_PROPSECT_CUSTOMER_DETAILS, parameters, commandType: CommandType.StoredProcedure);
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        prospect = new Prospect
                        {
                            ProspectCode = dataReader.GetString(SqlColumnNames.ProspectCode),
                            ProspectId = dataReader.GetInt64(SqlColumnNames.ProspectId),
                            ProspectDate = dataReader.GetDateTime(SqlColumnNames.ProspectDate),
                            LocationId = dataReader.GetInt32(SqlColumnNames.LocationId),
                            LocationDescription = dataReader.GetString(SqlColumnNames.LocationDescription),
                            ProspectName = dataReader.GetString(SqlColumnNames.ProspectName),
                            ProspectTypeId = dataReader.GetInt32(SqlColumnNames.ProspectTypeId),
                            GenderId = dataReader.GetInt32(SqlColumnNames.GenderId),
                            GenderName = dataReader.GetString(SqlColumnNames.GenderName),
                            DateofBirth = dataReader.GetDateTime(SqlColumnNames.DateofBirth),
                            AlternateMobileNumber = dataReader.GetString(SqlColumnNames.AlternateMobileNumber),
                            Website = dataReader.GetString(SqlColumnNames.Website),
                            Email = dataReader.GetString(SqlColumnNames.Email)
                        };
                    }
                }
            }
            return prospect;
        }

        /// <summary>
        /// Get List of Prospect Lookup.
        /// </summary>
        /// <returns>List of <see cref="Lookup"/></returns>
        public async Task<List<Lookup>> GetProspectLookup()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var lstLookups = await connection.QueryAsync<Lookup>(SqlCommandConstants.FOS_GET_PROSPECT_LOOKUP_DATA);
                return lstLookups.ToList();
            }
        }

        /// <summary>
        /// Inserts a Prospect.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="locationId">Location Id</param>
        /// <param name="prospectTypeId">Prospect Type</param>
        /// <param name="customerId">Customer Id</param>
        /// <param name="customerCode">Customer Code</param>
        /// <param name="genderId">Gender Id</param>
        /// <param name="prospectName">Prospect Name</param>
        /// <param name="dob">Date of Birth</param>
        /// <param name="mobileNumber">Mobile Number</param>
        /// <param name="alternativeMobileNumber">Alternative Mobile Number</param>
        /// <param name="email">Email</param>
        /// <param name="website">Website</param>
        /// <param name="communicationAddress1">Communication Address 1</param>
        /// <param name="communicationAddress2">Communication Address 2</param>
        /// <param name="communicationLandmark">Communication Landmark</param>
        /// <param name="communicationCity">CIty</param>
        /// <param name="communicationStateId">State Id</param>
        /// <param name="communicationCountryId">Country Id</param>
        /// <param name="communicationPinCode">Pin Code</param>
        /// <param name="permanentAddress1">Permanent Address 1</param>
        /// <param name="permanentAddress2">Permanent Address 2</param>
        /// <param name="permanentLandmark">Permanent Address Landmark.</param>
        /// <param name="permanentCity">Permanent Address City</param>
        /// <param name="permanentStateId">Permanent Address State Id.</param>
        /// <param name="permanentCountryId">Permanent Address Country Id.</param>
        /// <param name="permanentPinCode">Permanent Address Pin Code.</param>
        /// <param name="aadharNumber">Aadhar Number</param>
        /// <param name="aadharImagePath">Aadhar Address Image Path</param>
        /// <param name="panNumber">PAN Number</param>
        /// <param name="panImagePath">PAN Number Image Path.</param>
        /// <param name="ProspectImagePath">Prospect Image Path.</param>
        /// <param name="createdBy">Created By User Id.</param>
        /// <param name="prospectId">Prospect Id</param>
        /// <param name="prospectCode">Prospect Code.</param>
        /// <param name="errorCode">Error Code</param>
        /// <returns>Boolean Value Indicating if the record got saved.</returns>
        public async Task<bool> InsertProspectDetails(int companyId, int locationId, int prospectTypeId, int customerId, string customerCode, int? genderId, string prospectName, DateTime? prospectDate, DateTime? dob, string mobileNumber, string? alternativeMobileNumber, string? email, string? website, string communicationAddress1, string communicationAddress2, string communicationLandmark, string communicationCity, int? communicationStateId, int? communicationCountryId, string communicationPinCode, string permanentAddress1, string permanentAddress2, string permanentLandmark, string permanentCity, int? permanentStateId, int? permanentCountryId, string permanentPinCode, string? aadharNumber, string? aadharImagePath, string panNumber, string panImagePath, string ProspectImagePath, int createdBy, long? prospectId, string prospectCode, int errorCode)
        {
            var prospect = new Prospect();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_LOCATION_ID, locationId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_DATE, prospectDate, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_TYPE_ID, prospectTypeId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_CUSTOMER_ID, customerId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_CUSTOMER_CODE, prospectCode, DbType.String, ParameterDirection.Input, 50);
                    parameters.Add(SqlParameterConstants.PROSPECT_GENDER_ID, genderId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_NAME, prospectName, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add(SqlParameterConstants.PROSPECT_DATEOFBIRTH, dob, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_MOBILE_NUMBER, mobileNumber, DbType.String, ParameterDirection.Input, 20);
                    parameters.Add(SqlParameterConstants.PROSPECT_ALTERNATE_MOBILENUMBER, alternativeMobileNumber, DbType.String, ParameterDirection.Input, 20);
                    parameters.Add(SqlParameterConstants.PROSPECT_EMAIL, email, DbType.String, ParameterDirection.Input, 40);
                    parameters.Add(SqlParameterConstants.PROSPECT_WEBSITE, website, DbType.String, ParameterDirection.Input, 50);
                    parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_ADDRESS1, communicationAddress1, DbType.String, ParameterDirection.Input, 150);
                    parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_ADDRESS2, communicationAddress2, DbType.String, ParameterDirection.Input, 150);
                    parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_LANDMARK, communicationLandmark, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_CITY, communicationCity, DbType.String, ParameterDirection.Input, 60);
                    parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_STATE_ID, communicationStateId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_COUNTRY_ID, communicationCountryId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_PINCODE, communicationPinCode, DbType.String, ParameterDirection.Input,20);
                    parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_ADDRESS1, permanentAddress1, DbType.String, ParameterDirection.Input, 150);
                    parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_ADDRESS2, permanentAddress2, DbType.String, ParameterDirection.Input, 150);
                    parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_LANDMARK, permanentLandmark, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_CITY, permanentCity, DbType.String, ParameterDirection.Input, 60);
                    parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_STATE_ID, permanentStateId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_COUNTRY_ID, permanentCountryId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_PINCODE, permanentPinCode, DbType.String, ParameterDirection.Input, 20);
                    parameters.Add(SqlParameterConstants.PROSPECT_AADHAR_NUMBER, aadharNumber, DbType.String, ParameterDirection.Input, 20);
                    parameters.Add(SqlParameterConstants.PROSPECT_AADHAR_IMPAGEPATH, aadharImagePath, DbType.String, ParameterDirection.Input, 150);
                    parameters.Add(SqlParameterConstants.PROSPECT_PAN_NUMBER, panNumber, DbType.String, ParameterDirection.Input, 20);
                    parameters.Add(SqlParameterConstants.PROSPECT_PAN_IMAGEPATH, panImagePath, DbType.String, ParameterDirection.Input, 150);
                    parameters.Add(SqlParameterConstants.PROSPECT_IMAGEPATH, ProspectImagePath, DbType.String, ParameterDirection.Input, 150);
                    parameters.Add(SqlParameterConstants.PROSPECT_CREATED_BY, createdBy, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_ID, prospectId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_CODE, prospectCode, DbType.String, ParameterDirection.Input, 500);
                    parameters.Add(SqlParameterConstants.PROSPECT_ERROR_CODE,dbType: DbType.Int32,direction: ParameterDirection.Output);
                    await connection.ExecuteAsync(SqlCommandConstants.FOS_ORG_INSERT_ProspectMaster, parameters, commandType: CommandType.StoredProcedure,transaction:transaction);
                    transaction.Commit();
                    return true;
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
