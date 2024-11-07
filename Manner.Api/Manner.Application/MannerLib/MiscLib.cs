using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib
{
    public class MiscLib
    {
        public static Stream GetResourceStream(string ResourceName)
        {

            var @assembly = System.Reflection.Assembly.GetExecutingAssembly();

            var sr = new StreamReader(assembly.GetManifestResourceStream(ResourceName));
            string strData = sr.ReadToEnd();

            byte[] byteArray = Encoding.ASCII.GetBytes(strData);
            var stream = new MemoryStream(byteArray);
            return stream;

        }
    }
}
