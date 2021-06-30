using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Linq;
using System.Reflection;

namespace BluebirdPS.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "BluebirdPSConfiguration")]
    public class SetBluebirdPSConfigurationCommand : PSCmdlet
    {
        [Parameter]
        public RateLimitAction RateLimitAction { get; set; }
        [Parameter]
        public int RateLimitThreshold { get; set; }
        [Parameter]
        public OutputType OutputType { get; set; }
        [Parameter]
        public SwitchParameter Export { get; set; }

        protected override void ProcessRecord()
        {
            IEnumerable<string> configParameters = MyInvocation.BoundParameters.Keys.Except(CommonParameters).Where(param => param != "Export");
            
            foreach (string config in configParameters)
            {
                var configValue = MyInvocation.BoundParameters[config];
                string message = $"Setting configuration value for {config} to '{configValue}'.";
                WriteVerbose(message);

                PropertyInfo property = Metadata.Configuration.GetType().GetProperty(config);
                if (property != null)
                {
                    property.SetValue(Metadata.Configuration, configValue);
                }
            }

            if (Export.IsPresent)
            {
                ExportBluebirdPSConfigurationCommand exportConfiguration = new ExportBluebirdPSConfigurationCommand();
                WriteObject(exportConfiguration.Invoke());
            }
            else
            {
                WriteVerbose("Use the -Export switch to save the new configuration to disk.");
            }

        } 
    }
}
