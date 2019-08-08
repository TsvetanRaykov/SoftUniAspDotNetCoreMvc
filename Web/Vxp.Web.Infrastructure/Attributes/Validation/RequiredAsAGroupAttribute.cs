namespace Vxp.Web.Infrastructure.Attributes.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAsAGroupAttribute : ValidationAttribute
    {
        private readonly string _groupName;

        public RequiredAsAGroupAttribute(string groupName)
        {
            this._groupName = groupName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var properties = validationContext.ObjectType.GetProperties().Where(p =>
                p.CustomAttributes.Any(a => a.AttributeType == typeof(RequiredAsAGroupAttribute)));

            foreach (var propertyInfo in properties)
            {
                var attributeInfo = propertyInfo.GetCustomAttribute<RequiredAsAGroupAttribute>();

                if (this._groupName != attributeInfo._groupName) { continue; }

                var propValue = propertyInfo.GetValue(validationContext.ObjectInstance);
                if (propValue != null && value == null)
                {
                    return new ValidationResult(string.Format(this.ErrorMessageString, validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }
    }
}