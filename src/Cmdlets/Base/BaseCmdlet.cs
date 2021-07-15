using System.Collections.Generic;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using BluebirdPS.Core;
using BluebirdPS.Models;

namespace BluebirdPS.Cmdlets.Base
{
    public abstract class BaseCmdlet : PSCmdlet
    {
        private Configuration _configuration;
        private protected Configuration Configuration => _configuration ??= Config.GetOrCreateInstance();

        private List<ResponseData> _history;
        private protected List<ResponseData> History => _history ??= Core.History.GetOrCreateInstance();

        private MyState _sharedState;
        private protected MyState SharedState => _sharedState ??= MyState.GetForCmdlet(this);
    }

    internal sealed class MyState
    {
        private static readonly ConditionalWeakTable<PSModuleInfo, MyState> s_map = new();
        public static MyState GetForCmdlet(PSCmdlet cmdlet)
            => s_map.GetValue(cmdlet.MyInvocation.MyCommand.Module, static _ => new());

        public string Name { get; set; }

    }
}
