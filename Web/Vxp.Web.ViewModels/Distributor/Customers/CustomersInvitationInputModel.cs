namespace Vxp.Web.ViewModels.Distributor.Customers
{
    using AutoMapper;
    using Common;
    using Services.Mapping;
    using Services.Models;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using Data.Models;

    public class CustomersInvitationInputModel : IMapTo<EmailDto>, IHaveCustomMappings
    {
        [BindProperty]
        [Display(Name = "Email To")]
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string EmailTo { get; set; }

        [BindProperty]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string Subject { get; set; }

        [BindProperty]
        [Display(Name = "Message")]
        [Required(AllowEmptyStrings = false)]
        [MinLength(10, ErrorMessage = GlobalConstants.ErrorMessages.InvitationMessageLengthError)]
        public string MessageBody { get; set; }

        [BindProperty]
        [Display(Name = "Distributor Key")]
        [Required(AllowEmptyStrings = false)]
        public string DistributorKey { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? Accepted { get; set; }

        public string CreatedOnString => this.CreatedOn.ToLocalTime().ToString(CultureInfo.CurrentCulture);
        public string AcceptedOnString => this.Accepted?.ToLocalTime().ToString(CultureInfo.CurrentCulture);

        public IEnumerable<CustomersInvitationInputModel> SentInvitations { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<CustomerInvitation, CustomersInvitationInputModel>()
                .ForMember(dest => dest.EmailTo, opt => opt.MapFrom(src => HttpUtility.HtmlEncode(src.EmailTo)))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => HttpUtility.HtmlEncode(src.Subject)))
                .ForMember(dest => dest.MessageBody, opt => opt.MapFrom(src => HttpUtility.HtmlEncode(src.MessageBody)));
        }
    }
}