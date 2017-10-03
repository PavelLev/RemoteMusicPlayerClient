using System;
using System.Windows;
using System.Windows.Markup;
using DryIoc;

namespace RemoteFileDialog.Utility
{
    public class IocResolver : MarkupExtension
    {
        public Type ServiceType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (ServiceType == null || !Ioc.Container.IsRegistered(ServiceType))
            {
                return DependencyProperty.UnsetValue;
            }

            var result = Ioc.Container.Resolve(ServiceType);
            return result;
        }
    }
}