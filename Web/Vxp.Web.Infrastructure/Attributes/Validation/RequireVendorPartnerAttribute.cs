using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Vxp.Common;

namespace Vxp.Web.Infrastructure.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequireVendorPartnerAttribute : ValidationAttribute
    {
        private readonly string _compareRoleProperty;
        private readonly string[] _requiredRoles = new[]
        {
            GlobalConstants.Roles.VendorRoleName,
            GlobalConstants.Roles.DistributorRoleName
        };

        public RequireVendorPartnerAttribute(string compareRoleProperty)
        {
            this._compareRoleProperty = compareRoleProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.ErrorMessage = this.ErrorMessageString;

            var compareRolePropertyInfo = validationContext.ObjectType.GetProperty(this._compareRoleProperty);

            if (compareRolePropertyInfo == null)
            {
                throw new ArgumentException("Property with this name not found");
            }

            var comparisonValue = (string)compareRolePropertyInfo.GetValue(validationContext.ObjectInstance);

            var currentValue = (string)value;

            if (string.IsNullOrWhiteSpace(currentValue) && this._requiredRoles.Contains(comparisonValue))
            {
                return new ValidationResult(this.ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}