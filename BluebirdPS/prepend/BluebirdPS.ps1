using namespace System.Collections
using namespace System.Collections.Generic
using namespace Collections.ObjectModel
using namespace System.Management.Automation
using namespace System.Diagnostics.CodeAnalysis
using namespace Microsoft.PowerShell.Commands

using namespace BluebirdPS.Core
using namespace BluebirdPS.Models
using namespace BluebirdPS.Models.APIV1
using namespace BluebirdPS.Models.APIV2
using namespace BluebirdPS.Models.APIV2.Metrics
using namespace BluebirdPS.Models.APIV2.Metrics.Media
using namespace BluebirdPS.Models.APIV2.Metrics.Tweet
using namespace BluebirdPS.Models.APIV2.Metrics.User

# --------------------------------------------------------------------------------------------------

#region set base path variables
if ($IsWindows) {
    $DefaultSavePath = Join-Path -Path $env:USERPROFILE -ChildPath '.BluebirdPS'
} else {
    $DefaultSavePath = Join-Path -Path $env:HOME -ChildPath '.BluebirdPS'
}
#endregion

#region Authentication variables and setup
[SuppressMessage('PSUseDeclaredVarsMoreThanAssigments', '')]
$script:OAuth =  @{
    ApiKey = $null
    ApiSecret = $null
    AccessToken = $null
    AccessTokenSecret = $null
    BearerToken = $null
}
#endregion

#region BluebirdPS configuration variable
[SuppressMessage('PSUseDeclaredVarsMoreThanAssigments', '')]
$BluebirdPSConfiguration = [Configuration]@{
    ConfigurationPath = Join-Path -Path $DefaultSavePath -ChildPath 'Configuration.json'
    CredentialsPath = Join-Path -Path $DefaultSavePath -ChildPath 'twittercred.sav'
}
#endregion

#region other variables
[SuppressMessage('PSUseDeclaredVarsMoreThanAssigments', '')]
$BluebirdPSHistoryList = [List[ResponseData]]::new()
#endregion
