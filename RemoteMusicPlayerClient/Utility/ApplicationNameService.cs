namespace RemoteMusicPlayerClient.Utility
{
    public class ApplicationNameService : IApplicationNameService
    {
        public string Get()
        {
            return "Remote Music Player Client";
        }
    }
}