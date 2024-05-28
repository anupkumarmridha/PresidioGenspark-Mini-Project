﻿using System.ComponentModel.DataAnnotations;

namespace ApparelShoppingAppAPI.Models.DTO_Models
{
    public class AddressDTO
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}
