using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib;

public class Registration
{

    private static int RegisterLicense()
    {
        //XmlObjectBase.Register("Adas UK Ltd (5 * Developer Edition)", "Manner.xsd", "R6AKY2GN7TQMBLA1000000AA");

        // ##HAND_CODED_BLOCK_START ID="Namespace Declarations"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS
        // Add Additional namespace declarations here...
        //XmlSerializationContext.Default.SchemaType = LiquidTechnologies.Runtime.Net35.SchemaType.XSD;
        // LiquidTechnologies.Runtime.Net35.XmlSerializationContext.Default.DefaultNamespaceURI = "http:'www.fpml.org/2003/FpML-4-0"
        // LiquidTechnologies.Runtime.Net35.XmlSerializationContext.Default.NamespaceAliases.Add("dsig", "http:'www.w3.org/2000/09/xmldsig#")

        //XmlSerializationContext.Default.NamespaceAliases.Add("xs", "http:'www.w3.org/2001/XMLSchema-instance");

        // ##HAND_CODED_BLOCK_END ID="Namespace Declarations"## DO NOT MODIFY ANYTHING OUTSIDE OF THESE TAGS

        return 1;
    }
    public static int RegistrationIndicator = RegisterLicense();
}
