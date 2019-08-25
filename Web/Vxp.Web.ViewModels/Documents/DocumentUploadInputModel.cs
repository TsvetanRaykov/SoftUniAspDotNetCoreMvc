namespace Vxp.Web.ViewModels.Documents
{
    using AutoMapper;
    using Services.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Services.Mapping;
    using System.ComponentModel.DataAnnotations;
    using Vxp.Data.Common.Enums;

    public class DocumentUploadInputModel : IHaveCustomMappings
    {
        [Display(Name = "Upload Document")]
        [BindProperty]
        [Required]
        public IFormFile FormFile { get; set; }

        [Required(AllowEmptyStrings = false)]
        [BindProperty]
        public string Description { get; set; }

        [Required]
        [BindProperty]
        public DocumentType Type { get; set; }

        [Required]
        [BindProperty]
        public int ProjectId { get; set; }

        public string ReturnUrl { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<DocumentUploadInputModel, FileStoreDto>()
                .ForMember(dest => dest.OriginalFileName, opt => opt
                    .MapFrom(src => src.FormFile.FileName))
                .ForMember(dest => dest.ContentType, opt => opt
                .MapFrom(src => src.FormFile.ContentType));
        }
    }
}