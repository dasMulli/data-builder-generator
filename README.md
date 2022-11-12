# Data Builder Generator

Allows to generate data builder patterns for your model classes.

Reference using Visual Studio 2019 16.6 or .NET CLI 3.1.500 / 5.0.100 or higher and opt into preview language features in your project file:

```xml
<PropertyGroup>
  <LangVersion>Preview</LangVersion>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="DasMulli.DataBuilderGenerator" Version="*" PrivateAssets="All" />
</ItemGroup>
```

## Usage

Use the `GenerateDataBuilder` attribute to annotate the classes you wish to generate builder patterns for. If your class has constructors, be sure that the parameter names can be resolved to the name of a property:

```c#
    using DasMulli.DataBuilderGenerator;

    [GenerateDataBuilder]
    public class Person
    {
        public string FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public string LastName { get; set; }

        public Person(string firstName, string? middleNames, string lastName)
        {
            FirstName = firstName;
            MiddleNames = middleNames;
            LastName = lastName;
        }
    }
```

Then you can use the generated builder class:

```c#
var martinBuilder = new PersonBuilder()
    .WithFirstName("Martin")
    .WithMiddleNames("Andreas")
    .WithLastName("Ullrich");

var martin = martinBuilder.Build();

var otherMartin = martinBuilder.WithoutMiddleNames().WithLastName("Foo").Build();
```

## Notes

Special thanks to [Mayr-Melnhof Karton AG](https://www.mayr-melnhof.com/) for supporting the development of this project.
