using System.Text;
using tools.Converters.Hotspots;
using tools.Converters.Issues;
using tools.Converters.Projects;
using tools.Converters.Rules;

namespace tools.Apis
{
    internal class SonarqubeClient
    {
        private string url;
        private string credentials;

        public SonarqubeClient(string url, string user, string password)
        {
            this.url = url;
            this.credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(user + ":" + password));
        }

        public HotspotsConverter HotspotsSearch(string project, int pageSize, int pageNumber)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/api/hotspots/search?projectKey={project}&ps={pageSize}&p={pageNumber}");
            request.Headers.Add("Authorization", "Basic " + credentials);
            var response = client.Send(request);
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            return HotspotsConverter.FromJson(json);
        }

        public IssuesConverter IssuesSearch(string project, int pageSize, int pageNumber, string types = null)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/api/issues/search?componentKeys={project}&ps={pageSize}&p={pageNumber}" + (types != null ? ($"&types={types}") : string.Empty));
            request.Headers.Add("Authorization", "Basic " + credentials);
            var response = client.Send(request);
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            return IssuesConverter.FromJson(json);
        }

        public ProjectsConverter ProjectsSearch(List<string> projects = null)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/api/projects/search");
            request.Headers.Add("Authorization", "Basic " + credentials);
            var response = client.Send(request);
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            var result = ProjectsConverter.FromJson(json);
            if (projects != null)
            {
                foreach (var component in result.Components.ToList())
                {
                    var t = projects.FirstOrDefault(t => t.Equals(component.Name, StringComparison.OrdinalIgnoreCase));

                    if (t == null)
                        result.Components.Remove(component);
                }
            }
            return result;
        }

        public RulesConverter RuleShow(string rule)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/api/rules/show?key={rule}");
            request.Headers.Add("Authorization", "Basic " + credentials);
            var response = client.Send(request);
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            return RulesConverter.FromJson(json);
        }
    }
}
