using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Vxp.Common;

namespace Vxp.Web.Infrastructure.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredInSpecificRolesAttribute : ValidationAttribute
    {
        private readonly string _compareRoleProperty;
        private readonly string[] _requiredRoles;

        public RequiredInSpecificRolesAttribute(string compareRoleProperty, params string[] requiredRoles)
        {
            this._compareRoleProperty = compareRoleProperty;
            this._requiredRoles = requiredRoles;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var compareRolePropertyInfo = validationContext.ObjectType.GetProperty(this._compareRoleProperty);

            if (compareRolePropertyInfo == null)
            {
                throw new ArgumentException(GlobalConstants.ErrorMessages.PropertyNotFound);
            }

            var comparisonValue = (string)compareRolePropertyInfo.GetValue(validationContext.ObjectInstance);

            if (value == null && this._requiredRoles.Contains(comparisonValue))
            {
                return new ValidationResult(string.Format(this.ErrorMessageString, validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}