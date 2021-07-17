properties { }

Set-Variable -Name Build -Force -Scope Script -Value (. ([IO.Path]::Combine($PSScriptRoot, 'build','settings.ps1')) )

$ModuleVersion = (Import-PowerShellDataFile -Path $env:BHPSModuleManifest).ModuleVersion
$Build.OutputFolder = [IO.Path]::Combine($env:BHProjectPath, $Build.OutputFolder, $env:BHProjectName, $ModuleVersion)
$Build.DotNetSrcFolder = [IO.Path]::Combine($env:BHProjectPath, $Build.DotNetSrcFolder)
$Build.OutputLibFolder = [IO.Path]::Combine($Build.OutputFolder, $Build.OutputLibFolder)
$Build.HelpMarkdownSource = [IO.Path]::Combine($env:BHProjectPath, $Build.HelpMarkdownSource)
$Build.ExternalHelpPath = [IO.Path]::Combine($Build.OutputFolder, $Build.ExternalHelpPath)

$Build.Add('OutputManifestPath',[IO.Path]::Combine($Build.OutputFolder,"$env:BHProjectName.psd1"))
$Build.Add('ModuleVersion', $ModuleVersion)
$Build.Add('FileListParentFolder', ('{0}{1}' -f $Build.OutputFolder,[IO.Path]::DirectorySeparatorChar))

$VarFormat = '{0,-35}{1}'

function UpdateManifest {
    param([Hashtable]$ManifestProperties)
    do {
        try {
            Update-ModuleManifest @ManifestProperties
            $Retry = $false
        }
        catch {
            $Retry = $true
        }
    } while ($Retry)
}

FormatTaskName {
    param($taskName)
    Write-Host 'Task: ' -ForegroundColor Cyan -NoNewline
    Write-Host $taskName.ToUpper() -ForegroundColor Blue
}

task Init {
    Set-BuildEnvironment -BuildOutput $Build.OutputFolder -Force
    Write-Host 'Build System Details:' -ForegroundColor Yellow
    $VarFormat -f 'PowerShell Version',$PSVersionTable.PSVersion.ToString()
    Write-Host ''

    Write-Host 'Build Settings:' -ForegroundColor Yellow
    foreach ($Setting in $Build.Keys) {
        $VarFormat -f $Setting, ($Build[$Setting] -join ',')
    }
    Write-Host ''

    Write-Host 'BuildHelpers Settings:' -ForegroundColor Yellow
    (Get-Item ENV:BH*).Foreach({
        $VarFormat -f $_.name, $_.value
    })
    Write-Host ''

} -Description 'Initializes BuildHelpers and build system'

task CleanOutputFolder -Depends Init {
    if (Test-Path $Build.OutputFolder) {
        Remove-Item -Path $Build.OutputFolder -Recurse -Force -Verbose:$false
    }
    New-Item -Path $Build.OutputFolder -ItemType Directory > $null

} -Description 'Removes contents and creates output folder'

task CopyFiles -Depends CleanOutputFolder {
    $Manifest = Get-ChildItem -Path $env:BHPSModuleManifest -ErrorAction Ignore
    if ($Manifest) {
        $VarFormat -f 'Copying manifest file', $Manifest.Name | Write-Host
        Copy-Item -Path $Manifest -Destination $Build.OutputFolder -Force
    } else {
        throw "Module manifest not found: $env:BHPSModuleManifest"
    }

    foreach ($File in $Build.CopyFiles) {
        $FileToCopy = Get-ChildItem -Path $env:BHProjectPath -Filter $File
        if ($FileToCopy) {
            $VarFormat  -f 'Copying file',$FileToCopy.Name | Write-Host
            Copy-Item -Path $FileToCopy -Destination $Build.OutputFolder -Force
        } else {
            throw "File not found: $File"
        }
    }
    Write-Host ''
} -Description 'Copy list of files to output folder'

task CopyFolders -Depends CleanOutputFolder {
    foreach ($Folder in $Build.CopyFolders) {
        $VarFormat -f 'Copying folder',$Folder | Write-Host
        $FolderToCopy = Get-ChildItem -Path $env:BHProjectPath -Filter $Folder -Recurse -ErrorAction Ignore
        if ($FolderToCopy) {
            $OutputCopyFolderPath = [IO.Path]::Combine($Build.OutputFolder,$Folder)
            if (-Not (Test-Path -Path $OutputCopyFolderPath)) {
                $VarFormat -f '','creating folder' | Write-Host
                New-Item -Path $OutputCopyFolderPath -ItemType Directory > $null
            }
            $VarFormat -f '','copying folder contents' | Write-Host
            Get-ChildItem -Path $FolderToCopy -File | Copy-Item -Destination $OutputCopyFolderPath
        } else {
            throw "Folder not found: $Folder"
        }
    }
    Write-Host ''
} -Description 'Copy contents of folder list to output folder, as-is'

task DotNetPublish -Depends CleanOutputFolder {
    Write-Host 'Cleaning source folder'
    dotnet clean $Build.DotNetSrcFolder --nologo --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-Error -Message 'DotNetPublish task failed on clean' -ErrorAction Stop
    }
    Write-Host ''

    foreach ($Project in $Build.DotNetProjects) {
        $ProjectFile = [IO.Path]::Combine($Build.DotNetSrcFolder,"$Project.csproj")

        Write-Host "Publishing project $Project"
        dotnet publish $ProjectFile -o $Build.OutputLibFolder --nologo
        if ($LASTEXITCODE -ne 0) {
            Write-Error -Message 'DotNetPublish task failed on publish' -ErrorAction Stop
        }
    }
    Write-Host ''

} -Description 'Compile and publish .Net libraries'

task AddLibrariesToManifest -Depends DotNetPublish,CopyFiles {
    Write-Host 'Adding .NET libraries to manifest'
    Write-Host ''

    if ($Build.DotNetProjects.Count -gt 0) {
        if ($Build.NestedModules.Count) {
            foreach ($Nested in $Build.NestedModules) {
                $Library = Get-ChildItem -Path $Build.OutputLibFolder -Recurse -Filter $Nested -ErrorAction Ignore
                $LibraryList = [System.Collections.Generic.List[string]]::new()
                if ($Library) {
                    $VarFormat -f 'Found nested library',$Nested
                    $LibraryRelativePath = $Library.FullName.Replace($Build.FileListParentFolder,'')
                    $LibraryList.Add($LibraryRelativePath)
                } else {
                    throw "Nested library not found: $Nested"
                }
            }
            if ($LibraryList.Count -gt 0) {
                UpdateManifest -ManifestProperties @{ Path = $Build.OutputManifestPath;  NestedModules = ($LibraryList -join ',') }
            }
        }

        if ($Build.RootModule -match '\.dll$') {
            $RootModule = Get-ChildItem -Path $Build.OutputLibFolder -Recurse -Filter $Build.RootModule -ErrorAction Ignore
            if ($RootModule) {
                $VarFormat -f 'Found root module library',$Build.RootModule
                $RootModuleRelativePath = $RootModule.FullName.Replace($Build.FileListParentFolder,'')
                UpdateManifest -ManifestProperties @{ Path = $Build.OutputManifestPath; RootModule = $RootModuleRelativePath }
            } else {
                throw "Root module library not found: $Nested"
            }
        }
    }
    Write-Host ''

} -Description 'Add .Net libraries to manifest'

task AddCmdletsToExportToManifest -Depends DotNetPublish,CopyFiles {
    Write-Host 'Adding cmdlets list to manifest'
    Write-Host ''

    if ($Build.DotNetProjects.Count -gt 0) {
        if ($Build.RootModule -match '\.dll$') {
            $RootModule = Get-ChildItem -Path $Build.OutputLibFolder -Recurse -Filter $Build.RootModule -ErrorAction Ignore
            if ($RootModule) {
                Import-Module ([IO.Path]::Combine($PSScriptRoot, 'build','BuildFunctions.psm1'))

                $VarFormat -f 'Importing root module',$RootModule.BaseName

                $StartProcessParams = @{
                    Path = (Get-Command -Name 'pwsh').Source
                    Arguments = '-nologo',
                        "-Command &{ Import-Module $RootModule; (Get-Command -Module BluebirdPS -ListImported | Where-Object CommandType -eq 'Cmdlet').Name }"
                }
                $ProcessResults = Start-ProcessWithOutput @StartProcessParams

                if ($ProcessResults.Output) {
                    $CmdletList = $ProcessResults.Output.Split([System.Environment]::NewLine)
                    UpdateManifest -ManifestProperties @{ Path = $Build.OutputManifestPath; CmdletsToExport = $CmdletList }
                }

            } else {
                throw "Root module library not found: $Nested"
            }
        }
    }
    Write-Host ''

} -Description 'Add cmdlet list to manifest'

task AddFileListToManifest -Depends CopyFiles {
    Write-Host 'Adding file list to manifest'

    $FileList = Get-ChildItem -Path $env:BHBuildOutput -File -Recurse | ForEach-Object {
        $_.FullName.Replace($Build.FileListParentFolder,'')
    }
    UpdateManifest -ManifestProperties @{ Path = $Build.OutputManifestPath; FileList = $FileList }
    Write-Host ''

} -Description 'Add files list to module manifest'

task GenerateExternalHelp -Depends CleanOutputFolder {
    Write-Host 'Generating or updating external help'
    $NewExternalHelpParams = @{
        Path = Get-ChildItem -Path $Build.HelpMarkdownSource -Include '*-*.md' -Recurse
        OutputPath = $Build.ExternalHelpPath
        Force = $true
    }
    New-ExternalHelp @NewExternalHelpParams > $null
    Write-Host ''

} -Description 'Generate or updates MAML-based help from PlatyPS markdown files'

task GenerateContextualHelp -Depends CleanOutputFolder {
    Write-Host 'Generating or updating contextual help'
    $AboutHelpMarkdown = Get-ChildItem -Path $([IO.Path]::Combine($Build.HelpMarkdownSource,"about_*.md"))
    foreach ($Help in $AboutHelpMarkdown) {
        $NewAboutHelpParams = @{
            Path = $Help.FullName
            OutputPath = $Build.ExternalHelpPath
            Force = $true
        }
        New-ExternalHelp @NewAboutHelpParams > $null
    }
    Write-Host ''

} -Description 'Generate or updates contextual help from PlatyPS markdown files'

task GenerateMarkdownHelp {
    if (-Not (Test-Path -Path $Build.HelpMarkdownSource)) {
        'Creating markdown help folder'
        New-Item -Path $Build.HelpMarkdownSource -Directory > $null
    }

    # get functions and cmdlets

} -Description 'Generate new markdown help files with PlatyPS'

task TestHelp -Depends GenerateExternalHelp,GenerateContextualHelp {

} -Description 'Performs basic tests on the help files'

task UpdateChangeLog {

} -Description ''

task UpdateReleaseNotes -Depends UpdateChangeLog {

} -Description ''

task CreateReleaseAsset -Depends UpdateChangeLog {

} -Description ''

task PublishReleaseToGitHub -Depends CreateReleaseAsset {

} -Description 'Publish a release to GitHub'

task PublishModuleToTestNuget -Depends '' {


} -Description 'Publish module to test nuget repository'

task PublishModuleToPSGallery -Depends '' {


} -Description 'Publish module to PowerShell Gallery'



$FullBuild = 'CopyFiles','CopyFolders','DotNetPublish',
             'AddLibrariesToManifest','AddFileListToManifest','AddCmdletsToExportToManifest',
             'GenerateExternalHelp','GenerateContextualHelp'

task DotNet -Depends DotNetPublish
task NewMarkdownHelp
task Build -Depends $FullBuild

task default -Depends Build