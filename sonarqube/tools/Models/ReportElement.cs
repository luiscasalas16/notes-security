namespace tools.Models
{
    internal class ReportElement
    {
        public string Key { get; set; }
        public string Component { get; set; }
        public string Project { get; set; }
        public string RuleCode { get; set; }
        public string RuleDescription { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
        public string Line { get; set; }
        public string Type { get; set; }
        public string StartLine { get; set; }
        public string EndLine { get; set; }
        public string StartOffset { get; set; }
        public string EndOffset { get; set; }
    }
}
