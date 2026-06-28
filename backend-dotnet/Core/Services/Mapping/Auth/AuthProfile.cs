using AutoMapper;
using Domain.Entities.Identities;
using Shared.DTOS.AuthDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapping.Auth
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<Address,AddressDTO>().ReverseMap();
        }
    }
}
