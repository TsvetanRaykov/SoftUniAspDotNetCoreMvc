namespace Vxp.Web.ViewModels.Projects
{
    using AutoMapper;
    using Data.Models;
    using Documents;
    using Microsoft.AspNetCore.Mvc;
    using Orders;
    using Services.Mapping;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ProjectInputModel : IMapFrom<Project>, IHaveCustomMappings
    {
        public ProjectInputModel()
        {
            this.Orders = new List<OrderInputModel>();
            this.Documents = new List<DocumentViewModel>();
            this.UploadInputModel = new DocumentUploadInputModel();
        }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 1)]
        [BindProperty]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public DocumentUploadInputModel UploadInputModel { get; set; }

        public int Id { get; set; }
        public string OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<OrderInputModel> Orders { get; set; }

        public List<DocumentViewModel> Documents { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ReturnUrl { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ProjectInputModel, Project>()
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedOn, opt => opt.Ignore());
        }
    }
}