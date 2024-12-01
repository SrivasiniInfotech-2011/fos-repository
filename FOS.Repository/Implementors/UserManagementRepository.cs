using Dapper;
using FOS.Models;
using FOS.Models.Constants;
using FOS.Models.Entities;
using FOS.Repository.Interfaces;
using System;
using System.Collections.Generic;
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
                //var lstlookup = await connection.QueryAsync<Lookup>(SqlCommandConstants.GetUserLevellookup, new
                //{
                //    CompanyId = companyId,
                //    UserId = userId,

                //});
                //return lstlookup.ToList();
                return null;
            }
        }



        /// <summary>
        /// Get List of Userdesignationlevel Lookupp.
        /// </summary>
        /// <returns>List of <see cref="Lookup"/></returns>
        public async Task<List<Lookup>> GetUserdesignationlevelLookup()
        {
            //using (var connection = new SqlConnection(connectionString))
            //{
            //    var lstLookups = await connection.QueryAsync<Lookup>(SqlCommandConstants.GetUserDesignationlookup);
            //    return lstLookups.ToList();
            //}
            return null;
        }
    }
}
