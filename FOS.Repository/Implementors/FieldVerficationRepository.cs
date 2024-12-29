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
    public class FieldVerficationRepository(string connectionString) : IFieldVerficationRepository
    {
        /// <summary>
        /// Adds FvrAssetDetail
        /// </summary>
        /// <param name="companyId">Company Id.</param>
        /// <param name="leadId">Lead Id.</param>
        /// <param name="fvrAssetDetail">Fvr Asset Detail</param>
        /// <returns>Integer value indicating if the data was inserted.</returns>
        public async Task<int> AddFvrAssetDetail(int? companyId, int? userId, int? leadId, FvrAssetDetail fvrAssetDetail)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_ID, leadId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@TaxType_ID", fvrAssetDetail.TaxType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@TaxExpiry_Date", fvrAssetDetail.TaxExpiryDate, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@PermitStatus_ID", fvrAssetDetail.PermitStatus, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@PermitType_ID", fvrAssetDetail.PermitType, DbType.Int32, ParameterDirection.Input, 50);
                    parameters.Add("@PermitExpiry_Date", fvrAssetDetail.PermitExpiryDate, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@Insurance_Expiry_Date", fvrAssetDetail.InsuranceExpiryDate, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@Field_Executive_Comment", fvrAssetDetail.FieldExecutiveComment, DbType.String, ParameterDirection.Input);
                    parameters.Add("@RC_Registration_Date", fvrAssetDetail.RegistrationDate, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@FrontTyreStatus_ID", fvrAssetDetail.FrontTyreStatus, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@RearTyreStatus_ID", fvrAssetDetail.RearTyreStatus, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Vehicle_Colour", fvrAssetDetail.VehicleColour, DbType.String, ParameterDirection.Input, 50);
                    parameters.Add("@VehicleCondition_ID", fvrAssetDetail.VehicleCondition, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@VehicleBody_ID", fvrAssetDetail.VehicleBody, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@VehicleBody_Size", fvrAssetDetail.VehicleBodySize, DbType.String, ParameterDirection.Input, 50);
                    parameters.Add("@VehicleOwner_Name", fvrAssetDetail.VehicleOwnerName, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@VehicleEngine_Condition", fvrAssetDetail.VehicleConditionDescription, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@DuplicateKey_ID", fvrAssetDetail.DuplicateKey, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Visited_Date", fvrAssetDetail.VisitDate, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@Created_By", userId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@XML_AssetDocument", "", DbType.String, ParameterDirection.Input, 8000);
                    parameters.Add("@Vehicle_Inspected_Amount", fvrAssetDetail.InspectedValueAmount, DbType.Decimal, ParameterDirection.Input);
                    parameters.Add("@Vehicle_Inspected_Place", fvrAssetDetail.VerificationPlace, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add(SqlParameterConstants.VALIDATION_MESSAGE, DbType.String, direction: ParameterDirection.Output);
                    parameters.Add(SqlParameterConstants.PROSPECT_ERROR_CODE, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await connection.ExecuteAsync(SqlCommandConstants.FOS_ORG_INSERT_FVRASSETDETAILS, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
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

        /// <summary>
        /// Adds FvrHirerDetail
        /// </summary>
        /// <param name="companyId">Company Id.</param>
        /// <param name="leadId">Lead Id.</param>
        /// <param name="fvrDetail">Fvr Hirer Detail</param>
        /// <returns>Integer value indicating if the data was inserted.</returns>
        public async Task<int> AddFvrHirerDetail(int? companyId, int? leadId, FvrDetail? fvrDetail)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.LEAD_ID, leadId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add(SqlParameterConstants.PROSPECT_ID, fvrDetail.FvrProspectDetail.ProspectId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@FVR_Person_Type", fvrDetail.FvrHirerDetail.PersonType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@House_Accessibility_LookupValue_ID", fvrDetail.FvrHirerDetail.HouseAccessibility, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Locality_LookupValue_ID", fvrDetail.FvrHirerDetail.LocalityId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@HouseType_LookupValue_ID", fvrDetail.FvrHirerDetail.HouseType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@FlooringType_LookupValue_ID", fvrDetail.FvrHirerDetail.FlooringType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@RoofingType_LookupValue_ID", fvrDetail.FvrHirerDetail.RoofingType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@LivingStandard_LookupValue_ID", fvrDetail.FvrHirerDetail.LivingType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Permitted_LookupValue_ID", fvrDetail.FvrHirerDetail.EntryPermittedType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@House_Size", fvrDetail.FvrHirerDetail.HouseArea, DbType.String, ParameterDirection.Input, 20);
                    parameters.Add("@Landmark", fvrDetail.FvrHirerDetail.LandMark, DbType.String, ParameterDirection.Input, 200);
                    parameters.Add("@Recommented_LookupValue_ID", fvrDetail.FvrHirerDetail.Recommendation, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@EarlierVisited_LookupValue_ID", fvrDetail.FvrHirerDetail.EarlyVisitedType, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@PoliticalPicture_LookupValue_ID", fvrDetail.FvrHirerDetail.PoliticalAffiliation, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@FVR_Remarks", fvrDetail.FvrHirerDetail.Remarks, DbType.String, ParameterDirection.Input, 300);
                    parameters.Add("@Visited_Date", fvrDetail.FvrHirerDetail.DateVisited, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@House_Image_Path", fvrDetail.FvrHirerDetail.HouseImagePath, DbType.String, ParameterDirection.Input, 200);
                    parameters.Add("@XML_HouseFurniture_Details", "", DbType.String, ParameterDirection.Input, 1000);
                    parameters.Add(SqlParameterConstants.VALIDATION_MESSAGE, DbType.String, direction: ParameterDirection.Output);
                    parameters.Add(SqlParameterConstants.PROSPECT_ERROR_CODE, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await connection.ExecuteAsync(SqlCommandConstants.FOS_ORG_INSERT_FVRHIRERDETAILS, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
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

        /// <summary>
        /// Gets a List of Lookups for Hirer Screen.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <returns>Instance of type <see cref="List{Lookup}"/></returns>
        public async Task<IEnumerable<Lookup>?> GetFvrAssetLookup(int? companyId, int? userId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var lstLookups = await connection.QueryAsync<Lookup>(SqlCommandConstants.FOS_ORG_GET_FVRASSETLOOKUP);
                return lstLookups.ToList();
            }
        }

        /// <summary>
        /// Gets a List of Lookups for Hirer Screen.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <returns>Instance of type <see cref="List{Lookup}"/></returns>
        public async Task<IEnumerable<Lookup>?> GetFvrHirerLookup(int? companyId, int? userId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var lstLookups = await connection.QueryAsync<Lookup>(SqlCommandConstants.FOS_ORG_GET_FVRHIRERLOOKUP);
                return lstLookups.ToList();
            }
        }

        /// <summary>
        /// Gets the Lead Hirer Details.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <param name="leadNumber">Lead Number.</param>
        /// <param name="mode">Mode.</param>
        /// <param name="vehicleNumber">Vehicle Number.</param>
        /// <returns>Instance of type <see cref="FvrDetail"/></returns>
        public FvrDetail? GetFvrNeighbourHoodDetails(int? companyId, int? userId, long? leadId, long? fieldVerificationId)
        {
            var fvrNeighbourHoodDetail = new FvrDetail();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SqlCommandConstants.FOS_ORG_GET_EXISTING_PROPSECT_CUSTOMER_DETAILS;
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.LEAD_ID, leadId));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.FIELD_VERIFICATION_ID, fieldVerificationId));
                var dataAdapter = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                dataAdapter.Fill(ds);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        var dr = ds.Tables[0].Rows[0];
                        fvrNeighbourHoodDetail.FvrProspectDetail = new FvrLeadProspectDetail
                        {
                            LeadId = Convert.ToInt64(dr["Lead_ID"]),
                            LeadNumber = Convert.ToString(dr["Lead_Number"]),
                            VehicleNumber = Convert.ToString(dr["Vehicle_Registration_Number"]),
                            ProspectName = Convert.ToString(dr[SqlColumnNames.ProspectName]),
                        };
                    }

                    if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    {
                        var dr = ds.Tables[1].Rows[0];
                        fvrNeighbourHoodDetail.FvrNeighbourHood = new FvrNeighbourHood
                        {
                            ProspectId = Convert.ToInt64(dr["Prospect_ID"]),
                            Leadid = Convert.ToInt64(dr["Lead_ID"]),
                            HirerStayType = Convert.ToInt32(dr["HirerStay_ID"]),
                            HouseStatusType = Convert.ToInt32(dr["HouseStatus_ID"]),
                            ResidenceId = Convert.ToInt32(dr["Residence_ID"]),
                            NoOfYears = Convert.ToInt32(dr["Years_Staying"]),
                            NeighbourName = Convert.ToString(dr["Neighbour_Name"]),
                            MobileNumber = Convert.ToString(dr["Neighbour_MobileNumber"]),
                            NeighbourHoodAddress = Convert.ToString(dr["Neighbour_Address"]),
                            Comments = Convert.ToString(dr["Neighbour_Comments"]),
                        };
                    }

                    if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                    {

                        fvrNeighbourHoodDetail.FvrNeighbourHoodDocuments = ds.Tables[0].Rows.Cast<DataRow>()
                            .Select(r => new FvrDocument
                            {
                                FieldVerificationId = Convert.ToInt32(r["Field_Verification_ID"]),
                                DocumentTypeId = Convert.ToInt32(r["DocumentType_ID"]),
                                DocumentDescription = Convert.ToString(r["DocumentType_Decsription"]),
                                DocumentPath = Convert.ToString(r["Document_Path"])
                            }).ToArray();
                    }
                }
            }
            return fvrNeighbourHoodDetail;
        }

        public async Task<IEnumerable<Lookup>?> GetFvrNeighbourLookup(int? companyId, int? userId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var lstLookups = await connection.QueryAsync<Lookup>(SqlCommandConstants.FOS_ORG_GET_FVRNEIGHBOURLOOKUP);
                return lstLookups.ToList();
            }
        }

        /// <summary>
        /// Gets the Lead Asset Details.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <param name="leadNumber">Lead Number.</param>
        /// <param name="vehicleNumber">Vehicle Number.</param>
        /// <returns>Instance of type <see cref="FvrAsset"/></returns>
        public FvrAsset? GetLeadAssetDetails(int? companyId, int? userId, string? leadNumber = null, string? vehicleNumber = null)
        {
            var fvrAsset = new FvrAsset();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SqlCommandConstants.FOS_ORG_GET_FVRASSETDETAILS;
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.LEAD_NUMBER, leadNumber));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.LEAD_VEHICLE_NUMBER, vehicleNumber));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.MODE, "Q"));
                var dataAdapter = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                dataAdapter.Fill(ds);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        var dr = ds.Tables[0].Rows[0];
                        fvrAsset.FvrAssetDetail = new FvrAssetDetail
                        {
                            FieldVerificationId = Convert.ToInt32(dr["Field_Verification_ID"]),
                            ProspectName = Convert.ToString(dr["Prospect_Name"]),
                            TaxType = Convert.ToInt32(dr["Tax_TypeID"]),
                            TaxExpiryDate = Convert.ToDateTime(dr["TaxExpiry_Date"]),
                            PermitType = Convert.ToInt32(dr["Permit_TypeID"]),
                            PermitStatus = Convert.ToInt32(dr["Permit_StatusID"]),
                            PermitExpiryDate = Convert.ToDateTime(dr["PermitExpiry_Date"]),
                            InsuranceExpiryDate = Convert.ToDateTime(dr["Insurance_Expiry_Date"]),
                            FieldExecutiveComment = Convert.ToString(dr["Field_Executive_Comment"]),
                            RegistrationDate = Convert.ToDateTime(dr["RC_Registration_Date"]),
                            InspectedValueAmount = Convert.ToDecimal(dr["Inspected_Value_Amount"]),
                            FrontTyreStatusDescription = Convert.ToString(dr["Front_Tyre_status"]),
                            FrontTyreStatus = Convert.ToInt32(dr["Front_Tyre_statusID"]),
                            RearTyreStatusDescription = Convert.ToString(dr["Rear_Tyre_status"]),
                            RearTyreStatus = Convert.ToInt32(dr["Rear_Tyre_statusID"]),
                            VehicleColour = Convert.ToString(dr["Vehicle_Colour"])!,
                            VehicleConditionDescription = Convert.ToString(dr["Vechicle_Condition"]),
                            VehicleCondition = Convert.ToInt32(dr["Vechicle_ConditionID"]),
                            VerificationPlace = Convert.ToString(dr["Vehicle_Inspected_Place"]),
                            VehicleBodyDescription = Convert.ToString(dr["Vehicle_Body"]),
                            VehicleBody = Convert.ToInt32(dr["Vehicle_BodyID"]),
                            VehicleBodySize = Convert.ToString(dr["VehicleBody_Size"]),
                            VehicleOwnerName = Convert.ToString(dr["VehicleOwner_Name"]),
                            VehicleEngineCondition = Convert.ToString(dr["VehicleEngine_Condition"]),
                            VisitDate = Convert.ToDateTime(dr["Visited_Date"]),
                            VerifierName = Convert.ToString(dr["Verifier_Name"]),
                            DuplicateKey = Convert.ToInt32(dr["DuplicateKeyID"]),
                            VerifierId = Convert.ToInt32(dr["Verifier_ID"]),
                            VerifierCode = Convert.ToString(dr["Verifier_Code"]),
                            VehicleRegistrationNumber = Convert.ToString(dr["Vehicle_Registration_Number"]),
                            VehicleInspectionDate = Convert.ToDateTime(dr["Visited_Date"])
                        };
                    }

                    if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    {

                        fvrAsset.FvrDocuments = ds.Tables[1].Rows.Cast<DataRow>()
                            .Select(r => new FvrDocument
                            {
                                FieldVerificationId = Convert.ToInt32(r["Field_VerificationID"]),
                                DocumentDescription = Convert.ToString(r["Document_Type"]),
                                DocumentPath = Convert.ToString(r["Document_Path"])
                            }).ToArray();
                    }
                }
            }
            return fvrAsset;
        }

        /// <summary>
        /// Gets the Fvr Neighbourhood Details.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <param name="leadId">Lead Id.</param>
        /// <param name="fieldVerificationId">Field Verification Id.</param>
        /// <returns>Instance of type <see cref="FvrNeighbourHood"/></returns>
        public FvrDetail? GetLeadHirerDetails(int? companyId, int? userId, string? mode, string? leadNumber = null, string? vehicleNumber = null)
        {
            var fvrDetail = new FvrDetail();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SqlCommandConstants.FOS_ORG_GET_LEADHIRERDETAILS;
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_COMPANY_ID, companyId));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.PROSPECT_USER_ID, userId));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.LEAD_NUMBER, leadNumber));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.LEAD_VEHICLE_NUMBER, vehicleNumber));
                cmd.Parameters.Add(new SqlParameter(SqlParameterConstants.MODE, mode));
                var dataAdapter = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                dataAdapter.Fill(ds);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        var dr = ds.Tables[0].Rows[0];
                        fvrDetail.FvrProspectDetail = new FvrLeadProspectDetail
                        {
                            ProspectId = Convert.ToInt64(dr["Prospect_ID"]),
                            LeadId = Convert.ToInt64(dr["Lead_ID"]),
                            LeadNumber = Convert.ToString(dr["Lead_Number"]),
                            LeadDate = Convert.ToDateTime(dr["Lead_Date"]),
                            LocationId = Convert.ToInt32(dr["Location_ID"]),
                            LocationName = Convert.ToString(dr["Location_Name"]),
                            ProspectName = Convert.ToString(dr[SqlColumnNames.ProspectName]),
                            LeadType = Convert.ToInt32(dr["Lead_Type"]),
                            ProspectAddress = Convert.ToString(dr["Prospect_Address"]),
                            MobileNumber = Convert.ToString(dr["Mobile_Number"]),
                            VehicleNumber = Convert.ToString(dr["Vehicle_Number"]),
                        };
                    }

                    if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    {
                        var dr = ds.Tables[1].Rows[0];
                        fvrDetail.FvrHirerDetail = new FVrHirer
                        {
                            ProspectId = Convert.ToInt64(dr["ProspectId"]),
                            FieldVerificationId = Convert.ToInt64(dr["FieldVerificationId"]),
                            VisitedBy = Convert.ToString(dr["VisitedBy"]),
                            DateVisited = Convert.ToDateTime(dr["DateVisited"]),
                            PersonType = Convert.ToInt32(dr["PersonType"]),
                            HouseAccessibility = Convert.ToInt32(dr["HouseAccessibility"]),
                            LocalityId = Convert.ToInt32(dr["LocalityId"]),
                            HouseType = Convert.ToInt32(dr["HouseType"]),
                            FlooringType = Convert.ToInt32(dr["FlooringType"]),
                            RoofingType = Convert.ToInt32(dr["RoofingType"]),
                            LivingType = Convert.ToInt32(dr["LivingType"]),
                            EntryPermittedType = Convert.ToInt32(dr["EntryPermittedType"]),
                            HouseArea = Convert.ToString(dr["HouseArea"]),
                            LandMark = Convert.ToString(dr["LandMark"])!,
                            Recommendation = Convert.ToInt32(dr["Recommendation"]),
                            PoliticalAffiliation = Convert.ToInt32(dr["PoliticalAffiliation"]),
                            EarlyVisitedType = Convert.ToInt32(dr["EarlyVisitedType"]),
                            Remarks = Convert.ToString(dr["Remarks"]),
                            VerifierId = Convert.ToString(dr["VisitedBy"])!.Split(" - ")[0],
                        };
                    }

                    if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                    {

                        var dr = ds.Tables[2].Rows[0];
                        fvrDetail.FvrNeighbourHood = new FvrNeighbourHood
                        {
                            ProspectId = Convert.ToInt64(dr["ProspectId"]),
                            FieldVerificationId = Convert.ToInt64(dr["FieldVerificationId"]),
                            VisitedBy = Convert.ToString(dr["UserVisited"]),
                            DateVisited = Convert.ToDateTime(dr["DateVisited"]),
                            PersonType = Convert.ToInt32(dr["PersonType"]),
                            HirerStayType = Convert.ToInt32(dr["HirerStayType"]),
                            HouseStatusType = Convert.ToInt32(dr["HouseStatusType"]),
                            NeighbourHoodAddress = Convert.ToString(dr["NeighbourHoodAddress"]),
                            MobileNumber = Convert.ToString(dr["MobileNumber"]),
                            Comments = Convert.ToString(dr["Comments"])
                        };
                    }
                }
            }
            return fvrDetail;
        }
    }
}
