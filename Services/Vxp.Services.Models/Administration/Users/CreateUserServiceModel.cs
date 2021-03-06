﻿using AutoMapper;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Services.Models.Administration.Users
{
    public class CreateUserServiceModel : IMapTo<ApplicationUser>, IHaveCustomMappings
    {

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public string Role { get; set; }

        public string CompanyName { get; set; }

        public string CompanyVatNumber { get; set; }

        public string AccountNumber { get; set; }

        public string BicCode { get; set; }

        public string SwiftCode { get; set; }

        public string BankName { get; set; }

        public string DistributorId { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<CreateUserServiceModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.CompanyName != null ? new Company
                {
                    Name = src.CompanyName,
                    BusinessNumber = src.CompanyVatNumber
                } : null))
                .ForMember(dest => dest.BankAccounts, opt => opt.MapFrom(src => src.AccountNumber != null ? new[]
                {
                    new BankAccount {
                    AccountNumber = src.AccountNumber,
                    BankName = src.BankName,
                    BicCode = src.BicCode,
                    SwiftCode = src.SwiftCode,
                    }
                } : null))
                .ForMember(dest => dest.ContactAddress, opt => opt.MapFrom(src => src.Address != null ? new Address
                {
                    AddressLocation = src.Address,
                    City = src.City,
                    Phone = src.ContactPhone,
                    Email = src.ContactEmail ?? src.Email
                } : null));

        }
    }
}
