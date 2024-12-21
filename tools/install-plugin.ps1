#!/usr/bin/env pwsh

# Define the path to the Downloads directory and the Temp directory
$downloadsPath = [System.IO.Path]::Combine($env:USERPROFILE, 'Downloads')
$tempPath = [System.IO.Path]::GetTempPath()

# Define the path to the zip file and the destination directory
$zipFilePath = [System.IO.Path]::Combine($downloadsPath, 'ElgatoLightControlPlugin.zip')
$tempExtractionPath = [System.IO.Path]::Combine($tempPath, 'ElgatoLightControlPlugin')

# Create the temp extraction directory if it doesn't exist
if (-not (Test-Path -Path $tempExtractionPath)) {
    New-Item -ItemType Directory -Path $tempExtractionPath | Out-Null
}

# Extract the zip file to the temp extraction directory
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory($zipFilePath, $tempExtractionPath)

# Define the path to the .lplug4 file and the final destination directory
$lplug4FilePath = [System.IO.Path]::Combine($tempExtractionPath, 'ElgatoLightControlPlugin.lplug4')
$finalDestinationPath = [System.IO.Path]::Combine($env:LOCALAPPDATA, 'Logi', 'LogiPluginService', 'Plugins', 'ElgatoLightControl')

# Create the final destination directory if it doesn't exist
if (-not (Test-Path -Path $finalDestinationPath)) {
    New-Item -ItemType Directory -Path $finalDestinationPath | Out-Null
}

# Extract the .lplug4 file to the final destination directory
[System.IO.Compression.ZipFile]::ExtractToDirectory($lplug4FilePath, $finalDestinationPath)

Write-Output "Extraction complete. Files extracted to: $finalDestinationPath"
