using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Administration.Users
{
    public class ListUserViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Id { get; set; }
        public bool IsDeleted { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ContactAddressCity { get; set; }
        public string ContactAddressAddressLocation { get; set; }
        public string ContactAddressPhone { get; set; }
        public string ContactAddressCountryName { get; set; }

        public ApplicationRole Role { get; set; }

        public virtual ICollection<ApplicationUserRole<string>> Roles { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, ListUserViewModel>()
                .ForMember(dest => dest.Role, opt => opt
                    .MapFrom(src => src.Roles.FirstOrDefault().Role));
        }
    }
}