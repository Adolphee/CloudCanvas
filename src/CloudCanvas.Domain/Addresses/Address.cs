using CloudCanvas.Domain.User;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;

namespace CloudCanvas.Domain.Addresses
{
    public class Address
    {
        public string? Id { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        public string? UserId { get; set; }
        public List<string?> Inhabitants { get; set; } = default!;

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