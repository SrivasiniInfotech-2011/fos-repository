using FOS.Models.Entities;

namespace FOS.Repository.Interfaces
{
    public interface IFieldVerficationRepository
    {
        /// <summary>
        /// Gets a List of Lookups for Hirer Screen.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <returns>Instance of type <see cref="List{Lookup}"/></returns>
        Task<IEnumerable<Lookup>?> GetFvrHirerLookup(int? companyId, int? userId);

        /// <summary>
        /// Gets a List of Lookups for Asset Screen.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <returns>Instance of type <see cref="List{Lookup}"/></returns>
        Task<IEnumerable<Lookup>?> GetFvrAssetLookup(int? companyId, int? userId);

        /// <summary>
        /// Gets a List of Lookups for Neighbour Screen.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <returns>Instance of type <see cref="List{Lookup}"/></returns>
        Task<IEnumerable<Lookup>?> GetFvrNeighbourLookup(int? companyId, int? userId);

        /// <summary>
        /// Gets the Lead Hirer Details.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <param name="leadNumber">Lead Number.</param>
        /// <param name="mode">Mode.</param>
        /// <param name="vehicleNumber">Vehicle Number.</param>
        /// <returns>Instance of type <see cref="FvrDetail"/></returns>
        FvrDetail? GetLeadHirerDetails(int? companyId, int? userId, string? mode, string? leadNumber = null, string? vehicleNumber = null);

        /// <summary>
        /// Gets the Lead Asset Details.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <param name="leadNumber">Lead Number.</param>
        /// <param name="vehicleNumber">Vehicle Number.</param>
        /// <returns>Instance of type <see cref="FvrAsset"/></returns>
        FvrAsset? GetLeadAssetDetails(int? companyId, int? userId, string? leadNumber = null, string? vehicleNumber = null);

        /// <summary>
        /// Gets the Fvr Neighbourhood Details.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <param name="leadId">Lead Id.</param>
        /// <param name="fieldVerificationId">Field Verification Id.</param>
        /// <returns>Instance of type <see cref="FvrNeighbourHood"/></returns>
        FvrDetail? GetFvrNeighbourHoodDetails(int? companyId, int? userId, long? leadId, long? fieldVerificationId);

        /// <summary>
        /// Adds FvrAssetDetail
        /// </summary>
        /// <param name="companyId">Company Id.</param>
        /// <param name="leadId">Lead Id.</param>
        /// <param name="fvrAssetDetail">Fvr Asset Detail</param>
        /// <returns>Integer value indicating if the data was inserted.</returns>
        Task<int> AddFvrAssetDetail(int? companyId, int? userId, int? leadId,FvrAssetDetail? fvrAssetDetail);

        /// <summary>
        /// Adds FvrHirerDetail
        /// </summary>
        /// <param name="companyId">Company Id.</param>
        /// <param name="leadId">Lead Id.</param>
        /// <param name="fvrDetail">Fvr Hirer Detail</param>
        /// <returns>Integer value indicating if the data was inserted.</returns>
        Task<int> AddFvrHirerDetail(int? companyId, int? leadId, FvrDetail? fvrDetail);

        /// <summary>
        /// Gets the FVR Details.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="userId">User Id.</param>
        /// <param name="leadId">Lead Id.</param>
        /// <param name="personType">Person Type.</param>
        /// <returns>Instance of type <see cref="FvrDetail"/></returns>
        FvrDetail? GetFvrDetails(int? companyId, int? userId, int? leadId, int? personType);

    }
}
