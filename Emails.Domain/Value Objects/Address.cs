using Emails.Domain.Value_Objects.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Domain.Value_Objects
{
    [ComplexType]
    public class Address : ValueObject<Address>
    {
        [Column("Street")]
        [Required]
        [StringLength(256)]
        public string Street { get; private set; }
        [Column("PostalCode")]
        [StringLength(6)]
        [Required]
        public string PostalCode { get; private set; }

        private Address()
        {

        }

        public Address(string street, string postalCode)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new InvalidOperationException();
            if (string.IsNullOrWhiteSpace(postalCode))
                throw new InvalidOperationException();

            this.Street = street;
            this.PostalCode = postalCode;
        }

        protected override bool EqualsCore(Address other)
        {
            return Street == other.Street
                && PostalCode == other.PostalCode;
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                int hashCode = Street.GetHashCode();
                hashCode = (hashCode * 397) ^ PostalCode.GetHashCode();

                return hashCode;
            }
        }
    }
}
