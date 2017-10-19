using System;
using RemoteMusicPlayerClient.Utility;

namespace RemoteMusicPlayerClient.Networking
{
    public class OnlineStatusService : IOnlineStatusService
    {
        private readonly IToastService _toastService;
        private OnlineStatus _currentOnlineStatus = OnlineStatus.Online;

        public OnlineStatusService(IToastService toastService)
        {
            _toastService = toastService;
        }

        public void BecomeOnline()
        {
            Set(OnlineStatus.Online);
        }

        public void BecomeOffline()
        {
            Set(OnlineStatus.Offline);
        }

        private void Set(OnlineStatus newOnlineStatus)
        {
            if (_currentOnlineStatus != newOnlineStatus)
            {
                _currentOnlineStatus = newOnlineStatus;
                OnlineStatusChanged?.Invoke(newOnlineStatus);
                _toastService.Show(_currentOnlineStatus.ToString());
            }
            
        }

        public event Action<OnlineStatus> OnlineStatusChanged;
    }
}