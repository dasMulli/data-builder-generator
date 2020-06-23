using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

#nullable disable

namespace Microsoft.CodeAnalysis
{
	internal static class SyntaxExtensions
    {
        internal static bool ContainsAttributeType(this SyntaxList<AttributeListSyntax> attributes, SemanticModel semanticModel, INamedTypeSymbol attributeType, bool exactMatch = false)
			=> attributes.Any(list => list.Attributes.Any(attrbute => attributeType.IsAssignableFrom(semanticModel.GetTypeInfo(attrbute).Type, exactMatch)));

        internal static SyntaxToken[] GetModifiers(this Accessibility accessibility)
        {
			var list = new List<SyntaxToken>(2);

			switch (accessibility) {
                case Accessibility.Internal:
                    list.Add(Token(SyntaxKind.InternalKeyword));
                    break;
                case Accessibility.Public:
                    list.Add(Token(SyntaxKind.PublicKeyword));
                    break;
                case Accessibility.Private:
                    list.Add(Token(SyntaxKind.PrivateKeyword));
                    break;
                case Accessibility.Protected:
                    list.Add(Token(SyntaxKind.ProtectedKeyword));
                    break;
                case Accessibility.ProtectedOrInternal:
                    list.Add(Token(SyntaxKind.InternalKeyword));
                    list.Add(Token(SyntaxKind.ProtectedKeyword));
                    break;
                case Accessibility.ProtectedAndInternal:
                    list.Add(Token(SyntaxKind.PrivateKeyword));
                    list.Add(Token(SyntaxKind.ProtectedKeyword));
                    break;
                case Accessibility.NotApplicable:
                    break;
			}

			return list.ToArray();
		}
	}

	internal static class SymbolExtensions
	{
		public static bool IsNullable(this ITypeSymbol type)
		{
			return ((type as INamedTypeSymbol)?.IsGenericType ?? false)
				&& type.OriginalDefinition.ToDisplayString().Equals("System.Nullable<T>", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsNullable(this ITypeSymbol type, out ITypeSymbol nullableType)
		{
			if (type.IsNullable())
			{
				nullableType = ((INamedTypeSymbol)type).TypeArguments.First();
				return true;
			}
			else
			{
				nullableType = null;
				return false;
			}
		}

        private static readonly Dictionary<string, string> builtinTypeMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{"string",     typeof(string).ToString()},
			{"long",       typeof(long).ToString()},
			{"int",        typeof(int).ToString()},
			{"short",      typeof(short).ToString()},
			{"ulong",      typeof(ulong).ToString()},
			{"uint",       typeof(uint).ToString()},
			{"ushort",     typeof(ushort).ToString()},
			{"byte",       typeof(byte).ToString()},
			{"double",     typeof(double).ToString()},
			{"float",      typeof(float).ToString()},
			{"decimal",    typeof(decimal).ToString()},
			{"bool",       typeof(bool).ToString()},
		};

		public static string GetFullName(this INamespaceOrTypeSymbol type)
		{
            if (type is IArrayTypeSymbol arrayType)
            {
                return $"{arrayType.ElementType.GetFullName()}[]";
            }

            if ((type as ITypeSymbol).IsNullable(out ITypeSymbol t))
            {
                return $"System.Nullable`1[{t.GetFullName()}]";
            }

            var name = type.ToDisplayString();

            if (!builtinTypeMapping.TryGetValue(name, out string output))
            {
                output = name;
            }

            return output;
		}

		public static string GetFullMetadataName(this INamespaceOrTypeSymbol symbol)
		{
			ISymbol currentSymbol = symbol;
			var sb = new StringBuilder(currentSymbol.MetadataName);

			var last = currentSymbol;
			currentSymbol = currentSymbol.ContainingSymbol;

			if (currentSymbol == null)
			{
				return symbol.GetFullName();
			}

			while (currentSymbol is object && !IsRootNamespace(currentSymbol))
			{
				if (currentSymbol is ITypeSymbol && last is ITypeSymbol)
				{
					sb.Insert(0, '+');
				}
				else
				{
					sb.Insert(0, '.');
				}
				sb.Insert(0, currentSymbol.MetadataName);

				currentSymbol = currentSymbol.ContainingSymbol;
			}

			var namedType = symbol as INamedTypeSymbol;

			if (namedType?.TypeArguments.Any() ?? false)
			{
				var genericArgs = string.Join(",", namedType.TypeArguments.Select(GetFullMetadataName));
				sb.Append($"[{ genericArgs }]");
			}

			return sb.ToString();
		}

		private static bool IsRootNamespace(ISymbol s)
		{
			return s is object && s is INamespaceSymbol ns && ns.IsGlobalNamespace;
		}

		internal static bool IsAssignableFrom(this ITypeSymbol targetType, ITypeSymbol sourceType, bool exactMatch = false)
		{
            if (targetType is null)
            {
				return false;
            }

            if (exactMatch)
            {
				return SymbolEqualityComparer.Default.Equals(sourceType, targetType);
			}

			while (sourceType != null)
			{
				if (SymbolEqualityComparer.Default.Equals(sourceType, targetType))
                {
                    return true;
                }

                if (targetType.TypeKind == TypeKind.Interface)
                {
                    return sourceType.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, targetType));
                }

                sourceType = sourceType.BaseType;
			}

			return false;
		}

		internal static IEnumerable<ITypeSymbol> GetBaseTypes(this ITypeSymbol typeSymbol)
        {
			var currentSymbol = typeSymbol;
			while (currentSymbol.BaseType != null && currentSymbol.BaseType.GetFullMetadataName() != "System.Object")
            {
				currentSymbol = currentSymbol.BaseType;
				yield return currentSymbol;
            }
        }
	}
}