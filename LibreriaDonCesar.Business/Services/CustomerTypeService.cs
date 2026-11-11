using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.Business.Interfaces;
using LibreriaDonCesar.core.Common;
using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using LibreriaDonCesar.DataAccess.Interfaces;
using LibreriaDonCesar.DataAccess.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.Business.Services
{
    public class CustomerTypeService : ICustomerTypeService
    {
        private readonly ICustomerTypeRepository _customerTypeRepository;

        public CustomerTypeService(ICustomerTypeRepository customerTypeRepository)
        {
            _customerTypeRepository = customerTypeRepository;
        }

        public async Task<ServiceResponse<PagedResponse<IEnumerable<CustomerType>>>> GetAllAsync(PaginationParams pagination)
        {
            var result = await _customerTypeRepository.GetAllAsync(pagination);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<PagedResponse<IEnumerable<CustomerType>>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Success,
                    Message = "Operacion exitosa"
                };
            }

            switch (result.OperationStatusCode)
            {
                case 50037:
                    return new ServiceResponse<PagedResponse<IEnumerable<CustomerType>>>
                    {
                        Data = null,
                        IsSuccess = true,
                        MessageCode = MessageCodes.NoData,
                        Message = "No se encontraron registros"
                    };

                default:
                    return new ServiceResponse<PagedResponse<IEnumerable<CustomerType>>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = result.Message ?? "Ocurrió un error inesperado"
                    };
            }
        }

        public async Task<ServiceResponse<CustomerType>> GetByIdAsync(int id)
        {
            var repoResponse = await _customerTypeRepository.GetByIdAsync(id);

            try
            {
                if (repoResponse.OperationStatusCode == 0)
                {
                    return new ServiceResponse<CustomerType>
                    {
                        Data = repoResponse.Data,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Success,
                        Message = repoResponse.Message ?? "Operacion exitosa"
                    };
                }

                switch (repoResponse.OperationStatusCode)
                {
                    case 50050:
                        return new ServiceResponse<CustomerType>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No existe el tipo de cliente"
                        };

                    default:
                        return new ServiceResponse<CustomerType>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.ErrorDataBase,
                            Message = repoResponse.Message ?? "Ocurrio un error inesperado"
                        };
                }
            }
            catch (Exception)
            {
                return new ServiceResponse<CustomerType>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = repoResponse.Message ?? "Ocurrio un error inesperado"
                };
            }
        }

        public async Task<ServiceResponse<CustomerType>> GetByNameAsync(string name)
        {
            var result = await _customerTypeRepository.GetByNameAsync(name);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<CustomerType>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Success,
                    Message = "Operacion exitosa"
                };
            }

            var messageCode = new MessageCodes();
            var message = string.Empty;

            switch (result.OperationStatusCode)
            {
                case 50051:
                    messageCode = MessageCodes.NotFound;
                    message = "No existe el tipo de cliente";
                    break;

                default:
                    messageCode = MessageCodes.ErrorDataBase;
                    message = "Error en la base de datos al obtener el tipo de cliente.";
                    break;
            }

            return new ServiceResponse<CustomerType>
            {
                Data = null,
                IsSuccess = false,
                MessageCode = messageCode,
                Message = message
            };
        }

        public async Task<ServiceResponse<CustomerType>> CreateAsync(CreateCustomerTypeDto newCustomerType)
        {
            try
            {
                var existing = await _customerTypeRepository.GetByNameAsync(newCustomerType.CustomerTypeName);

                // Validar que la respuesta contiene datos antes de desreferenciar
                if (existing != null &&
                    existing.OperationStatusCode == 0 &&
                    existing.Data != null &&
                    existing.Data.Id != 0 &&
                    !string.IsNullOrEmpty(existing.Data.CustomerTypeName))
                {
                    return new ServiceResponse<CustomerType>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.Conflict,
                        Message = "Existe un registro con el nombre proporcionado"
                    };
                }

                var customerType = new CustomerType()
                {
                    CustomerTypeName = newCustomerType.CustomerTypeName
                };

                var result = await _customerTypeRepository.AddAsync(customerType);

                return new ServiceResponse<CustomerType>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Success,
                    Message = "Registro creado con exito"
                };
            }
            catch (Exception ex)
            {
                // Registrar ex (logger) aquí para diagnóstico; no suprimir detalles en producción
                return new ServiceResponse<CustomerType>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado"
                };
            }
        }
    }
}