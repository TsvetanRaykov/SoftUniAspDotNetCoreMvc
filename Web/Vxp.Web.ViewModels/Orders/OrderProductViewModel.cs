namespace Vxp.Web.ViewModels.Orders
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Data.Models;
    using Products;
    using Services.Mapping;
    using Vxp.Data.Common.Enums;

    public class OrderProductViewModel : IMapTo<OrderProduct>, IMapFrom<OrderProduct>, IHaveCustomMappings
    {
        public OrderProductViewModel()
        {
            ;
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductImageViewModel ProductImage { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductBasePrice { get; set; }
        [Range(0, 10000, ErrorMessage = "!")]
        public int Quantity { get; set; }
        public string TotalPrice => $"{(this.Price * this.Quantity):N2}";
        public string OldPrice => $"{this.ProductBasePrice:N2}";
        public string PriceFormatted => $"{this.Price:N2}";
        public string ProductData { get; set; }
        public string PriceModifierData { get; set; }
        public PriceModifierType PriceModifierType { get; set; }
        public decimal ModifierValue { get; set; }
        public decimal Price
        {
            get
            {
                decimal price;
                switch (this.PriceModifierType)
                {
                    case PriceModifierType.Increase:
                        price = this.ProductBasePrice + (this.ProductBasePrice * this.ModifierValue / 100);
                        break;
                    default:
                        price = this.ProductBasePrice - (this.ProductBasePrice * this.ModifierValue / 100);
                        break;
                }
                return Math.Round(price > 0 ? price : 0m, 2, MidpointRounding.AwayFromZero);
            }
        }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Product, OrderProductViewModel>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ProductBasePrice, opt => opt.MapFrom(src => src.BasePrice));
        }
    }
}