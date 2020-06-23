using FluentAssertions;
using System;
using Xunit;

namespace DataBuilderIntegrationTests
{
    public class DataBuilderGeneratorIntegrationTests
    {
        [Fact]
        public void ItShallSetProperties()
        {
            // Given
            var builder = DefaultAddress;

            // When
            var address = builder.Build();

            // then
            address.City.Should().Be("Vienna");
        }

        private AddressBuilder DefaultAddress => new AddressBuilder()
                .WithStreet("Some Street")
                .WithStreetNumber("22B")
                .WithTop("33")
                .WithCity("Vienna")
                .WithPostalCode("A-1234")
                .WithCountry("Austria");

        [Fact]
        public void ItShallSetOptionalProperties()
        {
            // Given
            var builder = DefaultAddress.WithNote("Test Note");

            // When
            var address = builder.Build();

            // then
            address.Note.Should().Be("Test Note");
        }

        [Fact]
        public void ItShallSetBaseTypeProperties()
        {
            // Given
            var builder = DefaultPerson.WithId(42);
            
            // When
            var person = builder.Build();

            // Then
            person.Id.Should().Be(42);
        }

        private PersonBuilder DefaultPerson => new PersonBuilder()
            .WithFirstName("Jane")
            .WithLastName("Doe");

        [Fact]
        public void ItShallIgnoreDefaultValuesEvenNonNullable()
        {
            // Given
            var builder = new AnswerToEveryThingBuilder()
                .WithAuthor("UnitTest")
                .WithCreationDate(DateTime.UtcNow);

            // When
            var answer = builder.Build();

            // Then
            answer.Value.Should().Be(42);
            answer.Question.Should().BeNull();
        }

        [Fact]
        public void ItShallFailOnUnsetConstructorValueOnly()
        {
            // Given
            var builder = new AnswerToEveryThingBuilder()
                .WithValue(0)
                .WithQuestion("test");

            // When
            Action when = () => builder.Build();

            // Then
            when.Should().Throw<InvalidOperationException>()
                .Which.Message.Should().Contain(nameof(AnswerToEveryThing.Author));
        }
    }
}
