using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using Newtonsoft.Json;

namespace BluebirdPS.Cmdlets
{
    public class WriteTwitterResponseCommand : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public ResponseData ResponseData { get; set; }

        protected override void EndProcessing()
        {
            switch (Metadata.Configuration.OutputType)
            {
                case OutputType.PSCustomObject:
                    WriteObject(ResponseData.ApiResponse);
                    break;
                case OutputType.JSON:
                    WriteObject(JsonConvert.SerializeObject(ResponseData.ApiResponse));
                    break;
                case OutputType.CustomClasses:
                    WriteObject(ResponseData.ApiResponse);
                    break;
            }
        }
    }
}
