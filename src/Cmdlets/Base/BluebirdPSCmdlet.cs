using BluebirdPS.Core;
using BluebirdPS.Models;
using System.Collections.Generic;
using System.Management.Automation;

namespace BluebirdPS.Cmdlets.Base
{
    public abstract class BluebirdPSCmdlet : PSCmdlet
    {
        internal static Configuration configuration = Config.GetOrCreateInstance();
        internal static List<ResponseData> history = History.GetOrCreateInstance();
    }
}
