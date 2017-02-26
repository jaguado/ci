using System;
using System.Linq;

namespace JAM.IIS.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (var imp = new ImpersonateHelper("jaguado", "vacas.2014", "cuprum.cl"))
            //{   
            var host = "192.168.83.240";
            //Manager.Helper.CleanIIS("localhost");
            //Manager.Helper.CloneIIS("192.168.83.240", "localhost");
            Manager.Helper.ListInformation(host);
            Environment.Exit(0);
            //}
            //result = IISHelper.CloneAppPool("Default", "localhost", "DefaultCloned", "192.168.83.240");
            //var result = IISHelper.CloneWebSite("Default Web Site", "localhost", "Default Web Site 2", "localhost");
            //var appPoolCreate = IISHelper.CreateOrModifyApplicationPool("Prueba2", Microsoft.Web.Administration.ProcessModelIdentityType.LocalSystem, "", "", "v4.0", true, false, Microsoft.Web.Administration.ManagedPipelineMode.Integrated, 9000, System.TimeSpan.MinValue, 0, System.TimeSpan.MinValue, 9);
            Console.ReadKey();
        }

       
    }
}
