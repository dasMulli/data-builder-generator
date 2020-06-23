using DasMulli.DataBuilderGenerator;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DataBuilderTests
{
    public class DataBuilderTests
    {
        [Fact]
        public async Task ItShallBuildSuccessfully()
        {
            // Given
            string sourceCode = @"
[DasMulli.DataBuilderGenerator.GenerateDataBuilderAttribute]
public class TestClass {}

[DasMulli.DataBuilderGenerator.GenerateDataBuilderAttribute]
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
";
            var outputAssembly = Path.Combine(Path.GetTempPath(), $"Test_{nameof(ItShallBuildSuccessfully)}.dll");

            // When
            var result = await WhenTheSourceCodeIsCompiled(sourceCode, outputAssembly);

            // Then
            result.Success.Should().BeTrue();
            var ctx = new AssemblyLoadContext(nameof(ItShallBuildSuccessfully), true);
            var loadedAssembly = ctx.LoadFromAssemblyPath(outputAssembly);
            loadedAssembly.GetType("TestClassBuilder")
                .Should().NotBeNull()
                .And.Subject.GetMember("Build").Length.Should().BeGreaterOrEqualTo(1);
        }

        private async Task<EmitResult> WhenTheSourceCodeIsCompiled(string sourceCode, string outputAssembly, [CallerMemberName] string compilationAssemblyName = "TestCompilation")
        {
            var host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
            var projectInfo = ProjectInfo.Create(
                ProjectId.CreateNewId(),
                VersionStamp.Create(),
                compilationAssemblyName,
                compilationAssemblyName,
                LanguageNames.CSharp
                )
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithNullableContextOptions(NullableContextOptions.Enable))
                .WithParseOptions(new CSharpParseOptions(LanguageVersion.Preview))
                .WithMetadataReferences(new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location)!, "netstandard.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location)!, "System.Runtime.dll")),
                    MetadataReference.CreateFromFile(typeof(GenerateDataBuilderAttribute).Assembly.Location)
                })
                .WithAnalyzerReferences(new[]{
                    new AnalyzerFileReference(typeof(DataBuilderGenerator).Assembly.Location, new DataBuilderGeneratorAnalyzerLoader())
                    });

            var compilation = await new AdhocWorkspace(host)
                .CurrentSolution
                .AddProject(projectInfo)
                .AddDocument(DocumentId.CreateNewId(projectInfo.Id, compilationAssemblyName + ".cs"), compilationAssemblyName + ".cs", SourceText.From(sourceCode, Encoding.UTF8))
                .GetProject(projectInfo.Id)!
                .GetCompilationAsync();

            var result = compilation!.Emit(outputAssembly);
            return result;
        }

        private class DataBuilderGeneratorAnalyzerLoader : IAnalyzerAssemblyLoader
        {
            public void AddDependencyLocation(string fullPath)
            {
            }

            public Assembly LoadFromPath(string fullPath)
            {
                return typeof(DataBuilderGenerator).Assembly;
            }
        }
    }
}
