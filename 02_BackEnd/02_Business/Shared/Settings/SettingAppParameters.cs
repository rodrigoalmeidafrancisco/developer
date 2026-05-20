namespace Shared.Settings
{
    public class SettingAppParameters
    {
        public SettingAppParameters()
        {
            Proxy = new SettingAppParametersProxy();
        }

        public SettingAppParametersProxy Proxy { get; set; }
    }

    public class SettingAppParametersProxy
    {
        public string ByPass { get; set; }
        public string[] ByPassArray => ByPass.Split('|');
        public bool Enable { get; set; }
        public string Porta { get; set; }
        public string Url { get; set; }
        public string UrlPorta => $"{Url}:{Porta}";
    }
}
