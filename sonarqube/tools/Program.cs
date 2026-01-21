namespace tools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:9000";

            string user = "admin";
            string password = "admin";

            string folder = "%userprofile%\\Desktop";

            new ReportGenerator(url, user, password).Generate(folder);

            Console.Write("Press any key to close...");
            Console.ReadKey();
        }
    }
}
