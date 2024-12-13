using Dapper;
using FOS.Models;
using FOS.Models.Constants;
using FOS.Models.Entities;
using FOS.Repository.Interfaces;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using static FOS.Models.Constants.Constants;

namespace FOS.Repository.Implementors
{
    /// <summary>
    /// Repository for processing Leads Information.
    /// </summary>
    /// <param name="connectionString">Connection String to the DB.</param>
    public class LeadsRepository(string connectionString) : ILeadsRepository
    {
        /// <summary>
        /// Gets the Asset Lookup Data.
        /// </summary>
        /// <returns>Instance of type <see cref="List{Lookup}"/></returns>
        public IEnumerable<Lookup>? GetAssetLookup(int companyId, int userId)
        {
            IEnumerable<Lookup>? assetLookupList = null;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = SqlCommandConstants.FOS_ORG_GET_ASSETLOOKUP;
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
            var dataAdapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            dataAdapter.Fill(ds);

            if (ds != null)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    assetLookupList = ds.Tables[0].Rows.Cast<DataRow>().Select(r => new Lookup
                    {
                        LookupTypeDescription = Convert.ToString(r["Asset_Category_Type"]),
                        LookupValueDescription = Convert.ToString(r["Asset_Category_Description"]),
                        LookupValueId = Convert.ToInt32(r["Asset_Category_ID"])
                    });
                }
            }
            return assetLookupList;
        }

        /// <summary>
        /// Gets Lead Details.
        /// </summary>
        /// <returns>Instance of type <see cref="Lead"/></returns>
        public Lead? GetLeadDetails(int companyId, int userId, int leadId, string vehicleNumber, string leadNumber)
        {
            var lead = new Lead { UserId = userId, CompanyId = companyId };
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = SqlCommandConstants.FOS_ORG_GET_LEADDETAILS;
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.LEAD_ID, leadId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.LEAD_VEHICLE_NUMBER, vehicleNumber));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.LEAD_NUMBER, leadNumber));
            var dataAdapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            dataAdapter.Fill(ds);

            if (ds != null)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Columns.Count == 2)
                {
                    return null;
                }

                lead = FormLeadProspectiveDetails(lead, ds);

                if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    var dr = ds.Tables[1].Rows[0];
                    FormLeadHeaderObject(lead, dr);

                }
                if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                {
                    var drRow = ds.Tables[2].Rows.Cast<DataRow>().FirstOrDefault();
                    FormLeadAssetDetailObject(lead, drRow);
                }
                if (ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
                {
                    var drRow = ds.Tables[3].Rows.Cast<DataRow>().FirstOrDefault();
                    FormIndividualObject(lead, drRow);
                }
                if (ds.Tables[4] != null && ds.Tables[4].Rows.Count > 0)
                {
                    var drRow = ds.Tables[4].Rows.Cast<DataRow>().FirstOrDefault();
                    FormNonIndividualObject(lead, drRow);
                }

                if (ds.Tables[5] != null && ds.Tables[5].Rows.Count > 0)
                {
                    lead.Guarantors = new List<LeadGuarantor>();
                    ds.Tables[5].Rows.Cast<DataRow>().ToList().ForEach((drRow) =>
                    {
                        lead.Guarantors.Add(AddLeadGuaranter(companyId, userId, drRow));
                    });
                }
            }
            return lead;

        }

        /// <summary>
        /// Gets a List of Lookups for Lead Generation Screen.
        /// </summary>
        /// <returns>Instance of type <see cref="List{Lookup}"/></returns>
        public IEnumerable<Lookup>? GetLeadGenerationLookup(int companyId, int userId)
        {
            IEnumerable<Lookup>? leadGenertionLookup = null;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = SqlCommandConstants.FOS_GET_LOOKUPS_FOR_LEAD_SCREEN;
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
            var dataAdapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            dataAdapter.Fill(ds);

            if (ds != null)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    leadGenertionLookup = ds.Tables[0].Rows.Cast<DataRow>().Select(r => new Lookup
                    {
                        LookupTypeId = Convert.ToInt32(r["LookupType_ID"]),
                        LookupTypeDescription = Convert.ToString(r["LookupType_Description"]),
                        LookupValueId = Convert.ToInt32(r["LookupValue_ID"]),
                        LookupValueDescription = Convert.ToString(r["LookupValue_Description"]),
                    });
                }
            }
            return leadGenertionLookup;
        }

        /// <summary>
        /// Gets the Prospect Detail.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id</param>
        /// <param name="mobileNumber">Mobile Number</param>
        /// <param name="aadharNumber">Aadhar Number</param>
        /// <param name="panNumber">Pan Number</param>
        /// <returns>Instance of type <see cref="LeadProspectDetail"/></returns>
        public LeadProspectDetail GetProspectDetailsForLead(int companyId, int userId, string mobileNumber, string aadharNumber, string panNumber)
        {
            var prospectDetail = new LeadProspectDetail();
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = SqlCommandConstants.FOS_ORG_GET_LEADPROSPECTDETAILS;
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_MOBILE_NUMBER, mobileNumber));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_AADHAR_NUMBER, aadharNumber));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_PAN_NUMBER, panNumber));
            var dataAdapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            dataAdapter.Fill(ds);

            if (ds != null)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    var dRow = ds.Tables[0].Rows.Cast<DataRow>().FirstOrDefault();
                    if (dRow != null)
                    {
                        prospectDetail = new LeadProspectDetail
                        {
                            LeadType = Convert.ToString(dRow["Lead_Type"]),
                            LocationId = Convert.ToInt32(dRow["Location_ID"]),
                            LocationName = Convert.ToString(dRow["Location_Name"]),
                            MobileNumber = Convert.ToString(dRow["Mobile_Number"]),
                            ProspectAddress = Convert.ToString(dRow["Prospect_Address"]),
                            ProspectId = Convert.ToInt32(dRow["Prospect_ID"]),
                            ProspectName = Convert.ToString(dRow["Prospect_Name"]),
                            ProspectTypeDescription = Convert.ToString(dRow["ProspectType_Description"]),
                            ProspectTypeId = Convert.ToInt32(dRow["ProspectType_ID"]),
                        };
                    }
                }
            }
            return prospectDetail;
        }

        /// <summary>
        /// Gets a List of Statuses for Lead Translander Screen.
        /// </summary>
        /// <returns>Instance of type <see cref="List{Status}"/></returns>
        public async Task<IEnumerable<LeadStatus>> GetStatusesForLead()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<LeadStatus>(SqlCommandConstants.FOS_ORG_GETLEADSTATUS);
        }

        /// <summary>
        /// Inserts the Lead Guaantor Data.
        /// </summary>
        public async Task<bool> InsertGuarantorData(Lead lead)
        {
            var createdCount = 0;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();
            if (lead != null && lead.Guarantors != null && lead.Guarantors.Count > 0)
            {
                foreach (var guarantor in lead.Guarantors)
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add(SqlParameterConstants.LEAD_ID, lead.Header.LeadId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.LEAD_GUARANTORTYPE_ID, guarantor.GuarantorTypeLookupValueId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.LEAD_RELATIONSHIP_ID, guarantor.GuarantorRelationshipLookupValueId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_COMPANY_ID, lead.CompanyId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_LOCATION_ID, lead.LocationId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_DATE, lead.Header.LeadDate, DbType.DateTime, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_TYPE_ID, lead.Header.LeadTypeLookupValueId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_CUSTOMER_ID, lead.CustomerId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_CUSTOMER_CODE, lead.CustomerCode, DbType.String, ParameterDirection.Input, 50);
                        parameters.Add(SqlParameterConstants.PROSPECT_GENDER_ID, guarantor.GenderId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.LEAD_GURANTOR_NAME, guarantor.GuarantorName, DbType.String, ParameterDirection.Input, 100);
                        parameters.Add(SqlParameterConstants.PROSPECT_DATEOFBIRTH, guarantor?.GuaranterDateOfBirth, DbType.DateTime, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_MOBILE_NUMBER, guarantor.MobileNumber, DbType.String, ParameterDirection.Input, 20);
                        parameters.Add(SqlParameterConstants.PROSPECT_ALTERNATE_MOBILENUMBER, guarantor.AlternateMobileNumber, DbType.String, ParameterDirection.Input, 20);
                        parameters.Add(SqlParameterConstants.PROSPECT_EMAIL, guarantor.Email, DbType.String, ParameterDirection.Input, 40);
                        parameters.Add(SqlParameterConstants.PROSPECT_WEBSITE, guarantor.Website, DbType.String, ParameterDirection.Input, 50);
                        parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_ADDRESS1, guarantor.CommunicationAddress?.AddressLine1, DbType.String, ParameterDirection.Input, 150);
                        parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_ADDRESS2, guarantor.CommunicationAddress?.AddressLine2, DbType.String, ParameterDirection.Input, 150);
                        parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_LANDMARK, guarantor.CommunicationAddress?.Landmark, DbType.String, ParameterDirection.Input, 150);
                        parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_CITY, guarantor.CommunicationAddress?.City, DbType.String, ParameterDirection.Input, 60);
                        parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_STATE_ID, guarantor.CommunicationAddress?.StateId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_COUNTRY_ID, guarantor.CommunicationAddress?.CountryId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_COMMUNICATION_PINCODE, guarantor.CommunicationAddress?.Pincode, DbType.String, ParameterDirection.Input, 20);
                        parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_ADDRESS1, guarantor.PermanentAddress?.AddressLine1, DbType.String, ParameterDirection.Input, 150);
                        parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_ADDRESS2, guarantor.PermanentAddress?.AddressLine2, DbType.String, ParameterDirection.Input, 150);
                        parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_LANDMARK, guarantor.PermanentAddress?.Landmark, DbType.String, ParameterDirection.Input, 150);
                        parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_CITY, guarantor.PermanentAddress?.City, DbType.String, ParameterDirection.Input, 60);
                        parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_STATE_ID, guarantor.PermanentAddress?.StateId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_COUNTRY_ID, guarantor.PermanentAddress?.CountryId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_PERMANENT_PINCODE, guarantor.PermanentAddress?.Pincode, DbType.String, ParameterDirection.Input, 20);
                        parameters.Add(SqlParameterConstants.PROSPECT_AADHAR_NUMBER, guarantor.AadharNumber, DbType.String, ParameterDirection.Input, 20);
                        parameters.Add(SqlParameterConstants.PROSPECT_AADHAR_IMPAGEPATH, guarantor.AadharImagePath, DbType.String, ParameterDirection.Input, 150);
                        parameters.Add(SqlParameterConstants.PROSPECT_PAN_NUMBER, guarantor.PanNumber, DbType.String, ParameterDirection.Input, 20);
                        parameters.Add(SqlParameterConstants.PROSPECT_PAN_IMAGEPATH, guarantor.PanImagePath, DbType.String, ParameterDirection.Input, 150);
                        parameters.Add(SqlParameterConstants.LEAD_GURANTOR_IMAGEPATH, guarantor.GuarantorImagePath, DbType.String, ParameterDirection.Input, 150);
                        parameters.Add(SqlParameterConstants.PROSPECT_CREATED_BY, lead.UserId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_ID, guarantor.ProspectId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add(SqlParameterConstants.PROSPECT_CODE, guarantor.ProspectCode, DbType.String, ParameterDirection.Output, 500);
                        parameters.Add(SqlParameterConstants.PROSPECT_ERROR_CODE, dbType: DbType.Int32, direction: ParameterDirection.Output);
                        parameters.Add(SqlParameterConstants.LEAD_GUARANTOR_AMOUNT, guarantor.GuarantorAmount, DbType.String, ParameterDirection.Input, 500);
                        await connection.ExecuteAsync(SqlCommandConstants.FOS_ORG_INSERT_LEADGUARANTORDETAILS, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                        transaction.Commit();
                        var erorCode = parameters.Get<int?>(SqlParameterConstants.PROSPECT_ERROR_CODE).GetValueOrDefault();
                        if (erorCode == 0)
                            createdCount += 1;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return createdCount == lead?.Guarantors?.Count;
        }

        /// <summary>
        /// Inserts the Lead Header cum Followup Data.
        /// </summary>
        /// <param name="lead">Lead Object.</param>
        /// <returns>Instance of type <see cref="LeadHeader"/></returns>
        public async Task<LeadHeader> InsertLeadDetails(int companyId, int userId, int locationId, LeadHeader leadHeader)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();
            if (leadHeader != null)
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add(name: SqlParameterConstants.PROSPECT_USER_ID, value: userId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    parameters.Add(name: SqlParameterConstants.PROSPECT_LOCATION_ID, value: locationId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    parameters.Add(name: SqlParameterConstants.PROSPECT_COMPANY_ID, value: companyId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    parameters.Add(name: SqlParameterConstants.LEAD_DATE, value: leadHeader?.LeadDate, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                    parameters.Add(name: SqlParameterConstants.PROSPECT_ID, value: leadHeader?.ProspectId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    parameters.Add(name: SqlParameterConstants.LEAD_VEHICLE_NUMBER, value: leadHeader?.VehicleRegistrationNumber, dbType: DbType.String, direction: ParameterDirection.Input, 20);
                    parameters.Add(name: SqlParameterConstants.LEAD_NUMBER, dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    parameters.Add(name: SqlParameterConstants.LEAD_ID, dbType: DbType.Int64, direction: ParameterDirection.Output);
                    parameters.Add(name: SqlParameterConstants.VALIDATION_CODE, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add(name: SqlParameterConstants.VALIDATION_MESSAGE, dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
                    await connection.ExecuteAsync(SqlCommandConstants.FOS_ORG_INSERT_LEADDETAILS, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                    transaction.Commit();
                    var validationCode = parameters.Get<int?>(SqlParameterConstants.VALIDATION_CODE).GetValueOrDefault();
                    var validationMessage = parameters.Get<string?>(SqlParameterConstants.VALIDATION_MESSAGE);
                    var leadNumber = parameters.Get<string?>(SqlParameterConstants.LEAD_NUMBER);
                    var leadId = parameters.Get<long>(SqlParameterConstants.LEAD_ID);

                    if (leadHeader != null)
                    {
                        leadHeader.LeadId = leadId;
                        leadHeader.LeadNumber = leadNumber!;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return leadHeader!;
        }

        /// <summary>
        /// Inserts the Lead Header.
        /// </summary>
        /// <param name="lead">Lead Object.</param>
        /// <returns>Instance of type <see cref="int"/></returns>
        public async Task<int> InsertLeadGenerationHeader(Lead lead)
        {
            int errorCode = 0;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();
            if (lead != null && lead.Header != null && lead.Asset != null)
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add(SqlParameterConstants.LEAD_ID, lead.Header.LeadId, DbType.Int64, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_COMPANY_ID, lead.CompanyId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_CREATED_BY, lead.UserId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LOB_ID, lead.LobId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_FINANCE_AMONT, lead.Header.FinanceAmount, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_DOCUMENT_ID, lead.Header.DocumentCategoryId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_TENURE, lead.Header.Tenure, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_TENURE_TYPE_ID, lead.Header.TenureLookupTypeId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_RATE, lead.Header?.Rate, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_SALES_PERSON_ID, lead.Header?.SalesPersonId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_REPAYMENT_FREQUENCY_ID, lead.Header?.RepaymentFrequencyLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_LEAVE_PERIOD, lead.Header?.LeavePeriod, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_ASSET_CLASS_ID, lead.Asset.AssetClassId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_ASSET_NAME_ID, lead.Asset?.AssetMakeId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_ASSET_TYPE_ID, lead.Asset?.AssetTypeId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.ASSET_MODEL, lead.Asset?.AssetModelId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_ENGINE_NUMBER, lead.Asset?.EngineNumber, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_CHASSIS_NUMBER, lead.Asset?.ChasisNumber, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_SERIAL_NUMBER, lead.Asset?.SerialNumber, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_OWNERSHIP_ID, lead.Asset?.OwnershipLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_MODEL, lead.Asset?.Model, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_VEHICLE_TYPE_ID, lead.Asset?.VehicleTypeLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_TAX_TYPE_ID, lead.Asset?.TaxTypeLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_FUEL_TYPE_ID, lead.Asset?.FuelTypeLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_ERROR_CODE, DbType.Int32, direction: ParameterDirection.Output);
                    await connection.ExecuteAsync(SqlCommandConstants.FOS_ORG_INSERT_LEADGENERATION_HEADER, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                    transaction.Commit();
                    errorCode = parameters.Get<int?>(SqlParameterConstants.PROSPECT_ERROR_CODE).GetValueOrDefault();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return errorCode;
        }

        /// <summary>
        /// Inserts the Lead Individual Data.
        /// </summary>
        /// <param name="lead">Lead Object.</param>
        /// <returns>Instance of type <see cref="int"/></returns>
        public async Task<int> InsertLeadIndividual(int companyId, int userId, int leadId, LeadIndividualDetail leadIndividualDetails)
        {
            int errorCode = 0;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();
            if (leadIndividualDetails != null)
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add(SqlParameterConstants.LEAD_ID, leadId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_CREATED_BY, userId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_HOUSE_TYPE_ID, leadIndividualDetails.HouseLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_FLOOR_NUMBER, leadIndividualDetails.DoorFloorNumber, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_HOUSESTATUS_ID, leadIndividualDetails.HouseStatusLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_HOUSE_RENTAL_AMOUNT, leadIndividualDetails.HouseRentalAmount, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_MARITIALSTATUS_ID, leadIndividualDetails.MaritialStatusLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_FATHER_NAME, leadIndividualDetails.FatherName, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_MOTHER_NAME, leadIndividualDetails.MotherName, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_EMPLOYMENT_ID, leadIndividualDetails.EmploymentLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_MONTHLY_SALARY, leadIndividualDetails.MonthlySalary, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_SPOUSEEMPLOYMENT_ID, leadIndividualDetails.SpouseEmploymentLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_SPOUSE_MONTHLY_SALARY, leadIndividualDetails.SpouseSalary, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_ADULT_DEPENDENTS, leadIndividualDetails.AdultDependents, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_CHILD_DEPENDENTS, leadIndividualDetails.ChildDependents, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_OWNED_TWO_WHEELER, leadIndividualDetails.OwnTwoWheeler, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_OWNED_FOUR_WHEELER, leadIndividualDetails.OwnFourWheeler, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_OWNED_HEAVY_VEHICLES, leadIndividualDetails.OwnHeavyVehicle, DbType.Boolean, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_EXISTING_LOAN_ALIVE, leadIndividualDetails.ExistingLoanCount, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_EXISITNG_LOANS_MONTHLY_EMI, leadIndividualDetails.ExistingLoanEmi, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_SPOUSENAME, leadIndividualDetails.SpouseName, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_ERROR_CODE, DbType.Int32, direction: ParameterDirection.Output);
                    await connection.ExecuteAsync(SqlCommandConstants.FOS_ORG_INSERT_LEADINDIVIDUALDETAILS, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                    transaction.Commit();
                    errorCode = parameters.Get<int?>(SqlParameterConstants.PROSPECT_ERROR_CODE).GetValueOrDefault();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return errorCode;
        }

        /// <summary>
        /// Inserts the Lead Non Individual Data.
        /// </summary>
        /// <returns>Instance of type <see cref="int"/></returns>
        public async Task<int> InsertLeadNonIndividual(int userId, int leadId, LeadNonIndividualDetail leadNonIndividual)
        {
            int errorCode = 0;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();
            if (leadNonIndividual != null)
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add(SqlParameterConstants.LEAD_ID, leadId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_CREATED_BY, userId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_PUBLIC_CLOSELY_ID, leadNonIndividual.PublicCloselyLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_DIRECTOR_PARTHERS_COUNT, leadNonIndividual.DirectorCount, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_LISTED_EXCHANGE, leadNonIndividual.ListedExchange, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_PAID_UP_CAPITAL, leadNonIndividual.PaidUpCapital, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_FACE_VALUE_SHARE, leadNonIndividual.FaceValueShare, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_BOOK_VALUE_SHARE, leadNonIndividual.BookValueShare, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_BUSINESS_PROFILE, leadNonIndividual.BusinessProfile, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_GEOGRAPHICAL_COVERAGE, leadNonIndividual.GeographicalCoverage, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_BRANCH_COUNT, leadNonIndividual.BranchCount, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_GOVERNMENT_INSTITUTION_PARTICIPATION_ID, leadNonIndividual.IndustryLookupValueId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_PROMOTER_STAKE, leadNonIndividual.PromoterStakePercentage, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_JV_PARTNER_NAME, leadNonIndividual.JVPartnerName, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_JV_PARTNER_PERCENTAGE, leadNonIndividual.JVPartnerPercentage, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_CEO_NAME, leadNonIndividual.CeoName, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_CEO_DATEOFBIRTH, leadNonIndividual.CeoDateofBirth, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_CEO_WEDDING_DATE, leadNonIndividual.CeoWeddingDate, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_EXPERIENCE, leadNonIndividual.CeoExperience, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_RESIDENTIAL_ADDRESS, leadNonIndividual.ResidentialAddress, DbType.String, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_ERROR_CODE, DbType.Int32, direction: ParameterDirection.Output);
                    await connection.ExecuteAsync(SqlCommandConstants.FOS_ORG_INSERT_LEADNONINDIVIDUALDETAILS, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                    transaction.Commit();
                    errorCode = parameters.Get<int?>(SqlParameterConstants.PROSPECT_ERROR_CODE).GetValueOrDefault();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return errorCode;
        }

        /// <summary>
        /// Gets the Leads Records for the Translander Page.
        /// </summary>
        /// <returns>Instance of type <see cref="LeadsTranslander"/></returns>
        public LeadsTranslander GetLeadsForTranslander(int? companyId, int? userId, string? status, string? leadNumber, string? vehicleNumber, int? currentPage, int? pageSize, string? searchValue)
        {
            var totalRecords = 0;
            var leadHeaders = new List<LeadHeader>();
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = SqlCommandConstants.FOS_ORG_GET_LEADTRANSLANDER;
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.STATUS, status));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.LEAD_VEHICLE_NUMBER, vehicleNumber));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.LEAD_NUMBER, leadNumber));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PAGE_SIZE, pageSize));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.CURRENT_PAGE, currentPage));
            cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.SEARCH_VALUE, searchValue));
            var totalRecordParameter = new SqlParameter(SqlParameterConstants.TOTAL_RECORDS, SqlDbType.Int);
            totalRecordParameter.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(totalRecordParameter);
            var dataAdapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            dataAdapter.Fill(ds);

            if (ds != null && ds.Tables.Count > 0)
            {
                totalRecords = Convert.ToInt32(ds.Tables[0].Rows[0]["Total_Records"]);
                leadHeaders = ds.Tables[0].Rows.Cast<DataRow>().Select(r => new LeadHeader
                {
                    LeadId = r.Field<long>("LEAD_ID"),
                    LeadNumber = r.Field<string>("LEAD_NUMBER")!,
                    LeadDate = r.Field<DateTime>("LEAD_DATE")!,
                    ProspectId = r.Field<long?>("PROSPECT_ID")!,
                    LeadCurrentStatusDescription = r.Field<string>("STATUS")!,
                }).ToList();
            }
            return new LeadsTranslander { Leads = leadHeaders, TotalRecords = totalRecords };
        }

        /// <summary>
        /// Forms the Prospect Detail Object.
        /// </summary>
        /// <param name="lead">Lead Object.</param>
        /// <param name="ds">DataSet object.</param>
        /// <returns></returns>
        private Lead FormLeadProspectiveDetails(Lead lead, DataSet ds)
        {
            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                var dr = ds.Tables[0].Rows[0];
                lead = new Lead
                {
                    Header = new LeadHeader
                    {
                        LeadId = Convert.ToInt32(dr["Lead_ID"]),
                        LeadNumber = Convert.ToString(dr["Lead_Number"])!,
                        LeadDate = Convert.ToDateTime(dr["Lead_Date"])!,
                        VehicleRegistrationNumber = Convert.ToString(dr["Vehicle_Number"])!,
                        LeadTypeLookupValueId = Convert.ToInt32(dr["Lead_Type"])!,
                        ProspectId = Convert.ToInt32(dr["Prospect_ID"]),
                    },
                    LeadProspectDetail = new LeadProspectDetail
                    {
                        ProspectId = Convert.ToInt32(dr["Prospect_ID"]),
                        LocationId = Convert.ToInt32(dr["Location_ID"]),
                        LocationName = Convert.ToString(dr["Location_Name"])!,
                        ProspectName = Convert.ToString(dr["Prospect_Name"])!,
                        LeadType = Convert.ToString(dr["Lead_Type"])!,
                        LeadTypeName = Convert.ToString(dr["Lead_TypeName"])!,
                        ProspectAddress = Convert.ToString(dr["Prospect_Address"])!,
                        MobileNumber = Convert.ToString(dr["Mobile_Number"])!,
                        VehicleNumber = Convert.ToString(dr["Vehicle_Number"])!,
                        AadharNumber = Convert.ToString(dr["Aadhar_Number"])!,
                        PanNumber = Convert.ToString(dr["PAN_Number"])!,
                        ProspectTypeId = Convert.ToInt32(dr["ProspectType_ID"])!,
                        ProspectTypeDescription = Convert.ToString(dr["ProspectType_Description"])!,
                    }
                };
            }

            return lead;
        }

        /// <summary>
        /// Forms the Lead Header Object.
        /// </summary>
        /// <param name="lead">Lead Object</param>
        /// <param name="dr">Data Row containing the Prospect details.</param>
        private void FormLeadHeaderObject(Lead? lead, DataRow dr)
        {
            if (lead != null)
            {
                lead.LobId = Convert.ToInt32(dr["LOB_ID"])!;
                lead.LobDescription = Convert.ToString(dr["LOB_Description"])!;
                if (lead.Header != null)
                {
                    lead.Header.FinanceAmount = Convert.ToDecimal(dr["Finance_Amount"])!;
                    lead.Header.Tenure = Convert.ToInt32(dr["Tenure"])!;
                    lead.Header.TenureLookupValueId = Convert.ToInt32(dr["TenureType_ID"])!;
                    lead.Header.TenureLookupTypeDescription = Convert.ToString(dr["TenureType_Description"])!;
                    lead.Header.Rate = Convert.ToDecimal(dr["Rate"])!;
                    lead.Header.RepaymentFrequencyLookupValueId = Convert.ToInt32(dr["RepaymentFrequency_ID"])!;
                    lead.Header.RepaymentFrequencyDescription = Convert.ToString(dr["RepaymentFrequency_Description"])!;
                    lead.Header.LeavePeriod = Convert.ToInt32(dr["Leave_Period"])!;
                    lead.Header.FieldExecutiveId = Convert.ToInt32(dr["FieldExecutive_ID"])!;
                    lead.Header.FieldExecutiveName = Convert.ToString(dr["FieldExecutive_Name"])!;
                    lead.Header.DocumentCategoryId = Convert.ToInt32(dr["DocumentID"])!;
                    lead.Header.DocumentName = Convert.ToString(dr["DocumentDescription"])!;
                }
            }


        }

        /// <summary>
        /// Forms the Lead Individual Object.
        /// </summary>
        /// <param name="lead">Lead Object.</param>
        /// <param name="drRow">Data Row Object containing the Lead Individual Details.</param>
        private void FormIndividualObject(Lead? lead, DataRow? drRow)
        {
            if (lead != null && drRow != null)
            {
                lead.IndividualDetail = new LeadIndividualDetail
                {
                    FatherName = Convert.ToString(drRow["Father_Name"])!,
                    MotherName = Convert.ToString(drRow["Mother_Name"])!,
                    MaritialStatusLookupValueId = Convert.ToInt32(drRow["Maritial_Status_ID"])!,
                    MaritialStatusDescription = Convert.ToString(drRow["Maritial_Status_Description"])!,
                    SpouseName = Convert.ToString(drRow["Spouse_Name"])!,
                    EmploymentLookupValueId = Convert.ToInt32(drRow["EmploymentType_ID"])!,
                    EmploymentTypeDescription = Convert.ToString(drRow["EmploymentType_Description"])!,
                    MonthlySalary = Convert.ToDecimal(drRow["Hirer_Salary"])!,
                    SpouseEmploymentLookupValueId = Convert.ToInt32(drRow["Spouse_EmploymentType_ID"])!,
                    SpouseEmploymentDescription = Convert.ToString(drRow["Spouse_EmploymentType_Description"])!,
                    SpouseSalary = Convert.ToDecimal(drRow["Spose_Salary"])!,
                    AdultDependents = Convert.ToInt32(drRow["Adult_Dependent"])!,
                    ChildDependents = Convert.ToInt32(drRow["Child_Dependent"])!,
                    HouseLookupValueId = Convert.ToInt32(drRow["HouseType_ID"])!,
                    HouseTypeDescription = Convert.ToString(drRow["HouseType_Description"])!,
                    DoorFloorNumber = Convert.ToString(drRow["Floor_Number"])!,
                    HouseStatusLookupValueId = Convert.ToInt32(drRow["HouseStatus_ID"])!,
                    HouseStatusDescription = Convert.ToString(drRow["HouseStatus_Description"])!,
                    HouseRentalAmount = Convert.ToDecimal(drRow["Rental_Amount"])!,
                    OwnTwoWheeler = Convert.ToInt32(drRow["Owned_TwoWheeler"])!,
                    OwnFourWheeler = Convert.ToInt32(drRow["Owned_FourWheeler"])!,
                    OwnHeavyVehicle = Convert.ToInt32(drRow["Owned_Heavyvehicle"]),
                    ExistingLoanCount = Convert.ToInt32(drRow["Existing_LoanCount"]),
                    ExistingLoanEmi = Convert.ToDecimal(drRow["Existing_EMI"])
                };
            }
        }

        /// <summary>
        /// Forms the Lead Asset Detail Object.
        /// </summary>
        /// <param name="lead">Lead Object.</param>
        /// <param name="drRow">Data Row containing the Lead Asset Details.</param>
        private void FormLeadAssetDetailObject(Lead? lead, DataRow? drRow)
        {
            if (lead != null && drRow != null)
            {
                lead.Asset = new LeadAssetDetail
                {
                    AssetClassId = Convert.ToInt32(drRow["Asset_Class_ID"])!,
                    AssetClassDescription = Convert.ToString(drRow["Asset_Class_Description"])!,
                    AssetMakeId = Convert.ToInt32(drRow["Asset_Make_ID"])!,
                    AssetMakeDescription = Convert.ToString(drRow["Asset_Make_Description"])!,
                    AssetModelId = Convert.ToInt32(drRow["Asset_Model_ID"])!,
                    ModelDescription = Convert.ToString(drRow["Asset_Model_Description"])!,
                    Model = Convert.ToString(drRow["Vehicle_YearModel"])!,
                    VehicleNumber = Convert.ToString(drRow["Vehicle_Number"])!,
                    AssetTypeId = Convert.ToInt32(drRow["Asset_Type_ID"])!,
                    AssetTypeDescription = Convert.ToString(drRow["Asset_Type_Description"])!,
                    ChasisNumber = Convert.ToString(drRow["Vehicle_Chasis_Number"])!,
                    EngineNumber = Convert.ToString(drRow["Vehicle_Engine_Number"])!,
                    FuelTypeDescription = Convert.ToString(drRow["Asset_FuelType_Description"])!,
                    FuelTypeLookupValueId = Convert.ToInt32(drRow["Asset_FuelType_ID"])!,
                    OwnershipDescription = Convert.ToString(drRow["Asset_Ownership_Description"])!,
                    OwnershipLookupValueId = Convert.ToInt32(drRow["Asset_Ownership_ID"])!,
                    SerialNumber = Convert.ToString(drRow["Vehicle_Serial_Number"])!,
                    TaxTypeDescription = Convert.ToString(drRow["Asset_TaxType_Description"])!,
                    TaxTypeLookupValueId = Convert.ToInt32(drRow["Asset_TaxType_ID"])!,
                    VehicleTypeDescription = Convert.ToString(drRow["Asset_VehicleType_Description"])!,
                    VehicleTypeLookupValueId = Convert.ToInt32(drRow["Asset_VehicleType_ID"])!,
                };
            }

        }

        /// <summary>
        /// Forms the Lead Non Individual Object.
        /// </summary>
        /// <param name="lead">Lead Object.</param>
        /// <param name="drRow">Data Row object containing the Lead Non-Individual object.</param>
        private void FormNonIndividualObject(Lead lead, DataRow? drRow)
        {
            if (lead != null && drRow != null)
            {
                lead.NonIndividualDetail = new LeadNonIndividualDetail
                {
                    PublicCloselyLookupValueId = Convert.ToInt32(drRow["PublicClosely_ID"])!,
                    PublicCloselyDescription = Convert.ToString(drRow["PublicClosely_Description"])!,
                    DirectorCount = Convert.ToInt32(drRow["Directors_Count"])!,
                    ListedExchange = Convert.ToString(drRow["Listed_Exchange"])!,
                    PaidUpCapital = Convert.ToDecimal(drRow["Paidup_Capital"])!,
                    FaceValueShare = Convert.ToDecimal(drRow["FaceValue_Shares"])!,
                    BookValueShare = Convert.ToDecimal(drRow["BookValue_Shares"])!,
                    BusinessProfile = Convert.ToString(drRow["Businees_Profile"])!,
                    GeographicalCoverage = Convert.ToString(drRow["Geographical_Coverage"])!,
                    BranchCount = Convert.ToInt32(drRow["Branch_Count"])!,
                    IndustryLookupValueId = Convert.ToInt32(drRow["ParticipationType_ID"])!,
                    IndustryDescription = Convert.ToString(drRow["ParticipationType_Description"])!,
                    PromoterStakePercentage = Convert.ToDecimal(drRow["Promoter_Stake"])!,
                    JVPartnerName = Convert.ToString(drRow["JV_PartnerName"])!,
                    JVPartnerPercentage = Convert.ToDecimal(drRow["JV_PartnerShake"])!,
                    CeoName = Convert.ToString(drRow["CEO_Name"])!,
                    CeoDateofBirth = Convert.ToDateTime(drRow["CEO_DateofBirth"])!,
                    CeoExperience = Convert.ToInt32(drRow["Years_Experince"])!,
                    ResidentialAddress = Convert.ToString(drRow["Residential_Address"])!,
                    CeoWeddingDate = Convert.ToDateTime(drRow["Wedding_Date"])!
                };
            }
        }

        /// <summary>
        /// Adds the Lead Guarantor Object to the Lead.
        /// </summary>
        /// <param name="guarantors">Guarantors object from the Lead object.</param>
        /// <param name="drRow">Data Row containing the Lead Guarantor Details.</param>
        private LeadGuarantor AddLeadGuaranter(int companyId, int userId, DataRow drRow)
        {
            var guarantor = new LeadGuarantor();
            if (drRow != null)
            {

                guarantor.GuarantorTypeLookupValueId = Convert.ToInt32(drRow["GuarantorType_ID"]);
                guarantor.GuarantorTypeDescription = Convert.ToString(drRow["GuarantorType_Description"]);
                guarantor.GuarantorAmount = Convert.ToDecimal(drRow["Guarantor_Amount"]);
                guarantor.GuarantorRelationshipLookupValueId = Convert.ToInt32(drRow["GuarantorRelationship_ID"]);
                guarantor.GuarantorRelationshipDescription = Convert.ToString(drRow["GuarantorRelationship_Description"]);
                guarantor.ProspectId = Convert.ToInt32(drRow["Guarantor_ID"]);
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = SqlCommandConstants.FOS_ORG_GET_EXISTING_PROPSECT_CUSTOMER_DETAILS;
                    cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
                    cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
                    cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_ID, guarantor.ProspectId));
                    var dataAdapter = new SqlDataAdapter(cmd);
                    var ds = new DataSet();
                    dataAdapter.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            var dr = ds.Tables[0].Rows[0];

                            guarantor.ProspectCode = Convert.ToString(dr[SqlColumnNames.ProspectCode]);
                            guarantor.GuarantorName = Convert.ToString(dr[SqlColumnNames.ProspectName]);
                            guarantor.GenderId = Convert.ToInt32(dr[SqlColumnNames.GenderId]);
                            guarantor.GuaranterDateOfBirth = Convert.ToDateTime(dr[SqlColumnNames.DateofBirth]);
                            guarantor.MobileNumber=Convert.ToString(dr[SqlColumnNames.MobileNumber]);
                            guarantor.AlternateMobileNumber = Convert.ToString(dr[SqlColumnNames.AlternateMobileNumber]);
                            guarantor.Website = Convert.ToString(dr[SqlColumnNames.Website]);
                            guarantor.Email = Convert.ToString(dr[SqlColumnNames.Email]);

                        }

                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            var communicationAddress = ds.Tables[1].Rows.Cast<DataRow>().FirstOrDefault(dr => dr.Field<int>("Address_LookupValue_ID") == 1);
                            var permanentAddress = ds.Tables[1].Rows.Cast<DataRow>().FirstOrDefault(dr => dr.Field<int>("Address_LookupValue_ID") == 2);
                            if (communicationAddress != null)
                                guarantor.CommunicationAddress = new Address
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
                                guarantor.PermanentAddress = new Address
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
                                    guarantor.AadharImagePath = Convert.ToString(aadharDocument["Upload_Path"]);
                                    guarantor.AadharNumber = Convert.ToString(aadharDocument["Document_IdentityValue"]);
                                }
                                if (panDocument != null)
                                {
                                    guarantor.PanNumber = Convert.ToString(panDocument["Document_IdentityValue"]);
                                    guarantor.PanImagePath = Convert.ToString(panDocument["Upload_Path"]);
                                }
                                if (prospectDocument != null)
                                    guarantor.GuarantorImagePath = Convert.ToString(prospectDocument["Upload_Path"]);
                            }
                        }

                    }
                }
            }
            return guarantor;
        }
    }
}
