using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Models.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required]
        public string CompanyName { get; set; }

        public string? StreetAddress { get; set; }

        public string? City { get; set; }

        public string? State{ get; set; }


        public string? PostalCode { get; set; }

        public string? PhoneNumber { get; set; }

    }

}
