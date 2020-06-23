using System;
using System.Diagnostics;

namespace DasMulli.DataBuilderGenerator
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [Conditional("CodeGeneration")]
    public sealed class GenerateDataBuilderAttribute : Attribute
    {
    }
}
