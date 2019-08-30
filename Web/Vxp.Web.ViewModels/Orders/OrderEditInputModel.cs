namespace Vxp.Web.ViewModels.Orders
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Vxp.Data.Common.Enums;
    using Products;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Data.Models;
    using Services.Mapping;

    public class OrderEditInputModel : IMapFrom<Order>, IHaveCustomMappings
    {
        public OrderEditInputModel()
        {
            this.Products = new List<OrderProductViewModel>();
        }
        public int Id { get; set; }

        public int ProjectId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(36, MinimumLength = 36)]
        public string SellerId { get; set; }
        public ProductSellerViewModel Seller { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(36, MinimumLength = 36)]
        public string BuyerId { get; set; }

        public ProductSellerViewModel Buyer { get; set; }

        public List<OrderProductViewModel> Products { get; set; }

        public string TotalPrice
        {
            get { return $"{this.Products.Sum(p => p.Price * p.Quantity):N2}"; }
        }

        [Display(Name = "Ordered date")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Updated")]
        public DateTime? ModifiedOn { get; set; }

        [Display(Name = "Order Status")]
        [Required]
        public OrderStatus Status { get; set; }

        public string ReturnUrl { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<OrderEditInputModel, Order>()
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.Buyer, opt => opt.Ignore())
                .ForMember(dest => dest.BuyerId, opt => opt.Ignore())
                .ForMember(dest => dest.Seller, opt => opt.Ignore())
                .ForMember(dest => dest.SellerId, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}