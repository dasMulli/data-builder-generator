using DasMulli.DataBuilderGenerator;
using FluentAssertions;
using Xunit;

namespace DataBuilderIntegrationTests
{
    [GenerateDataBuilder]
    public class SolarSystem
    {
        public string Label { get; set; }
        public decimal Power { get; set; }
        public decimal? GeoLatitude { get; set; }
        public decimal? GeoLongitude { get; set; }
        public int? InstallationYear { get; set; }
        public int? NumberOfModules { get; set; }

        public SolarSystem(string label, decimal power, int? numberOfModules)
        {
            Label = label;
            Power = power;
            NumberOfModules = numberOfModules;
        }
    }

    public class DataBuilderGeneratorMemberTypesIntegrationTest
    {
        [Fact]
        public void ItShallInferCorrectTypesForNullableValueTypes()
        {
            SolarSystemBuilder builder = SolarPowerSystem;

            // when
            SolarSystem address = builder.Build();

            // then
            address.Label.Should().Be("My Solar Power System");
            address.Power.Should().Be(120);
            address.GeoLatitude.Should().Be((decimal) -31.837029860151624);
            address.GeoLongitude.Should().Be((decimal) 115.92793868479286);
            address.InstallationYear.Should().Be(null);
            address.NumberOfModules.Should().Be(null);
        }

        private SolarSystemBuilder SolarPowerSystem => new SolarSystemBuilder()
            .WithLabel("My Solar Power System")
            .WithPower(120)
            .WithGeoLatitude((decimal) -31.837029860151624)
            .WithGeoLongitude((decimal) 115.92793868479286)
            .WithInstallationYear(null)
            .WithNumberOfModules(null);
    }
}