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

        public IEnumerable<DocumentCategory>? GetDocumentCategories(int companyId, int userId, int option)
        {
            IEnumerable<DocumentCategory>? documentCategoryList = null;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = SqlCommandConstants.FOS_SYSAD_DOCUMENT_LOOKUPDETAILS;
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.OPTION, option));
            var dataAdapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            dataAdapter.Fill(ds);

            if (ds != null)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    documentCategoryList = ds.Tables[0].Rows.Cast<DataRow>().Select(r => new DocumentCategory
                    {
                        DocumentCategoryDescription = Convert.ToString(r["Category_Description"]),
                        DocumentCategoryId = Convert.ToInt32(r["Document_Category_ID"])
                    });
                }
            }
            return documentCategoryList;
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
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SqlCommandConstants.FOS_ORG_GET_EXISTING_PROPSECT_CUSTOMER_DETAILS;
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_MOBILE_NUMBER, mobileNumber));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_AADHAR_NUMBER, aadharNumber));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_ID, prospectId));
                var dataAdapter = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                dataAdapter.Fill(ds);

                if (ds != null && ds.Tables.Count>0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        var dr = ds.Tables[0].Rows[0];
                        prospect = new Prospect
                        {
                            ProspectCode = Convert.ToString(dr[SqlColumnNames.ProspectCode]),
                            ProspectId = Convert.ToInt64(dr[SqlColumnNames.ProspectId]),
                            ProspectDate = Convert.ToDateTime(dr[SqlColumnNames.ProspectDate]),
                            LocationId = Convert.ToInt32(dr[SqlColumnNames.LocationId]),
                            LocationDescription = Convert.ToString(dr[SqlColumnNames.LocationDescription]),
                            ProspectName = Convert.ToString(dr[SqlColumnNames.ProspectName]),
                            ProspectTypeId = Convert.ToInt32(dr[SqlColumnNames.ProspectTypeId]),
                            GenderId = Convert.ToInt32(dr[SqlColumnNames.GenderId]),
                            GenderName = Convert.ToString(dr[SqlColumnNames.GenderName]),
                            DateofBirth = Convert.ToDateTime(dr[SqlColumnNames.DateofBirth]),
                            AlternateMobileNumber = Convert.ToString(dr[SqlColumnNames.AlternateMobileNumber]),
                            Website = Convert.ToString(dr[SqlColumnNames.Website]),
                            Email = Convert.ToString(dr[SqlColumnNames.Email])
                        };
                    }

                    if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    {
                        var communicationAddress = ds.Tables[1].Rows.Cast<DataRow>().FirstOrDefault(dr => dr.Field<int>("Address_LookupValue_ID") == 1);
                        var permanentAddress = ds.Tables[1].Rows.Cast<DataRow>().FirstOrDefault(dr => dr.Field<int>("Address_LookupValue_ID") == 2);
                        if (communicationAddress != null)
                            prospect.CommunicationAddress = new Address
                            {
                                AddressLine1 = Convert.ToString(communicationAddress["Address_1"]),
                                AddressLine2 = Convert.ToString(communicationAddress["Address_2"]),
                                City = Convert.ToString(communicationAddress["City"]),
                                CountryId = Convert.ToInt32(communicationAddress["Country_ID"]),
                                Landmark = Convert.ToString(communicationAddress["Address_Landmark"]),
                                Pincode = Convert.ToString(communicationAddress["Pincode"]),
                                StateId = Convert.ToInt32(communicationAddress["State_ID"]),
                            };
                        if (permanentAddress != null)
                            prospect.PermanentAddress = new Address
                            {
                                AddressLine1 = Convert.ToString(permanentAddress["Address_1"]),
                                AddressLine2 = Convert.ToString(permanentAddress["Address_2"]),
                                City = Convert.ToString(permanentAddress["City"]),
                                CountryId = Convert.ToInt32(permanentAddress["Country_ID"]),
                                Landmark = Convert.ToString(permanentAddress["Address_Landmark"]),
                                Pincode = Convert.ToString(permanentAddress["Pincode"]),
                                StateId = Convert.ToInt32(permanentAddress["State_ID"]),
                            };


                        if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                        {
                            var aadharDocument = ds.Tables[2].Rows.Cast<DataRow>().FirstOrDefault(s => s.Field<int>("ProspectDocument_ID") == 1);
                            var panDocument = ds.Tables[2].Rows.Cast<DataRow>().FirstOrDefault(s => s.Field<int>("ProspectDocument_ID") == 11);
                            var prospectDocument = ds.Tables[2].Rows.Cast<DataRow>().FirstOrDefault(s => s.Field<int>("ProspectDocument_ID") == 21);
                            if (aadharDocument != null)
                            {
                                prospect.AadharImagePath = Convert.ToString(aadharDocument["Upload_Path"]);
                                prospect.AadharNumber = Convert.ToString(aadharDocument["Document_IdentityValue"]);
                            }
                            if (panDocument != null)
                            {
                                prospect.PanNumber = Convert.ToString(panDocument["Document_IdentityValue"]);
                                prospect.PanNumberImagePath = Convert.ToString(panDocument["Upload_Path"]);
                            }
                            if (prospectDocument != null)
                                prospect.ProspectImagePath = Convert.ToString(prospectDocument["Upload_Path"]);
                        }
                    }
                }
                return prospect;
            }
        }

        public IEnumerable<FieldExecutive>? GetFieldExecutives(int companyId, int userId, string prefix)
        {
            IEnumerable<FieldExecutive>? fieldExecutiveList = null;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = SqlCommandConstants.FOS_ORG_GET_USERNAMEAGT;
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PREFIX_TEXT, prefix));
            var dataAdapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            dataAdapter.Fill(ds);

            if (ds != null)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    fieldExecutiveList = ds.Tables[0].Rows.Cast<DataRow>().Select(r => new FieldExecutive
                    {
                        FieldExecutiveName = Convert.ToString(r["UserName"]),
                        FieldExecutiveId = Convert.ToInt32(r["UserID"])
                    });
                }
            }
            return fieldExecutiveList;
        }

        /// <summary>
        /// Get List of LOB.
        /// </summary>
        /// <returns>List of <see cref="LineOfBusiness"/></returns>
        public IEnumerable<LineOfBusiness>? GetLineofBusiness(int companyId, int userId)
        {
            IEnumerable<LineOfBusiness>? lobList = null;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = SqlCommandConstants.FOS_GET_LOB_LIST;
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
            var dataAdapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            dataAdapter.Fill(ds);

            if (ds != null)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    lobList = ds.Tables[0].Rows.Cast<DataRow>().Select(r => new LineOfBusiness
                    {
                        LineOfBusinessName = Convert.ToString(r["LOB_NAME"]),
                        LineOfBusinessId = Convert.ToInt32(r["LOB_ID"])
                    });
                }
            }
            return lobList;
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
        /// Get List of States.
        /// </summary>
        /// <returns>List of <see cref="Lookup"/></returns>
        public async Task<List<Lookup>> GetStateLookups()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var lstLookups = await connection.QueryAsync<Lookup>(SqlCommandConstants.FOS_GET_STATE_LOOKUP);
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
        public async Task<int> InsertProspectDetails(int companyId, int locationId, int prospectTypeId, int customerId, string customerCode, int? genderId, string prospectName, DateTime? prospectDate, DateTime? dob, string mobileNumber, string? alternativeMobileNumber, string? email, string? website, string communicationAddress1, string communicationAddress2, string communicationLandmark, string communicationCity, int? communicationStateId, int? communicationCountryId, string communicationPinCode, string permanentAddress1, string permanentAddress2, string permanentLandmark, string permanentCity, int? permanentStateId, int? permanentCountryId, string permanentPinCode, string? aadharNumber, string? aadharImagePath, string panNumber, string panImagePath, string ProspectImagePath, int createdBy, long? prospectId, string prospectCode, int errorCode)
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
                    parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_PINCODE, communicationPinCode, DbType.String, ParameterDirection.Input, 20);
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
                    parameters.Add(SqlParameterConstants.PROSPECT_ERROR_CODE, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await connection.ExecuteAsync(SqlCommandConstants.FOS_ORG_INSERT_ProspectMaster, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                    transaction.Commit();
                    return parameters.Get<int?>(SqlParameterConstants.PROSPECT_ERROR_CODE).GetValueOrDefault();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<List<ProspectExportData>> GetProspectDataForExport()
        {
            using var connection = new SqlConnection(connectionString);
            var lstProspectData = await connection.QueryAsync<ProspectExportData>(SqlCommandConstants.FOS_PROSPECT_EXPORT_QUERY);
            return lstProspectData.ToList();
        }
    }
}
