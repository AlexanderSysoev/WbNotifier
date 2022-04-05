using System.Reflection;
using Refit;

namespace WbNotifier;

public class EnumAsIntParameterFormatter: DefaultUrlParameterFormatter
{
    public override string? Format(object? value, ICustomAttributeProvider attributeProvider, Type type)
    {
        if (value == null)
        {
            return null;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsEnum ||
            type.IsEnum)
        {
            return ((int) value).ToString();
        }
        
        return base.Format(value, attributeProvider, type);
    }
}