namespace Vxp.Web.ViewModels.Users
{
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Data.Models;
    using Services.Mapping;

    public class UserProfileBankAccountInputModel : IMapFrom<BankAccount>, IMapTo<BankAccount>, IHaveCustomMappings
    {
        public int? Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(36)]
        public string OwnerId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Account number")]
        [StringLength(30)]
        public string AccountNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "BIC code")]
        [StringLength(20)]
        public string BicCode { get; set; }

        [Display(Name = "SWIFT code")]
        [StringLength(20)]
        public string SwiftCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Bank name")]
        [StringLength(30)]
        public string BankName { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UserProfileBankAccountInputModel, BankAccount>()
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.DistributorKeys, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedOn, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedOn, opt => opt.Ignore());
        }
    }
}