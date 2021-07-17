BuildHelpers\Set-BuildEnvironment -Force

[ordered]@{
    OutputFolder = 'Output'
    CopyFolders = @()
    CopyFiles = @('LICENSE')
    NestedModules = @()
    RootModule = "$env:BHProjectName.dll"

    DotNetProjects = @($env:BHProjectName)
    DotNetSrcFolder = 'src'
    OutputLibFolder = 'lib'

    HelpMarkdownSource = 'docs'
    ExternalHelpPath = (Get-UICulture).Name

    ChangeLogUri = 'https://docs.bluebirdps.dev/en/latest/CHANGELOG/'

    PublishToTestNuget = @('DockerNuGet')
    PublishToPSGallery = $false
}
