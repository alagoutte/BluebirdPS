using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core;
using BluebirdPS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace BluebirdPS.Cmdlets.Helpers
{
    [Cmdlet(VerbsCommon.Set, "BluebirdPSConfiguration")]
    public class SetBluebirdPSConfigurationCommand : BluebirdPSCmdlet
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

                PropertyInfo property = configuration.GetType().GetProperty(config);
                if (property != null)
                {
                    property.SetValue(configuration, configValue);
                }
            }

            if (Export.IsPresent)
            {
                Config.ExportConfiguration();
            }
            else
            {
                WriteVerbose("Use the -Export switch to save the new configuration to disk.");
            }

        }
    }
}
