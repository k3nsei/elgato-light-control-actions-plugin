#!/usr/bin/env pwsh

param (
    [Parameter(Mandatory=$false, ValueFromPipeline=$true)]
    [string] $SdkUrl = "https://marketplace.logi.com/resources/20/Logi_Plugin_Tool_Win_6_0_1_20790_ccd09903f8.zip"
)

$downloadUrl = $SdkUrl

$dir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$tempDir = [System.IO.Path]::GetTempPath()

$pkgPath = [System.IO.Path]::Combine($tempDir, "LogiPluginSdkTools.zip")
$outPath = "$dir\LogiPluginSdkTools"
$nestedDir = "$outPath\LogiPluginSdkTools"

# remove output directory if it exists
if (Test-Path -Path $outPath) {
    Remove-Item -Recurse -Force -Path $outPath
}

# recreate output directory
New-Item -ItemType Directory -Path $outPath

# download package file
Invoke-WebRequest -Uri $downloadUrl -OutFile $pkgPath

# extract downloaded package
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory($pkgPath, $outPath)

# remove downloaded package
Remove-Item -Path $pkgPath

# move the extracted files back to output directory if they are nested
if (Test-Path -Path $nestedDir) {
    Get-ChildItem -Path $nestedDir | Move-Item -Destination $outPath
    Remove-Item -Recurse -Force -Path $nestedDir
}
