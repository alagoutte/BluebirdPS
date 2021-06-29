using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using BluebirdPS.APIV2.MediaInfo;
using BluebirdPS.APIV2.TweetInfo;
using BluebirdPS.APIV2.UserInfo;
using BluebirdPS.APIV2.Objects;
using System.Collections;
using System.IO;
using System.Collections.ObjectModel;

namespace BluebirdPS
{
    public class Helpers
    {
        internal static string GetFileSavePath()
        {
            return Platform.IsWindows ?
                    Path.Join(Environment.GetEnvironmentVariable("USERPROFILE"), ".BluebirdPS") :
                    Path.Join(Environment.GetEnvironmentVariable("HOME"), ".BluebirdPS");
        }

        public static bool HasProperty(PSObject input, string propertyName)
        {
            return input != null && input.Properties[propertyName] != null;
        }

        public static string ToTitleCase(string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }
        
        public static DateTime? ConvertFromV1Date(string input)
        {
            return input != null ? (DateTime?)DateTime.ParseExact(input, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.CurrentCulture) : null;

        }

        public static DateTime? ConvertFromEpochTime(string input)
        {
            try
            {
                return input.Length == 10
                    ? DateTimeOffset.FromUnixTimeSeconds(long.Parse(input)).ToLocalTime().DateTime
                    : DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(input)).ToLocalTime().DateTime;

            }
            catch
            {
                return null;
            }
        }

        public static string ConvertToV1Date(DateTime input)
        {
            return input != null ? input.ToUniversalTime().ToString("s", CultureInfo.InvariantCulture) + "Z" : null;
        }

        public static bool IsJson(string text)
        {
            return true;
        }

        public static string GetErrorCategory(string errorType)
        {            
            if (Metadata.ErrorCategoryV2.ContainsKey(errorType))
            {
                return Metadata.ErrorCategoryV2[errorType].ToString();
            } else
            {
                return "NotSpecified";
            }
        }
    
        public static string GetErrorCategory(int statusCode, int errorCode)
        {

            switch (statusCode)
            {
                case 406:
                    return "InvalidData";
                case 415:
                    return "LimitsExceeded";
                case 420:
                    return "QuotaExceeded";
                case 422:
                    return errorCode == 404 ? "InvalidOperation" : "InvalidArgument";
                default:
                    foreach (KeyValuePair<int, Hashtable> kvp in Metadata.ErrorCategoryV1)
                    {

                        if (kvp.Key == statusCode)
                        {
                            Hashtable nested = (Hashtable)Metadata.ErrorCategoryV1[kvp.Key];
                            if (nested.ContainsKey(errorCode))
                            {
                                return nested[errorCode].ToString();
                            }

                        }
                    }
                    break;
            }
            
            return "NotSpecified";
        }

        public static PSObject GetVariable(string variableName)
        {
            using (PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                Collection<PSObject> variableInfo = pwsh.AddCommand("Get-Variable").AddParameter("Name", variableName).AddParameter("ValueOnly", true).Invoke();
                foreach (PSObject variable in variableInfo)
                {
                    return variable;                    
                }
            }
            return null;
        }
        
        public static void SetVariable(string variableName, object value, string scope)
        {
            using (PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                Collection<PSObject> variableInfo = pwsh.AddCommand("Set-Variable")
                    .AddParameter("Name", variableName)
                    .AddParameter("Value", value)
                    .AddParameter("Scope", scope).Invoke();                
            }
        }
    }
}
