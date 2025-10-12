using System.Reflection;
using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;
using StudentEnrollment.Application;
using StudentEnrollment.Domain;
using Vogen;

namespace StudentEnrollment.StartUp;

public static class OpenApiUtils
{
    public static ICollection<ITypeMapper> AddValueObjectTypeMappers(this ICollection<ITypeMapper> typeMappers)
    {
        var valueObjects = typeof(DependencyInjectionDomain).Assembly
                            .GetTypes()
                            .Concat(typeof(DependencyInjectionApplication).Assembly.GetTypes())
                            .Where(type => type.HasGenericAttribute<ValueObjectAttribute<object>>());

        foreach (var valueObject in valueObjects)
        {
            var attribute = valueObject.GetCustomAttribute<ValueObjectAttribute>()!;
            var attributeType = attribute.GetType();

            if (!attributeType.IsGenericType || attributeType.GenericTypeArguments.Length != 1)
            {
                continue;
            }

            typeMappers.Add(new PrimitiveTypeMapper(valueObject, schema => schema.PrimitiveBasedJsonSchema(attributeType.GenericTypeArguments[0])));
        }

        return typeMappers;
    }

    private static bool HasGenericAttribute<TAttribute>(this Type type)
    {
        return type.GetCustomAttributes()
            .Any(attr => attr.GetType().IsGenericType &&
                        attr.GetType().GetGenericTypeDefinition() == typeof(TAttribute).GetGenericTypeDefinition());
    }

    private static JsonSchema PrimitiveBasedJsonSchema(this JsonSchema schema, Type type)
    {
        if (type == typeof(int))
        {
            schema.Type = JsonObjectType.Integer;
            schema.Format = "int32";
        }
        else if (type == typeof(string))
        {
            schema.Type = JsonObjectType.String;
        }
        else if (type == typeof(Guid))
        {
            schema.Type = JsonObjectType.String;
            schema.Format = "uuid";
            schema.Description = "A GUID.";
            schema.Example = "e1ddbeb8-0009-4a20-885a-6bfcd02fc3d1";
        }
        else
        {
            throw new ArgumentException("Value object inner type could not be mapped to JsonObjectType.");
        }
        return schema;
    }
}
