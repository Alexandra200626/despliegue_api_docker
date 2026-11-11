using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.Business.DTOs
{
    public class CreateCustomerTypeDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "El nombre del tipo de cliente no debe exceder los 50 caracteres.")]
        public string CustomerTypeName { get; set; }
    }
}
