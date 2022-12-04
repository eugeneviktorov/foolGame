using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FoolGame.Api
{
    internal sealed class OperationIdFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!string.IsNullOrEmpty(operation.OperationId))
                return;

            var name = context.MethodInfo.Name;
            if (name.EndsWith("Async"))
                name = name[..^5];
            operation.Extensions.Add("x-operation-name", new OpenApiString(CamelCase(name)));
        }

        private static string CamelCase(string name) =>
            string.IsNullOrEmpty(name) ? name : char.ToLowerInvariant(name[0]) + name[1..];
    }
}