using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.core.Common;
using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.Business.Interfaces
{
    public interface ICustomerTypeService
    {
        Task<ServiceResponse<PagedResponse<IEnumerable<CustomerType>>>> GetAllAsync(PaginationParams pagination);
        Task<ServiceResponse<CustomerType>> GetByIdAsync(int id);
        Task<ServiceResponse<CustomerType>> GetByNameAsync(string name);
        Task<ServiceResponse<CustomerType>> CreateAsync(CreateCustomerTypeDto newColor);
    }
}
