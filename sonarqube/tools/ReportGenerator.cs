using CsvHelper;
using System.Collections.Specialized;
using System.Globalization;
using tools.Apis;
using tools.Models;
using tools.utilities;

namespace tools
{
    internal class ReportGenerator
    {
        private SonarqubeClient sonarqubeClient;

        private StringDictionary rulesCache = new StringDictionary();

        private Dictionary<string, int> counterCache = new Dictionary<string, int>();

        public ReportGenerator(string url, string user, string password) 
        {
            sonarqubeClient = new SonarqubeClient(url, user, password);
        }

        public void Generate(string folder, List<string> proyects = null)
        {
            folder = Environment.ExpandEnvironmentVariables(folder);

            GeneratePrimaryReport(folder, proyects);
            GenerateSecondaryReport(folder, proyects);
            GenerateSummaryReport(folder);
        }

        private void GeneratePrimaryReport(string folder, List<string> proyects = null)
        {
            string path = Path.Combine(folder, "sonarqube-primary-report.csv");

            if (File.Exists(path))
                File.Delete(path);

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<ReportElement>();
                csv.NextRecord();

                var projects = sonarqubeClient.ProjectsSearch(proyects);

                foreach (var project in projects.Components)
                {
                    for (int page = 1; ; page++)
                    {
                        if (!GenerateIssues(project.Key, page, "BUG,VULNERABILITY", csv))
                            break;
                    }

                    for (int page = 1; ; page++)
                    {
                        if (!GenerateHotSpots(project.Key, page, csv))
                            break;
                    }
                }
            }
        }

        private void GenerateSecondaryReport(string folder, List<string> proyects = null)
        {
            string path = Path.Combine(folder, "sonarqube-secondary-report.csv");

            if (File.Exists(path))
                File.Delete(path);

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<ReportElement>();
                csv.NextRecord();

                var projects = sonarqubeClient.ProjectsSearch(proyects);

                foreach (var project in projects.Components)
                {
                    for (int page = 1 ;; page++)
                    {
                        if (!GenerateIssues(project.Key, page, "CODE_SMELL", csv))
                            break;
                    }
                }
            }
        }

        private void GenerateSummaryReport(string folder)
        {
            string path = Path.Combine(folder, "sonarqube-summary-report.csv");

            if (File.Exists(path))
                File.Delete(path);

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<ReportSummary>();
                csv.NextRecord();

                foreach (var key in counterCache.Keys)
                {
                    string[] keySplit = key.Split('|');

                    ReportSummary reportSummary = new ReportSummary()
                    {
                        Project = keySplit[0],
                        Type = keySplit[1],
                        Level = keySplit[2],
                        Count = counterCache[key].ToString()
                    };

                    csv.WriteRecord(reportSummary);
                    csv.NextRecord();
                }
            }
        }

        private bool GenerateIssues(string project, int page, string types, CsvWriter csv)
        {
            var issues = sonarqubeClient.IssuesSearch(project, 100, page, types);

            if (issues.Issues.Count == 0)
                return false;

            Console.WriteLine($"Issues{(types != null ? ($" ({types})") : string.Empty)} - project {project} - page {page}");

            foreach (var issue in issues.Issues)
            {
                Count(project + "|" + issue.Type.ToString() + "|" + issue.Severity.ToString());

                ReportElement reportElement = new ReportElement()
                {
                    Key = issue.Key,
                    Component = issue.Component,
                    Project = issue.Project,
                    RuleCode = issue.Rule,
                    RuleDescription = GetRuleDescription(issue.Rule),
                    Severity = issue.Severity.ToString(),
                    Message = issue.Message,
                    Line = issue.Line.ToString(),
                    Type = issue.Type.ToString(),
                    StartLine = issue.TextRange.StartLine.ToString(),
                    EndLine = issue.TextRange.EndLine.ToString(),
                    StartOffset = issue.TextRange.StartOffset.ToString(),
                    EndOffset = issue.TextRange.EndOffset.ToString()
                };

                csv.WriteRecord(reportElement);
                csv.NextRecord();
            }

            return true;
        }

        private bool GenerateHotSpots(string project, int page, CsvWriter csv)
        {
            var hotspots = sonarqubeClient.HotspotsSearch(project, 100, page);

            if (hotspots.Hotspots.Count == 0)
                return false;

            Console.WriteLine($"HotSpots - project {project} - page {page}");

            foreach (var hotspot in hotspots.Hotspots)
            {
                Count(project + "|HOTSPOTS|" + hotspot.VulnerabilityProbability);

                ReportElement reportElement = new ReportElement()
                {
                    Key = hotspot.Key,
                    Component = hotspot.Component,
                    Project = hotspot.Project,
                    RuleCode = hotspot.RuleKey,
                    RuleDescription = GetRuleDescription(hotspot.RuleKey),
                    Severity = hotspot.VulnerabilityProbability,
                    Message = hotspot.Message,
                    Line = hotspot.Line.ToString(),
                    Type = "HOTSPOTS",
                    StartLine = hotspot.TextRange.StartLine.ToString(),
                    EndLine = hotspot.TextRange.EndLine.ToString(),
                    StartOffset = hotspot.TextRange.StartOffset.ToString(),
                    EndOffset = hotspot.TextRange.EndOffset.ToString()
                };

                csv.WriteRecord(reportElement);
                csv.NextRecord();
            }

            return true;
        }

        private void Count(string key)
        {
            if (counterCache.ContainsKey(key))
                counterCache[key] += 1;
            else
                counterCache.Add(key, 1);
        }

        private string GetRuleDescription(string rule)
        {
            string ruleDescription;

            if (rulesCache.ContainsKey(rule))
                ruleDescription = rulesCache[rule]!;
            else
            {
                var ruleObject = sonarqubeClient.RuleShow(rule).Rule;

                if (ruleObject.HtmlDesc != null)
                    ruleDescription = Html.ConvertToPlainText(ruleObject.HtmlDesc).Trim();
                else
                    ruleDescription = ruleObject.Name;

                rulesCache.Add(rule, ruleDescription);
            }

            return ruleDescription;
        }
    }
}
