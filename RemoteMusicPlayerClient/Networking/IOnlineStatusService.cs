using System;

namespace RemoteMusicPlayerClient.Networking
{
    public interface IOnlineStatusService
    {
        void BecomeOnline();
        void BecomeOffline();
        event Action<OnlineStatus> OnlineStatusChanged;
    }
}