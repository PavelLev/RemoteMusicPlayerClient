using System.Reflection;
using Windows.UI.Notifications;

namespace RemoteMusicPlayerClient.Utility
{
    public class ToastService : IToastService
    {
        private readonly IApplicationNameService _applicationNameService;

        public ToastService(IApplicationNameService applicationNameService)
        {
            _applicationNameService = applicationNameService;
        }

        public void Show(string message)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            
            var stringElement = toastXml.GetElementsByTagName("text")[0];
            stringElement.AppendChild(toastXml.CreateTextNode("Online"));
            
            var toast = new ToastNotification(toastXml);
            
            ToastNotificationManager.CreateToastNotifier(_applicationNameService.Get()).Show(toast);
        }
    }
}