namespace tools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:9000";

            string user = "admin";
            string password = "prueba123*";

            string folder = "%userprofile%\\Desktop";

            List<string> proyects = new List<string>
            {
                "PlusFramework-API-2",
                "PlusFramework-COL-2",
                "PlusFramework-DAL-2",
                "PlusFramework-ENT-2",
                "PlusFramework-FCL-2",
                "PlusFramework-RED-2",
                "PlusFramework-RSL-2",
                "PlusFramework-SEC-2",
                "PlusFramework-UIL-2",
                "PlusFramework-UTL-2",
            };

            new ReportGenerator(url, user, password).Generate(folder, proyects);

            Console.Write("Press any key to close...");
            Console.ReadKey();
        }
    }
}