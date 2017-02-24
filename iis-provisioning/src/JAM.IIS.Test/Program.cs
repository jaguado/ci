using System;
using System.Linq;

namespace JAM.IIS.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var appPoolCreate = IISHelper.CreateOrModifyApplicationPool("Prueba2", Microsoft.Web.Administration.ProcessModelIdentityType.LocalSystem, "", "", "v4.0", true, false, Microsoft.Web.Administration.ManagedPipelineMode.Integrated, 9000, System.TimeSpan.MinValue, 0, System.TimeSpan.MinValue, 9);
            var host = "localhost";
            var info = IISHelper.GetServerManager(host);
            Console.WriteLine(host + " Information");
            Console.WriteLine("");
            Console.WriteLine("Listing Application Pools:");
            foreach(var appPool in info.ApplicationPools)
            {
                Console.WriteLine(appPool.Name);
            }
            Console.WriteLine("");
            Console.WriteLine("Listing Sites:");
            foreach (var sites in info.Sites)
            {
                Console.WriteLine(sites.Name);
            }

            //var result = IISHelper.CloneAppPool("DefaultAppPool", "localhost", "DefaultAppPoolCloned", "localhost");
            var result = IISHelper.CloneWebSite("Default Web Site", "localhost", "Default Web Site 2", "localhost");
            Console.ReadKey();
        }
    }
}
