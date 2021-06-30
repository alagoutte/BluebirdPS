using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Text.RegularExpressions;
using BluebirdPS;
using System.IO;

namespace BluebirdPS
{
    public enum RateLimitAction
    {
        // type of output stream used to display a message
        // when the remaining calls are at the RateLimitThreshold or less

        Verbose,
        Warning,
        Error
    }

    public enum OutputType
    {
        CustomClasses,
        PSCustomObject,
        JSON
    }

    public class Configuration
    {
        
        public string AuthUserId { get; set; }
        public string AuthUserName { get; set; }
        public DateTime? AuthValidationDate { get; set; }
        public DateTime? AuthLastExportDate { get; set; }
        public RateLimitAction RateLimitAction { get; set; } = RateLimitAction.Warning;
        public int RateLimitThreshold { get; set; } = 5;
        public string ConfigurationPath { get; set; } = Path.Join(Helpers.GetFileSavePath(), "Configuration.Json");
        public string CredentialsPath { get; set; } = Path.Join(Helpers.GetFileSavePath(), "twittercred.sav");
        public OutputType OutputType { get; set; } = OutputType.JSON;
        public Configuration() { }

    }
}
