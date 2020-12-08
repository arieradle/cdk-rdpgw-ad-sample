using System.Reflection;
using System.IO;

namespace CdkRdgwAdSample
{
    public class Utils
    {
        public static string GetResource(string fileName)
        {
            var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CdkRdgwAdSample.Resources." + fileName);
            StreamReader reader = new StreamReader( resStream );
            string res = reader.ReadToEnd();

            return res;
        }
    }
}