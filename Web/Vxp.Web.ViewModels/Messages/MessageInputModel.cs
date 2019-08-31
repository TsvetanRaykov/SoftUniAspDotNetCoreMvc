namespace Vxp.Web.ViewModels.Messages
{
    using System.Globalization;
    using Services.Mapping;
    using AutoMapper;
    using Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class MessageInputModel : IHaveCustomMappings
    {
        public DateTime CreatedOn { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(36, MinimumLength = 36)]
        public string RecipientId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Message { get; set; }

        public string SenderName { get; set; }

        public string CreatedOnString => this.CreatedOn.ToLocalTime().ToString(CultureInfo.CurrentCulture);

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Message, MessageInputModel>()
                .ForMember(dest => dest.Message, opt => opt
                    .MapFrom(src => src.Body))
                .ForMember(dest => dest.SenderName, opt => opt
                    .MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"));
        }
    }
}