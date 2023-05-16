using System;
using Wemogy.Core.Extensions;

namespace Wemogy.AspNetCore.GraphQl.Extensions
{
    public static class GraphTypeTypeExtensions
    {
        public static string GraphTypeName(this Type graphType, bool upperFirst = false)
        {
            string graphTypeNameEnding = "GraphType";
            string graphTypeFullName = graphType.Name;
            string graphTypeNameWithoutEnding =
                graphTypeFullName.Substring(0, graphTypeFullName.Length - graphTypeNameEnding.Length);

            if (upperFirst)
            {
                return graphTypeNameWithoutEnding.ToPascalCase();
            }

            return graphTypeNameWithoutEnding.ToCamelCase();
        }

        public static string InputGraphTypeName(this Type graphType, bool upperFirst = false)
        {
            string graphTypeNameEnding = "InputGraphType";
            string graphTypeFullName = graphType.Name;
            string graphTypeNameWithoutEnding =
                graphTypeFullName.Substring(0, graphTypeFullName.Length - graphTypeNameEnding.Length);

            if (upperFirst)
            {
                return graphTypeNameWithoutEnding.ToPascalCase();
            }

            return graphTypeNameWithoutEnding.ToCamelCase();
        }
    }
}
