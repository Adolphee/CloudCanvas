using CloudCanvas.Shared.Utilities;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;

namespace CloudCanvas.Web.Data
{
    public class Address
    {
        public string? Id { get; private set; }
        public string? Street { get; private set; }
        public string? HouseNumber { get; private set; }
        public string? City { get; private set; }
        public string? PostalCode { get; private set; }
        public string? Country { get; private set; }

        public string? UserId { get; private set; }
        public List<ApplicationUser> Inhabitants { get; private set; } = default!;

        private Address() { }

        public Address(string street, string house, string pcode, string city, string country)
        {
            UpdateAddress(street, house, pcode, city);
            Country = country.Trim().ToUpperInvariant();
        }

        public void UpdateAddress(string street, string house, string pcode, string city)
        {
            Street = street.Trim();
            HouseNumber = house?.Trim() ?? string.Empty;
            PostalCode = pcode.Trim().ToUpperInvariant();
            City = city.Trim();
            // Addresses are country-bound
        }

        public Func<string> GetStreetAndHouse => () => $"{Street} {HouseNumber}";
        public Func<string> GetFullAddress => () => $"{Street} {HouseNumber}, {PostalCode} {City}, {Country}";
    }
}