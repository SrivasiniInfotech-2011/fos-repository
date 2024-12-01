using FOS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Repository.Interfaces
{

    /// <summary>
    /// Repository for UserManagement.
    /// </summary>
    public interface IUsermanagementRepository
    {

        /// <summary>
        /// Get List of UserLevel Lookup.
        /// </summary>
        /// <returns>List of <see cref="Lookup"/></returns>
        public Task<List<Lookup>> GetUserLevelLookup(int? companyId, int? userId);



        /// <summary>
        /// Get List of Prospect Lookup.
        /// </summary>
        /// <returns>List of <see cref="Lookup"/></returns>
        public Task<List<Lookup>> GetUserdesignationlevelLookup();
    }
}
