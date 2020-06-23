using DasMulli.DataBuilderGenerator;

namespace DataBuilderIntegrationTests
{
    [GenerateDataBuilder]
    public class Address
    {
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string? Top { get; set; }
        public int? Staircase { get; set; }
        public int? Floor { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string? Note { get; set; }

        public Address(string street, string streetNumber, string? top, string city, string postalCode, string country)
        {
            Street = street;
            StreetNumber = streetNumber;
            Top = top;
            City = city;
            PostalCode = postalCode;
            Country = country;
        }
    }
}