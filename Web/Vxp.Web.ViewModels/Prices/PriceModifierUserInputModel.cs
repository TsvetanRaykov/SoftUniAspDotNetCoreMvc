﻿namespace Vxp.Web.ViewModels.Prices
{
    using AutoMapper;
    using Data.Models;
    using Services.Mapping;

    public class PriceModifierUserInputModel : IHaveCustomMappings
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, PriceModifierUserInputModel>()
                .ForMember(dest => dest.Name, opt => opt
                    .MapFrom(src => src.Company != null ? src.Company.Name : $"{src.FirstName} {src.LastName}"));
        }
    }
}