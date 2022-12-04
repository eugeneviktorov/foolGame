using System.Reflection;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FoolGame.Api;

public static class GenericExtensions
{
    public static void CopyProperties<T>(this T source, T destination)
    {
        var props = source?.GetType().GetProperties();
        if (props == null) return;
        foreach (var prop in props)
        {
            prop.SetValue(destination, prop.GetValue(source));
        }
    }
}

public class OptionSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsGenericType && context.Type.GetGenericTypeDefinition() == typeof(FSharpOption<>))
        {
            var underlyingType = context.Type.GetGenericArguments()[0]!;
            if (context.SchemaRepository.TryLookupByType(underlyingType, out var innerSchema))
            {
                innerSchema.CopyProperties(schema);
                
                schema.ReadOnly = true;
                schema.Nullable = true;

            }
            else if (underlyingType.IsGenericType && underlyingType.GetInterfaces().Contains(typeof(IEnumerable<>)))
            {
                var paramType = underlyingType.GetGenericArguments()[0]!;
                if (context.SchemaRepository.TryLookupByType(typeof(IEnumerable<>).MakeGenericType(paramType),
                        out var collSchema))
                {
                    collSchema.CopyProperties(collSchema);
                }
            }
            else
            {
                switch (underlyingType)
                {
                    case var str when str == typeof(string):
                        schema.Title = "string";
                        schema.Type = "string";
                        break;
                    case var dec when dec == typeof(decimal):
                        schema.Title = "number";
                        schema.Type = "number";
                        break;
                }
            }
        }
    }
}

public class UnionSchemaFilter : ISchemaFilter
{
    private static readonly List<Type> BuiltInTypes = new List<Type>()
    {
        typeof(FSharpOption<>),
        typeof(FSharpList<>),
        typeof(FSharpMap<,>)
    };

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if ((!context.Type.IsGenericType || BuiltInTypes.All(t => context.Type.GetGenericTypeDefinition() != t)) &&
            FSharpType.IsUnion(context.Type, null))
        {
            var cases = FSharpType.GetUnionCases(context.Type, null);

            foreach (var c in cases)
            {
                var caseFields = c.GetFields();

                if (caseFields.Length <= 0) continue;
                
                var caseType = caseFields[0].ReflectedType?.GetTypeInfo().DeclaredFields.FirstOrDefault()
                    ?.FieldType;
                var caseSchema = context.SchemaGenerator.GenerateSchema(caseType, context.SchemaRepository);

                var wrapped = new OpenApiSchema();
                wrapped.Properties.Add(c.Name, caseSchema);
                schema.OneOf.Add(wrapped);
            }
        }
    }
}