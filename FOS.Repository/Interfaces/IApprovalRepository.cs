using FOS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Repository.Interfaces
{
    public interface IApprovalRepository
    {
        public Task<IEnumerable<Lookup>> GetLookupsForApproval();
        public Task<IEnumerable<Lookup>> GetLookupsForApproval();

    }
}
