using System;
using System.ComponentModel.DataAnnotations;
using Vxp.Common;

namespace Vxp.Web.Infrastructure.Attributes.Validation
{
    public class RequiredIfNewAttribute : ValidationAttribute
    {
        private readonly string _booleanProperty;

        public RequiredIfNewAttribute(string booleanProperty)
        {
            this._booleanProperty = booleanProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(this._booleanProperty);

            if (propertyInfo == null)
            {
                throw new ArgumentException(GlobalConstants.ErrorMessages.PropertyNotFound);
            }

            var comparisonValue = (bool)propertyInfo.GetValue(validationContext.ObjectInstance);


            var currentValue = (string)value;

            if (string.IsNullOrWhiteSpace(currentValue) && comparisonValue)
            {
                return new ValidationResult(string.Format(this.ErrorMessageString, validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}