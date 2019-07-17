using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using AutoMapper;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.Areas.Administration.ViewModels.Dashboard
{
    public class ListUserViewModel : IMapFrom<ApplicationUser>
    {
        public string Id { get; set; }
        public bool IsDeleted { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ContactAddressCity { get; set; }
        public string ContactAddressAddressLocation { get; set; }
        public string ContactAddressPhone { get; set; }
        public string ContactAddressCountryName { get; set; }

        public string CompanyName { get; set; }

        public string Role { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }
    }
}