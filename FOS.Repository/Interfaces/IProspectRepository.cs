using FOS.Models.Entities;

namespace FOS.Repository.Interfaces
{
    /// <summary>
    /// Repository for Prospect Master.
    /// </summary>
    public interface IProspectRepository
    {
        /// <summary>
        /// Get List of States.
        /// </summary>
        /// <returns>List of <see cref="Lookup"/></returns>
        public Task<List<Lookup>> GetStateLookups();

        /// <summary>
        /// Get List of Prospect Lookup.
        /// </summary>
        /// <returns>List of <see cref="Lookup"/></returns>
        public Task<List<Lookup>> GetProspectLookup();

        /// <summary>
        /// Get List of Branch Location.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="lobId">Lob Id</param>
        /// <param name="isActive"> Is Active</param>
        /// <param name="userId">User Id</param>
        /// <returns>List of <see cref="Location"/></returns>
        public Task<List<Location>> GetBranchLocations(int? companyId, int? userId, int? lobId, bool? isActive);

        /// <summary>
        /// Get the Customer Details for a Prospect.
        /// </summary>
        /// <returns>List of <see cref="Prospect"/></returns>
        public Task<Prospect> GetExistingProspectCustomerDetails(int? companyId, int? userId, string mobileNumber, string aadharNumber, string panNumber, int? prospectId);

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
        /// <param name="prospectDate">Prospect Date.</param>
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
        /// <returns>Integer Value Indicating if the record got saved.</returns>
        public Task<int> InsertProspectDetails(
                                                int companyId, int locationId, int prospectTypeId, int customerId, string customerCode, int? genderId,
                                                string prospectName,DateTime?prospectDate, DateTime? dob, string mobileNumber, string? alternativeMobileNumber, string? email,
                                                string? website, string communicationAddress1, string communicationAddress2, string communicationLandmark,
                                                string communicationCity, int? communicationStateId, int? communicationCountryId, string communicationPinCode,
                                                string permanentAddress1, string permanentAddress2, string permanentLandmark, string permanentCity, int? permanentStateId,
                                                int? permanentCountryId, string permanentPinCode, string? aadharNumber, string? aadharImagePath, string panNumber,
                                                string panImagePath, string ProspectImagePath, int createdBy, long? prospectId, string prospectCode, int errorCode
                                                );

        /// <summary>
        /// Get List of LOB.
        /// </summary>
        /// <returns>List of <see cref="LineOfBusiness"/></returns>
        public IEnumerable<LineOfBusiness>? GetLineofBusiness(int companyId,int userId);

        /// <summary>
        /// Get List of LOB.
        /// </summary>
        /// <returns>List of <see cref="DocumentCategory"/></returns>
        public IEnumerable<DocumentCategory>? GetDocumentCategories(int companyId, int userId, int option);

        /// <summary>
        /// Gets the List of Field Executives.
        /// </summary>
        /// <param name="companyId">Company Id.</param>
        /// <param name="userId">User Id.</param>
        /// <param name="prefix">Prefix.</param>
        /// <returns></returns>
        public IEnumerable<FieldExecutive>? GetFieldExecutives(int companyId,int userId,string prefix);

        /// <summary>
        /// Get List of Prospects for Export.
        /// </summary>
        /// <returns>List of <see cref="ProspectExportData"/></returns>
        public Task<List<ProspectExportData>> GetProspectDataForExport();
    }
}
