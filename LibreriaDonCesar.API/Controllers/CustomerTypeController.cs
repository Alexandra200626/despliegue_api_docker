using DocumentFormat.OpenXml.EMMA;
using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.Business.Interfaces;
using LibreriaDonCesar.Business.Services;
using LibreriaDonCesar.core.Common;
using LibreriaDonCesar.Core.Common;
using LibreriaDonCesar.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaDonCesar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerTypesController : ControllerBase
    {
        private readonly ICustomerTypeService _customerTypeService;

        public CustomerTypesController(ICustomerTypeService customerTypeService)
        {
            _customerTypeService = customerTypeService;
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
        {
            var serviceResponse = await _customerTypeService.GetAllAsync(pagination);

            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
                var customerTypeDtoCollection = serviceResponse.Data.Data.Select(a => new CustomerTypeDto
                {
                    Id = a.Id,
                    CustomerTypeName = a.CustomerTypeName
                });

                var apiResponse = new ApiResponse<IEnumerable<CustomerTypeDto>>
                {
                    Data = customerTypeDtoCollection,
                    Meta = new
                    {
                        totalRecords = serviceResponse.Data.TotalRecords,
                        totalPages = serviceResponse.Data.TotalPages,
                        pageNumber = serviceResponse.Data.PageNumber,
                        pageSize = serviceResponse.Data.PageSize,
                        message = serviceResponse.Message
                    }
                };

                return Ok(apiResponse);
            }

            var unsuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NoData:
                    unsuccessfulResponse.Code = "200";
                    unsuccessfulResponse.Message = "No se encontraron registros";
                    unsuccessfulResponse.Details = new { info = "Temporalmente no hay registros en la BD" };
                    return Ok(unsuccessfulResponse);

                default:
                    unsuccessfulResponse.Code = "500";
                    unsuccessfulResponse.Message = "Ocurrio un error inesperado";
                    unsuccessfulResponse.Details = new { info = "Error interno en la aplicacion" };
                    return StatusCode(500, unsuccessfulResponse);
            }
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                var response = new UnsuccessfulResponseDto()
                {
                    Code = "400",
                    Message = "Id proporcionado debe ser mayor a 0",
                    Details = new { info = "Error en el formato del valor enviado" }
                };

                return BadRequest(response);
            }

            var serviceResponse = await _customerTypeService.GetByIdAsync(id);

            if (serviceResponse.IsSuccess)
            {
                var customerTypeDto = new CustomerTypeDto
                {
                    Id = serviceResponse.Data!.Id,
                    CustomerTypeName = serviceResponse.Data.CustomerTypeName
                };

                return Ok(customerTypeDto);
            }

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    var unsuccessfulResponse = new UnsuccessfulResponseDto
                    {
                        Code = "404",
                        Message = "No se encontro un tipo de cliente asociado al Id proporcionado",
                        Details = new { info = serviceResponse.Message ?? "No se encontró el recurso solicitado" }
                    };
                    return NotFound(unsuccessfulResponse);

                default:
                    unsuccessfulResponse = new UnsuccessfulResponseDto
                    {
                        Code = "500",
                        Message = "Ocurrió un error",
                        Details = new { info = serviceResponse.Message ?? "Error interno no esperado" }
                    };
                    return StatusCode(500, unsuccessfulResponse);
            }
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet("byname/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            if (string.IsNullOrWhiteSpace(name))
            {
                unSuccessfulResponse.Code = "400";
                unSuccessfulResponse.Message = "El dato proporcionado no es válido";
                unSuccessfulResponse.Details = new { Error = "El Name no puede ser nulo o vacío" };
                return BadRequest(unSuccessfulResponse);
            }

            var serviceResponse = await _customerTypeService.GetByNameAsync(name);

            if (serviceResponse.IsSuccess)
            {
                var customerTypeDto = new CustomerTypeDto
                {
                    Id = serviceResponse.Data!.Id,
                    CustomerTypeName = serviceResponse.Data.CustomerTypeName
                };

                return Ok(customerTypeDto);
            }

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = "No se encontró el tipo de cliente asociado al valor de Name proporcionado";
                    unSuccessfulResponse.Details = new { Error = "No hay registros asociados al valor de Name proporcionado" };
                    return NotFound(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = serviceResponse.Message ?? "Ocurrió un error inesperado";
                    return StatusCode(500, unSuccessfulResponse);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateCustomerTypeDto customerTypeDto)
        {
            var serviceResponse = await _customerTypeService.CreateAsync(customerTypeDto);

            if (serviceResponse.IsSuccess)
            {
                var newCustomerTypeDto = new CustomerTypeDto
                {
                    Id = serviceResponse.Data!.Id,
                    CustomerTypeName = serviceResponse.Data.CustomerTypeName,
                };

                return CreatedAtAction(
                    nameof(GetById),
                    new { Id = newCustomerTypeDto.Id },
                    newCustomerTypeDto
                );
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.Conflict:
                    unSuccessfulResponse.Code = "409";
                    unSuccessfulResponse.Message = "El nombre del tipo de cliente ya existe";
                    unSuccessfulResponse.Details = new { info = "No se puede duplicar un tipo de cliente" };
                    return Conflict(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrio un error inesperado";
                    unSuccessfulResponse.Details = new { info = "Error interno inesperado" };
                    return StatusCode(500, unSuccessfulResponse);
            }
        }
    }
}
