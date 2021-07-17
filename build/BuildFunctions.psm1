function Start-ProcessWithOutput {
    [CmdLetBinding()]
    param(
        [string]$Path,
        [string[]]$Arguments,
        [int]$Timeout
    )

    #https://stackoverflow.com/questions/11531068/powershell-capturing-standard-out-and-error-with-process-object

    'Attempting to create process start info' | Write-Verbose
    try {
        $ProcessStartInfo = [System.Diagnostics.ProcessStartInfo]::new()
        $ProcessStartInfo.CreateNoWindow = $true
        #$ProcessStartInfo.LoadUserProfile = $false
        $ProcessStartInfo.FileName = $Path
        $ProcessStartInfo.RedirectStandardOutput = $true
        $ProcessStartInfo.RedirectStandardError = $true
        $ProcessStartInfo.UseShellExecute = $false
        if ($Arguments.Count -gt 0) {
            $ProcessStartInfo.Arguments = $Arguments
        }
        'Successfully created process start info' | Write-Verbose
        $ProcessString = '{0} {1}' -f $ProcessStartInfo.FileName,$ProcessStartInfo.Arguments
        'Process: {0}' -f $ProcessString | Write-Verbose
    }
    catch {
        $PSCmdlet.ThrowTerminatingError($_)
    }

    $StandardOutput = [System.Text.StringBuilder]::new()
    $StandardError = [System.Text.StringBuilder]::new()

    try {
        $Process = [System.Diagnostics.Process]::new()
        $Process.StartInfo = $ProcessStartInfo

        $TimeoutReached = $null

        'Attempting to create process' | Write-Verbose
        [void]$Process.Start()
        $StartTime = Get-Date

        'Waiting for process to exit or timeout' | Write-Verbose
        while (!$Process.HasExited) {
            if ($Timeout) {
                if ($StartTime  -le (Get-Date).AddSeconds(-$Timeout)) {
                    try { [void]$Process.Kill() }
                    catch {}
                    [void]$Process.Refresh()
                    break
                }
            }
            if (!$Process.StandardOutput.EndOfStream) {
                [void]$StandardOutput.AppendLine($Process.StandardOutput.ReadLine())
            }
            if (!$Process.StandardError.EndOfStream) {
                [void]$StandardError.AppendLine($Process.StandardError.ReadLine())
            }
            Start-Sleep -Milliseconds 10
        }
    }
    catch {
        $PSCmdlet.ThrowTerminatingError($_)
    }

    'Process completed' | Write-Verbose

    if ($Timeout) {
        if ($StartTime -le (Get-Date).AddSeconds(-$Timeout)) {
            $TimeoutReached = $true
            [void]$StandardError.AppendLine('Process terminated due exceeded timeout')
            'Timeout of {0} seconds exceeded' -f $Timeout | Write-Verbose
        } else {
            $TimeoutReached = $false
            'Timeout of {0} seconds not exceeded' -f $Timeout | Write-Verbose
        }
    }

    while (!$Process.StandardOutput.EndOfStream) {
        [void]$StandardOutput.AppendLine($Process.StandardOutput.ReadLine())
    }
    while (!$Process.StandardError.EndOfStream) {
        [void]$StandardError.AppendLine($Process.StandardError.ReadLine())
    }

    [PsCustomObject]@{
        Process = $ProcessString
        Output = $StandardOutput.ToString()
        Error = $StandardError.ToString()
        StartTime = $StartTime
        ExitTime = $Process.ExitTime
        ElapsedTime = New-TimeSpan -Start $StartTime -End $Process.ExitTime
        TimeoutReached = $TimeoutReached
        ExitCode = $Process.ExitCode
    }
}

function Get-GitLog {
    [CmdLetBinding(DefaultParameterSetName='Default')]
    param (

        [Parameter(ParameterSetName='Default',ValueFromPipeline)]
        [Parameter(ParameterSetName='SourceTarget',ValueFromPipeline)]
        [ValidateScript({Resolve-Path -Path $_ | Test-Path})]
        [string]$GitFolder='.',

        [Parameter(ParameterSetName='SourceTarget',Mandatory)]
        [string]$StartCommitId,
        [Parameter(ParameterSetName='SourceTarget')]
        [string]$EndCommitId='HEAD'
    )

    Push-Location
    try {
        $GitPath = (Resolve-Path -Path $GitFolder).Path
        $GitCommand = Get-Command -Name git -ErrorAction Stop
        if ((Get-Location).Path -ne $GitPath) {
            Set-Location -Path $GitFolder
        }
        Write-Verbose -Message "Folder - $GitPath"
    }
    catch {
        $PSCmdlet.ThrowTerminatingError($_)
    }

    if ($StartCommitId) {
        $GitLogCommand = '"{0}" log --oneline --format="%H`t%h`t%ai`t%an`t%ae`t%ci`t%cn`t%ce`t%G?`t%s`t%f" {1}...{2} 2>&1' -f $GitCommand.Source,$StartCommitId,$EndCommitId
    } else {
        $GitLogCommand = '"{0}" log --oneline --format="%H`t%h`t%ai`t%an`t%ae`t%ci`t%cn`t%ce`t%G?`t%s`t%f" 2>&1' -f $GitCommand.Source
    }

    Write-Verbose -Message "Command - $GitLogCommand"
    $GitLog = Invoke-Expression -Command "& $GitLogCommand" -ErrorAction SilentlyContinue

    if ((Get-Location).Path -ne $GitPath) {
        Pop-Location
    }

    $GitLogFormat = 'CommitId',
        'ShortCommitId',
        'AuthorDate',
        'AuthorName',
        'AuthorEmail',
        'CommitterDate',
        'CommitterName',
        'ComitterEmail',
        @{label='CommitterSignature';expression={
            switch ($_.CommitterSignature) {
                'G' { 'Valid'}
                'B' { 'BadSignature'}
                'U' { 'GoodSignatureUnknownValidity'}
                'X' { 'GoodSignatureExpired'}
                'Y' { 'GoodSignatureExpiredKey'}
                'R' { 'GoodSignatureRevokedKey'}
                'E' { 'MissingKey'}
                'N' { 'NoSignature'}
            }
        }},
        'CommitMessage',
        'SafeCommitMessage'

    if ($GitLog[0] -notmatch 'fatal:') {
        $GitLog | ConvertFrom-Csv -Delimiter "`t" -Header 'CommitId','ShortCommitId','AuthorDate','AuthorName','AuthorEmail','CommitterDate','CommitterName','ComitterEmail','CommitterSignature','CommitMessage','SafeCommitMessage' | Select-Object $GitLogFormat
    } else {
        if ($GitLog[0] -like "fatal: ambiguous argument '*...*'*") {
            Write-Warning -Message 'Unknown revision. Please check the values for StartCommitId or EndCommitId; omit the parameters to retrieve the entire log.'
        } else {
            Write-Error -Category InvalidArgument -Message ($GitLog -join "`n")
        }
    }
    Pop-Location
}

function Get-ReleaseNotes {
    [CmdLetBinding()]
    param(
        [Parameter(Mandatory)]
        [ValidateScript({Test-Path -Path $_})]
        [string]$ChangeLogPath,
        [Parameter(Mandatory)]
        [uri]$ChangeLogUri
    )

    $FullChangeLogLocation = "For full CHANGELOG, see $ChangeLogUri" -f $ChangeLogUri.AbsoluteUri

    $ChangeLog = [System.Text.StringBuilder]::new()
    $Lines = Get-Content -Path $ChangeLogPath
    $Count = 0
    foreach ($Line in $Lines) {
        if ($Line -match '^## \[\d\.|^## \d\.') {
            if ($null -eq $LastReleaseBegin) {
                $LastReleaseBegin = $Count
            } elseif ($null -eq $LastReleaseEnd) {
                $LastReleaseEnd = $Count - 1
                break
            }
        }
        $Count++
    }
    [void]$ChangeLog.Append($Lines[$LastReleaseBegin..$LastReleaseEnd] -join [System.Environment]::NewLine)
    [void]$ChangeLog.AppendLine()

    [void]$ChangeLog.AppendLine($FullChangeLogLocation)

    $ChangeLog.ToString()
}

function Get-ChangeLogUpdate {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [ValidateScript({Test-Path -Path $_})]
        [string]$ProjectPath,
        [Parameter(Mandatory)]
        [ValidateSet('Bugfix','Security','Feature','Maintenance')]
        [string[]]$ReleaseType,
        [Parameter(Mandatory)]
        [ValidateSet('No Required','Recommended','Strongly Recommended')]
        [string]$UpdateRequired,
        [Parameter(Mandatory)]
        [string]$ProjectOwner,
        [Parameter(Mandatory)]
        [version]$TargetRelease,
        [uri]$ReleaseLink,
        [string]$TargetReleaseDate='Unreleased'
    )

    try {
        enum ChangeLogEntryType {
            Security; Deprecated; Removed; Fixed; Changed; Added; Maintenance
        }
        $ProjectPath = Resolve-Path -Path $ProjectPath
        $Project = Split-Path -Path $ProjectPath -Leaf
        $LastReleaseCommit = Get-GitHubRelease -OwnerName $ProjectOwner -RepositoryName $Project | Sort-Object -Property created_at -Descending | Select-Object -First 1
        if ($LastReleaseCommit) {
            $GitLog = Get-GitLog -GitFolder $ProjectPath -StartCommitId $LastReleaseCommit.target_commitish
        } else {
            $GitLog = Get-GitLog -GitFolder $ProjectPath
        }

    }
    catch {
        $PSCmdlet.ThrowTerminatingError($_)
    }

    $NewChangeLogEntry = [System.Text.StringBuilder]::new()
    [void]$NewChangeLogEntry.AppendLine()

    if ($ReleaseLink) {
        $TargetReleaseText = '## [{0}] - {1}' -f $TargetRelease.ToString(),$TargetReleaseDate
    } else {
        $TargetReleaseText = '## {0} - {1}' -f $TargetRelease.ToString(),$TargetReleaseDate
    }
    [void]$NewChangeLogEntry.AppendLine($TargetReleaseText)
    [void]$NewChangeLogEntry.AppendLine()

    switch ($ReleaseType.Count) {
        1 { $ReleaseTags = $ReleaseType[0] }
        2 { $ReleaseTags = '{0} and {1}' -f $ReleaseType[0],$ReleaseType[1] }
        3 { $ReleaseTags = '{0}, {1}, and {2}' -f $ReleaseType[0],$ReleaseType[1],$ReleaseType[2] }
        4 { $ReleaseTags = '{0}, {1}, {2}, and {3}' -f $ReleaseType[0],$ReleaseType[1],$ReleaseType[2],$ReleaseType[3] }
    }
    $ReleaseTypeText = '{0}; Update {1}' -f $ReleaseTags,$UpdateRequired
    [void]$NewChangeLogEntry.AppendLine($ReleaseTypeText)
    [void]$NewChangeLogEntry.AppendLine()

    $ChangeLogCommits = foreach ($Commit in $GitLog) {
        $IssueNumber = $GitHubIssue = $null
        $Action,$Message = $Commit.CommitMessage -Split ' '

        if ([ChangeLogEntryType].GetEnumNames() -match "^$Action") {
            $EntryType = [ChangeLogEntryType]$Action
        } else {
            $EntryType = 'Maintenance'
        }
        if ($Message -match '#') {
            $Issue = $Message -match '#'
            if ($Issue -is [boolean]) {
                $IssueNumber = $Message.Replace('#','')
            } else {
                $IssueNumber = $Issue.Replace('#','')
            }

            $GitHubIssue = Get-GitHubIssue -OwnerName $ProjectOwner -RepositoryName $Project -Issue $IssueNumber |
                Select-Object -Property number,html_url,title |
                Sort-Object -Property number
        }

        [PSCustomObject]@{
            ShortCommitId = $Commit.ShortCommitId
            CommitDate = $Commit.CommitDate
            EntryType = $EntryType
            CommitMessage = $Commit.CommitMessage
            GitHubIssue = $GitHubIssue
        }
    }
    $ChangeLogCommits | Out-String | Write-Verbose

    foreach ($EntryType in [ChangeLogEntryType].GetEnumNames()) {
        $SectionCommits = $ChangeLogCommits.Where({$_.EntryType -match $EntryType -and $_.GitHubIssue}) | Sort-Object -Property GitHubIssue.created_at,CommitterDate,CommitMessage

        if ($SectionCommits) {
            $SectionHeader = '### {0}' -f $EntryType
            [void]$NewChangeLogEntry.AppendLine($SectionHeader)
            [void]$NewChangeLogEntry.AppendLine()

            foreach ($Entry in $SectionCommits) {
                if ($Entry.GitHubIssue) {
                    $EntryText = '- [Issue #{0}]({1}) - {2}' -f $Entry.GitHubIssue.number,$Entry.GitHubIssue.html_url,$Entry.GitHubIssue.title
                }
                if (-Not $NewChangeLogEntry.ToString().Contains($EntryText)) {
                    $EntryText | Write-Verbose
                    [void]$NewChangeLogEntry.AppendLine($EntryText)
                }
            }
            [void]$NewChangeLogEntry.AppendLine()
        }
    }

    if ($ReleaseLink) {
        $ReleaseLinkText = '[{0}]: {1}' -f $TargetRelease.ToString(),$ReleaseLink.AbsoluteUri
        [void]$NewChangeLogEntry.AppendLine($ReleaseLinkText)
    }

    $NewChangeLogEntry.ToString()
}
