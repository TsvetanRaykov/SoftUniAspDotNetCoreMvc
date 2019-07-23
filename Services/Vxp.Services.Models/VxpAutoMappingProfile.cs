using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Services.Models
{
    public class VxpAutoMappingProfile : IHaveCustomMappings
    {
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationRole, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
        }
    }
}