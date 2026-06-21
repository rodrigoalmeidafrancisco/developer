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
        public List<string> Policys { get; set; }
        public List<KeyValuePair<string, string[]>> PolicysListAccess => ListOfAccessPolicies();
        public string Type { get; set; }
        public string WebUri { get; set; }

        private List<KeyValuePair<string, string[]>> ListOfAccessPolicies()
        {
            var lista = new List<KeyValuePair<string, string[]>>();
            if (Policys != null)
            {
                foreach (var policy in Policys)
                {
                    var partes = policy.Split('|');
                    if (partes.Length == 2)
                    {
                        lista.Add(new KeyValuePair<string, string[]>(partes[0], partes[1].Split(',')));
                    }
                }
            }
            return lista;
        }
    }
}
