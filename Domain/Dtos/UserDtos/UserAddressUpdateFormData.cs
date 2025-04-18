using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Domain.Dtos.UserDtos;

public class UserAddressUpdateFormData
{
    [SwaggerSchema(Description = "Uppdaterat gatunamn.")]
    public string? StreetName { get; set; }


    [SwaggerSchema(Description = "Uppdaterat gatunummer.")]
    public string? StreetNumber { get; set; }


    [SwaggerSchema(Description = "Uppdaterat postnummer.")]
    public string? PostalCode { get; set; }


    [SwaggerSchema(Description = "Uppdaterad stad.")]
    public string? City { get; set; }


    [SwaggerSchema(Description = "Uppdaterat land.")]
    public string? Country { get; set; }
}

