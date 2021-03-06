﻿namespace Vxp.Web.ViewModels.Prices
{
    using Data.Models;
    using Services.Mapping;
    using System.ComponentModel.DataAnnotations;
    using Vxp.Data.Common.Enums;
    using AutoMapper;

    public class PriceModifierInputModel : IHaveCustomMappings
    {
        public PriceModifierInputModel()
        {
            this.PriceModifierRange = PriceModifierRange.Total;
        }
        public int PriceModifierId { get; set; }

        public string ReturnUrl { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SellerId { get; set; }
        public PriceModifierUserInputModel Seller { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string BuyerId { get; set; }
        public PriceModifierUserInputModel Buyer { get; set; }

        [Required]
        public PriceModifierRange PriceModifierRange { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public PriceModifierType PriceModifierType { get; set; }

        [Display(Name = "Value %")]
        [Required]
        [Range(typeof(decimal), "0.00", "100000")]
        public decimal PercentValue { get; set; }

        public override string ToString()
        {
            if (this.PercentValue == 0) { return $"{this.PercentValue:N2} %"; }

            switch (this.PriceModifierType)
            {
                case PriceModifierType.Decrease:
                    return $"Down {this.PercentValue:N2} %";
                default:
                    return $"Up {this.PercentValue:N2} %";
            }
        }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<PriceModifierInputModel, PriceModifier>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PriceModifierId));
            configuration.CreateMap<PriceModifier, PriceModifierInputModel>()
                .ForMember(dest => dest.PriceModifierId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
