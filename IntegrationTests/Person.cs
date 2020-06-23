using DasMulli.DataBuilderGenerator;
using System;

namespace DataBuilderIntegrationTests
{
    public class BaseEntity
    {
        public long Id { get; set; }
    }

    [GenerateDataBuilder]
    public class Person : BaseEntity
    {
        public string FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public string LastName { get; set; }
        public Address? Address { get; set; }

        public Person(string firstName, string? middleNames, string lastName, Address? address)
        {
            FirstName = firstName;
            MiddleNames = middleNames;
            LastName = lastName;
            Address = address;
        }
    }
}