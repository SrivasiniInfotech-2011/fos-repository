using FOS.Models.Entities;

namespace FOS.Repository.Interfaces
{
    public interface ILeadsRepository
    {

        /// <summary>
        /// Gets a List of Statuses for Lead Translander Screen.
        /// </summary>
        /// <returns>Instance of type <see cref="List{Status}"/></returns>
        Task<IEnumerable<LeadStatus>> GetStatusesForLead();

        /// <summary>
        /// Gets a List of Lookups for Lead Generation Screen.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <returns>Instance of type <see cref="List{Lookup}"/></returns>
        IEnumerable<Lookup>? GetLeadGenerationLookup(int companyId, int userId);

        /// <summary>
        /// Gets the Prospect Detail.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id</param>
        /// <param name="mobileNumber">Mobile Number</param>
        /// <param name="aadharNumber">Aadhar Number</param>
        /// <param name="panNumber">Pan Number</param>
        /// <returns>Instance of type <see cref="LeadProspectDetail"/></returns>
        LeadProspectDetail GetProspectDetailsForLead(int companyId, int userId, string mobileNumber, string aadharNumber, string panNumber);

        /// <summary>
        /// Gets the Asset Lookup Data.
        /// </summary>
        /// <returns>Instance of type <see cref="List{Lookup}"/></returns>
        IEnumerable<Lookup>? GetAssetLookup(int companyId, int userId);

        /// <summary>
        /// Gets Lead Details.
        /// </summary>
        /// <returns>Instance of type <see cref="Lead"/></returns>
        Lead? GetLeadDetails(int companyId, int userId, int leadId, string vehicleNumber, string leadNumber);

        /// <summary>
        /// Gets the Leads Records for the Translander Page.
        /// </summary>
        /// <returns>Instance of type <see cref="LeadsTranslander"/></returns>
        LeadsTranslander GetLeadsForTranslander(int? companyId, int? userId, string? status, string? leadNumber,string? vehicleNumber,int? currentPage,int? pageSize,string? searchValue);

        /// <summary>
        /// Inserts the Lead Header cum Followup Data.
        /// </summary>
        /// <returns>Instance of type <see cref="LeadHeader"/></returns>
        Task<LeadHeader> InsertLeadDetails(int companyId, int userId, int locationId, LeadHeader leadHeader);

        /// <summary>
        /// Inserts the Lead Header.
        /// </summary>
        /// <returns>Instance of type <see cref="int"/></returns>
        Task<int> InsertLeadGenerationHeader(Lead lead);

        /// <summary>
        /// Inserts the Lead Individual Data.
        /// </summary>
        /// <returns>Instance of type <see cref="int"/></returns>
        Task<int> InsertLeadIndividual(int companyId, int userId, int leadId, LeadIndividualDetail leadIndividualDetails);

        /// <summary>
        /// Inserts the Lead Non Individual Data.
        /// </summary>
        /// <returns>Instance of type <see cref="int"/></returns>
        Task<int> InsertLeadNonIndividual(int userId, int leadId, LeadNonIndividualDetail leadNonIndividual);

        /// <summary>
        /// Inserts the Lead Guaantor Data.
        /// </summary>
        /// <param name="lead">Lead Object.</param>
        /// <returns>Instance of type <see cref="bool"/></returns>
        Task<bool> InsertGuarantorData(Lead lead);

    }
}
