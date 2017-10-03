using System.Globalization;
using System.Windows.Controls;

namespace RemoteFileDialog.Utility
{
    public class ValidationRuleWrapper : ValidationRule
    {
        public object ValidationRule { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ((ValidationRule)ValidationRule).Validate(value, cultureInfo);
        }
    }
}