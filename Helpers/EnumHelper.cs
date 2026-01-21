using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Trasformazioni.Helpers
{
    public static class EnumHelper
    {
        public static string GetDisplayName(Enum value)
        {
            var member = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

            if (member != null)
            {
                var attribute = member.GetCustomAttribute<DisplayAttribute>();
                if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name))
                {
                    return attribute.Name;
                }
            }

            return value.ToString();
        }
    }
}
