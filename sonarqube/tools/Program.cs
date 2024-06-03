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
                "PlusFramework-API-3",
                "PlusFramework-COL-3",
                "PlusFramework-DAL-3",
                "PlusFramework-ENT-3",
                "PlusFramework-FCL-3",
                "PlusFramework-RED-3",
                "PlusFramework-RSL-3",
                "PlusFramework-SEC-3",
                "PlusFramework-UTL-3",
                "PlusFramework-UIL-3"
            };

            new ReportGenerator(url, user, password).Generate(folder, proyects);

            Console.Write("Press any key to close...");
            Console.ReadKey();
        }
    }
}