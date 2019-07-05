using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Chatbot.API.Helpers
{
    // this class was implemented based on this post
    // Manual Validation with Data Annotations
    // https://odetocode.com/blogs/scott/archive/2011/06/29/manual-validation-with-data-annotations.aspx
    public static class ModelValidator
    {
        public static ModelValidatorResult IsValid(object model)
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results);

            if (!isValid)
                return new ModelValidatorResult(
                    results?.Select(error => new ErrorValidation(
                        error?.ErrorMessage,
                        error?.MemberNames?.ToList()
                    ))?.ToList()
                ); //TODO: wrapped up this into the Auto Mapper
            else
                return new ModelValidatorResult();
        }
    }

    public class ModelValidatorResult
    {
        public ModelValidatorResult()
        {
        }

        public ModelValidatorResult(List<ErrorValidation> errorValidations)
        {
            HasError = true;
            ErrorValidations = errorValidations;
        }

        public bool HasError { get; } = false;
        public List<ErrorValidation> ErrorValidations { get; }
    }

    public class ErrorValidation
    {
        public ErrorValidation(string errorMessage, List<string> memberNames)
        {
            this.ErrorMessage = errorMessage ?? throw new System.ArgumentNullException(nameof(errorMessage));
            MemberNames = memberNames;
        }

        public string ErrorMessage { get; }
        public List<string> MemberNames { get; }
    }
}