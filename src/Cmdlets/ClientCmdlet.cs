using System.Management.Automation;
using AutoMapper;
using Tweetinvi;
using Mapper = BluebirdPS.Core.Mapper;

namespace BluebirdPS.Cmdlets
{
    public abstract class ClientCmdlet : AuthCmdlet
    {
        [Parameter()]
        public SwitchParameter NoPagination { get; set; }

        private protected IMapper mapper = Mapper.GetOrCreateInstance();

        private protected TwitterClient Client => Core.Client.Create(oauth, this);

    }
}
