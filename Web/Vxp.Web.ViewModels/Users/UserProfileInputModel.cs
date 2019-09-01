namespace Vxp.Web.ViewModels.Users
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Data.Models;
    using Services.Mapping;
    using Infrastructure.Attributes.Validation;
    using ModelBinders;
    using Common;

    public class UserProfileInputModel : IHaveCustomMappings
    {
        public bool IsNewUser { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public UserProfileInputModel()
        {
            this.BankAccounts = new HashSet<SelectListItem>();
            this.AvailableCountries = new List<SelectListItem>();
            this.AvailableDistributors = new List<SelectListItem>();
            this.AvailableRoles = new List<SelectListItem>();
            this.Company = new UserProfileCompanyInputModel();
        }

        [StringLength(36)]
        public string UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Role")]
        [StringLength(36)]
        public string RoleName { get; set; }

        [Required]
        [Display(Name = "Login email")]
        [EmailAddress]
        [Remote(action: "ValidateNewUsername", controller: "Users", AdditionalFields = nameof(IsNewUser) + ",__RequestVerificationToken", HttpMethod = "Post")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "First name")]
        [StringLength(50,
            ErrorMessage = "{0} must be at least {2} symbols", MinimumLength = 3)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Last name")]
        [StringLength(50,
            ErrorMessage = "{0} must be at least {2} symbols", MinimumLength = 3)]
        public string LastName { get; set; }

        [RequiredIfNew(nameof(IsNewUser),
            ErrorMessage = GlobalConstants.ErrorMessages.RequiredField)]
        [Display(Name = "Password")]
        [StringLength(100,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password",
            ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //Company
        public UserProfileCompanyInputModel Company { get; set; }

        //Address
        public UserProfileAddressInputModel ContactAddress { get; set; }

        [ModelBinder(typeof(KvpStringListToSelectListModelBinder))]
        public IEnumerable<SelectListItem> BankAccounts { get; set; }

        //Other
        [Display(Name = "Distributor")]
        [StringLength(36)]
        public string DistributorId { get; set; }

        public List<SelectListItem> AvailableRoles { get; set; }

        [ModelBinder(typeof(KvpStringListToSelectListModelBinder))]
        public ICollection<SelectListItem> AvailableDistributors { get; set; }

        [ModelBinder(typeof(KvpStringListToSelectListModelBinder))]
        public ICollection<SelectListItem> AvailableCountries { get; set; }

        public string StatusMessage { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationRole, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            configuration.CreateMap<BankAccount, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => $"{src.BankName} [{src.AccountNumber}]"));

            configuration.CreateMap<ApplicationUser, UserProfileInputModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Roles.First().Role.Name))
                .ForMember(dest => dest.AvailableDistributors,
                    opt => opt.MapFrom(src => src.Distributors.Select(d => new SelectListItem
                    {
                        Value = d.DistributorKey.BankAccount.Owner.Id,
                        Text = $"{d.DistributorKey.BankAccount.Owner.Company.Name}"
                    })));

            configuration.CreateMap<UserProfileInputModel, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.BankAccounts, opt => opt.Ignore())
                .ForMember(dest => dest.Distributors, opt => opt.Ignore());
        }
    }
}