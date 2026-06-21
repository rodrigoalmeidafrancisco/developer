namespace Shared.Settings
{
    public class SettingAppApplication
    {
        public SettingAppApplication()
        {

        }

        public string _Build { get; set; }
        public string _Environment { get; set; }
        public string _Release { get; set; }
        public string Name { get; set; }
        public List<SettingAppApplicationPolicy> Policys { get; set; }
        public string Type { get; set; }
        public string WebUri { get; set; }
    }

    public class SettingAppApplicationPolicy
    {
        public string Name { get; set; }
        public string[] Scopes { get; set; }
    }
}
