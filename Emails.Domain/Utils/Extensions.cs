using Emails.Domain.Entities.Base;
using Emails.Domain.Value_Objects.Base;
using FunctionalExtensionsLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Domain.Utils
{
    public static class PropsValidationExtension
    {
        public static Result<IDomainValidatableObject> Validate(this IDomainValidatableObject validatableObject) 
        {
            ValidationContext validationContext = new ValidationContext(validatableObject, null, null);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateObject(validatableObject, validationContext, validationResults);

            if (validationResults.Count > 0)
                return Result.Fail<IDomainValidatableObject>(string.Join(",", validationResults.Select(x => x.ErrorMessage)));
            else
                return Result.Ok<IDomainValidatableObject>(validatableObject);
        }
    }
}
