using System.Collections.Generic;
using System.Management.Automation;
using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core.Processors;
using BluebirdPS.Models.APIV2;
using Tweetinvi.Models.V2;
using Tweetinvi.Exceptions;
using BluebirdPS.Core;
using Tweetinvi;

namespace BluebirdPS.Cmdlets.API.APIV2
{
    [Cmdlet(VerbsCommon.Get, "TwitterUser")]
    public class GetTwitterUserCommand : BluebirdPSUserCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        [ValidateCount(1, 100)]

        public List<string> User { get; set; }

        Dictionary<string, List<string>> userList = new Dictionary<string,List<string>>()
        {
            { "Names", new List<string>()  },
            { "Ids", new List<string>() }
        };

        protected override void BeginProcessing()
        {
            
        }
        protected override void ProcessRecord()
        {
            if (User != null)
            {
                foreach (string user in User)
                {
                    try
                    {
                        _ = long.Parse(user);
                        userList["Ids"].Add(user);
                    }
                    catch
                    {
                        userList["Names"].Add(user);
                    }
                }
            }
        }

        protected override void EndProcessing()
        {
            WriteVerbose($"Attempting to retrieve {userList["Names"].Count + userList["Ids"].Count} users.");

            List<object> results = GetUser(MyInvocation.BoundParameters, userList);
            WriteObject(results, false);
        }

        internal static List<object> GetUser(IDictionary<string, object> parameters, IDictionary<string, List<string>> userlist)
        {
            List<object> results = new List<object>();

            if (userlist["Names"].Count == 0 && userlist["Ids"].Count == 0)
            {
                try
                {
                    UserV2 result = client.UsersV2.GetUserByNameAsync(Metadata.Configuration.AuthUserName).GetAwaiter().GetResult().User;                    
                    results.Add(mapper.Map<User>(result));
                }
                catch (TwitterException ex)
                {
                    results.Add(GetTwitterErrorRecord.ProcessV2Error(ex));
                }
            }

            if (userlist["Names"].Count > 0)
            {
                try
                {
                    UsersV2Response apiResponse = client.UsersV2.GetUsersByNameAsync(string.Join(",", userlist["Names"])).GetAwaiter().GetResult();
                    foreach (UserV2 result in apiResponse.Users)
                    {
                        results.Add(mapper.Map<User>(result));
                    }
                }
                catch (TwitterException ex)
                {
                    results.Add(GetTwitterErrorRecord.ProcessV2Error(ex));
                }
            }

            if (userlist["Ids"].Count > 0)
            {
                try
                {
                    UsersV2Response apiResponse = client.UsersV2.GetUsersByIdAsync(string.Join(",", userlist["Ids"])).GetAwaiter().GetResult();
                    foreach (UserV2 result in apiResponse.Users)
                    {
                        results.Add(mapper.Map<User>(result));
                    }
                }
                catch (TwitterException ex)
                {
                    results.Add(GetTwitterErrorRecord.ProcessV2Error(ex));
                }

            }
            return results;
        }
    }
}
