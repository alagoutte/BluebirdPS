function Get-TwitterApiEndpoint {
    [CmdletBinding()]
    param(
        [Parameter()]
        [string[]]$CommandName,
        [ValidateNotNullOrEmpty()]
        [string]$Endpoint
    )

    if ($PSBoundParameters.ContainsKey('Endpoint')) {
        $TwitterEndpoints | Where-Object {$_.ApiEndpoint -match $Endpoint -and $_.CommandName -ne 'Get-TwitterApiEndpoint'} | Sort-Object -Property Visibility
    } else {
        $TwitterEndpoints | Where-Object {$_.ApiEndpoint.Count -gt 0 -and $_.CommandName -ne 'Get-TwitterApiEndpoint'} | Sort-Object -Property Visibility
    }

}
