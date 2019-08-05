using System;
using System.ComponentModel.DataAnnotations;

namespace Vxp.Web.Infrastructure.Attributes.Validation
{
    public class RequiredWhenNewUserAttribute : ValidationAttribute
    {
        private readonly string _booleanProperty;

        public RequiredWhenNewUserAttribute(string booleanProperty)
        {
            this._booleanProperty = booleanProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.ErrorMessage = this.ErrorMessageString;

            var propertyInfo = validationContext.ObjectType.GetProperty(this._booleanProperty);

            if (propertyInfo == null)
            {
                throw new ArgumentException("Property with this name not found");
            }

            var comparisonValue = (bool)propertyInfo.GetValue(validationContext.ObjectInstance);


            var currentValue = (string)value;

            if (string.IsNullOrWhiteSpace(currentValue) && comparisonValue)
            {
                return new ValidationResult(this.ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}