using Emails.Domain.Value_Objects.Base;
using FunctionalExtensionsLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Domain.Value_Objects
{
    public class ValidEmail : ValueObject<ValidEmail>
    {
        private string _value;

        private ValidEmail(string value)
        {
            _value = value;
        }

        public static Result<ValidEmail> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Fail<ValidEmail>("Email cannot be empty");

            if(value.Length > 256)
            {
                return Result.Fail<ValidEmail>("Email is too long");
            }

            if(!value.Contains("@"))
            {
                return Result.Fail<ValidEmail>("Email is invalid");
            }

            return Result.Ok<ValidEmail>(new ValidEmail(value));
        }

        protected override bool EqualsCore(ValidEmail other)
        {
            return _value == other._value;
        }

        protected override int GetHashCodeCore()
        {
            return _value.GetHashCode();
        }

        public static implicit operator string(ValidEmail validEmail)
        {
            return validEmail._value;
        }

        public static implicit operator ValidEmail(string email)
        {
            return Create(email).Value;
        }
    }
}
