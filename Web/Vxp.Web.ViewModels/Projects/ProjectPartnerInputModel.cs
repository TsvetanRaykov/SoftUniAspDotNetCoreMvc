namespace Vxp.Web.ViewModels.Projects
{
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Data.Models;
    using Services.Mapping;

    public class ProjectPartnerInputModel : IMapTo<ApplicationUser>, IHaveCustomMappings
    {
        [Display(Name = "Partner")]
        public string Name { get; set; }
        public string Id { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, ProjectPartnerInputModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.Company.Name ?? src.UserName));
        }
    }
}