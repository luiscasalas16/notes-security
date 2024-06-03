using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using CsvHelper;
using tools.Apis;
using tools.Converters.Issues;
using tools.Models;
using tools.utilities;

namespace tools
{
    internal class ReportGenerator
    {
        readonly SonarqubeClient sonarqubeClient;

        readonly StringDictionary rulesCache = new StringDictionary();

        readonly Dictionary<string, int> counterCache = new Dictionary<string, int>();

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
                    for (int page = 1; ; page++)
                    {
                        if (!GenerateIssues(project.Key, page, "CODE_SMELL", csv))
                            break;
                    }
                }
            }
        }

        internal record ReportData(string Project, string Type, string Level, int Count);

        private void GenerateSummaryReport(string folder)
        {
            List<ReportData> summaries = new List<ReportData>();

            foreach (var key in counterCache.Keys)
            {
                string[] keySplit = key.Split('|');

                summaries.Add(
                    new ReportData(
                        Project: keySplit[0],
                        Type: keySplit[1],
                        Level: keySplit[2],
                        Count: counterCache[key]
                    )
                );
            }

            StringBuilder report = new StringBuilder();

            GenerateSummaryReport1(summaries, report);
            report.AppendLine();
            report.AppendLine();
            GenerateSummaryReport2(summaries, report);

            // file
            string path = Path.Combine(folder, "sonarqube-summary-report.csv");

            if (File.Exists(path))
                File.Delete(path);

            File.WriteAllText(path, report.ToString());
        }

        readonly string hotspots = "HOTSPOTS";
        readonly string[] issues = { "VULNERABILITY", "BUG", "CODE_SMELL" };
        readonly string[] issues_levels = { "BLOCKER", "CRITICAL", "MAJOR", "MINOR", "INFO" };
        readonly string[] hotspots_levels = { "HIGH", "MEDIUM", "LOW" };
        readonly string separator = ",";

        private void GenerateSummaryReport1(List<ReportData> summaries, StringBuilder report)
        {
            List<string> proyects = summaries.Select(t => t.Project).Distinct().ToList();

            //header

            report.Append("PROYECTS");
            foreach (var issue in issues)
                report.Append(separator + issue);
            report.Append(separator + hotspots);
            report.AppendLine();

            //projects

            string sum1(string type, string project)
            {
                var t = summaries
                    .Where(t => t.Type == type && t.Project == project)
                    .Sum(t => t.Count);

                return t == 0 ? "-" : Convert.ToString(t);
            }

            string sum2(string type)
            {
                var t = summaries.Where(t => t.Type == type).Sum(t => t.Count);

                return t == 0 ? "-" : Convert.ToString(t);
            }

            foreach (var project in proyects)
            {
                report.Append(project);
                foreach (var issue in issues)
                    report.Append(separator + sum1(issue, project));
                report.Append(separator + sum1(hotspots, project));
                report.AppendLine();
            }

            // totals
            report.Append("TOTALS");
            foreach (var issue in issues)
                report.Append(separator + sum2(issue));
            report.Append(separator + sum2(hotspots));
        }

        private void GenerateSummaryReport2(List<ReportData> summaries, StringBuilder report)
        {
            List<string> proyects = summaries.Select(t => t.Project).Distinct().ToList();

            //header 1

            report.Append("PROYECTS");
            foreach (var issue in issues)
            {
                foreach (var issue_level in issues_levels)
                    report.Append(separator + issue);
            }
            foreach (var hotspot_level in hotspots_levels)
                report.Append(separator + hotspots);
            report.AppendLine();

            //header 2

            report.Append("PROYECTS");
            foreach (var issue in issues)
            {
                foreach (var issue_level in issues_levels)
                    report.Append(separator + issue_level);
            }
            foreach (var hotspot_level in hotspots_levels)
                report.Append(separator + hotspot_level);
            report.AppendLine();

            //projects

            string count(string type, string level, string project)
            {
                var t = summaries.Find(t =>
                    t.Type == type && t.Level == level && t.Project == project
                );

                return t == null ? "-" : Convert.ToString(t.Count);
            }

            string sum(string type, string level)
            {
                var t = summaries.Where(t => t.Type == type && t.Level == level).Sum(t => t.Count);

                return t == 0 ? "-" : t.ToString();
            }

            foreach (var project in proyects)
            {
                report.Append(project);
                foreach (var issue in issues)
                {
                    foreach (var issue_level in issues_levels)
                        report.Append(separator + count(issue, issue_level, project));
                }
                foreach (var hotspot_level in hotspots_levels)
                    report.Append(separator + count(hotspots, hotspot_level, project));

                report.AppendLine();
            }

            // totals
            report.Append("TOTALS");
            foreach (var issue in issues)
            {
                foreach (var issue_level in issues_levels)
                    report.Append(separator + sum(issue, issue_level));
            }
            foreach (var hotspot_level in hotspots_levels)
                report.Append(separator + sum(hotspots, hotspot_level));
        }

        private bool GenerateIssues(string project, int page, string types, CsvWriter csv)
        {
            var issues = sonarqubeClient.IssuesSearch(project, 100, page, types);

            if (issues.Issues.Count == 0)
                return false;

            Console.WriteLine(
                $"Issues{(types != null ? ($" ({types})") : string.Empty)} - project {project} - page {page}"
            );

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
