using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.DataAccess.Interfaces
{
    public interface ICustomerTypeRepository
    {
        Task<RepositoryResponse<PagedResponse<IEnumerable<CustomerType>>>> GetAllAsync(PaginationParams pagination);
        Task<RepositoryResponse<CustomerType>> GetByIdAsync(int id);
        Task<RepositoryResponse<CustomerType>> GetByNameAsync(string name);
        Task<RepositoryResponse<CustomerType>> AddAsync(CustomerType customerType);
    }
}
