namespace Vxp.Services.Models
{
    using AutoMapper;
    using Vxp.Data.Models;
    using Mapping;
    using Data.Common.Enums;
    using Microsoft.AspNetCore.Http;
    public class FileStoreDto : IMapTo<Document>, IHaveCustomMappings
    {
        public IFormFile FormFile { get; set; }
        public string UserId { get; set; }
        public int ProjectId { get; set; }
        public DocumentType Type { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Document, FileStoreDto>()
                .ForMember(dest => dest.UserId, opt => opt
                    .MapFrom(src => src.Project.OwnerId));
        }
    }
}