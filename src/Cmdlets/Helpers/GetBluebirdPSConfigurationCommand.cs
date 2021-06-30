using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;

namespace BluebirdPS.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "BluebirdPSConfiguration")]
    [OutputType(typeof(Configuration))]
    public class GetBluebirdPSConfigurationCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(Metadata.Configuration);
        }        
    }
}
