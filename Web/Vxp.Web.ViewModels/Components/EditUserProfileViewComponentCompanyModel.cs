﻿namespace Vxp.Web.ViewModels.Components
{
    using Common;
    using Data.Models;
    using Infrastructure.Attributes.Validation;
    using Services.Mapping;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc;


    public class EditUserProfileViewComponentCompanyModel : IMapFrom<Company>, IMapTo<Company>
    {
        public string RoleName { get; set; }

        public EditUserProfileViewComponentCompanyModel()
        {
            this.ContactAddress = new EditUserProfileViewComponentAddressModel();
            this.ShippingAddress = new EditUserProfileViewComponentAddressModel();
        }

        [Display(Name = "Company name")]
        [StringLength(30)]
        [RequiredInSpecificRoles(compareRoleProperty: nameof(RoleName), GlobalConstants.Roles.VendorRoleName, GlobalConstants.Roles.DistributorRoleName,
            ErrorMessage = "The Company name is required.")]
        [Remote(action: "VerifyCompanyName", controller: "Users", AdditionalFields = nameof(BusinessNumber), HttpMethod = "Post")]
        public string Name { get; set; }

        [Display(Name = "Business number")]
        [StringLength(15)]
        [RequiredInSpecificRoles(compareRoleProperty: nameof(RoleName), GlobalConstants.Roles.VendorRoleName, GlobalConstants.Roles.DistributorRoleName,
            ErrorMessage = "The Company BIN is required.")]
        [Remote(action: "VerifyBusinessNumber", controller: "Users", AdditionalFields = nameof(Name), HttpMethod = "Post")]
        public string BusinessNumber { get; set; }

        public EditUserProfileViewComponentAddressModel ContactAddress { get; set; }

        public EditUserProfileViewComponentAddressModel ShippingAddress { get; set; }

    }
}