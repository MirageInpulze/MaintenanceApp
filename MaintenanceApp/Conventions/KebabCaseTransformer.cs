using System.Text.RegularExpressions;

namespace MaintenanceApp.Conventions;

public class KebabCaseTransformer: IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null) return null;

        // Chuyển đổi PascalCase hoặc camelCase sang kebab-case
        return Regex.Replace(value.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLower();
    }
}
