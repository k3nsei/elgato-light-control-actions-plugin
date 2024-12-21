#!/usr/bin/env pwsh

$pkgPath = [System.IO.Path]::Combine($env:USERPROFILE, 'Downloads', 'ElgatoLightControlPlugin.zip')
$tmpDir = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), [guid]::NewGuid().ToString())
$outDir = [System.IO.Path]::Combine($tmpDir, 'ElgatoLightControlPlugin')

$cleanup = {
  Remove-Item -Recurse -Force -Path $tmpDir
}

try {
  if (-not (Test-Path -Path $outDir)) {
    New-Item -ItemType Directory -Path $outDir | Out-Null
  }

  Add-Type -AssemblyName System.IO.Compression.FileSystem

  [System.IO.Compression.ZipFile]::ExtractToDirectory($pkgPath, $outDir)

  $plgPath = [System.IO.Path]::Combine($outDir, 'ElgatoLightControlPlugin.lplug4')
  $dstDir = [System.IO.Path]::Combine($env:LOCALAPPDATA, 'Logi', 'LogiPluginService', 'Plugins', 'ElgatoLightControl')

  if (-not (Test-Path -Path $dstDir)) {
    New-Item -ItemType Directory -Path $dstDir | Out-Null
  }

  if ((Get-ChildItem -Path $dstDir | Measure-Object).Count -gt 0) {
    Remove-Item -Recurse -Force -Path (Get-ChildItem -Path $dstDir).FullName
  }

  [System.IO.Compression.ZipFile]::ExtractToDirectory($plgPath, $dstDir)

  Write-Output "Plugin installation complete."
  Write-Output "The plugin has been installed to: $dstDir"
}
finally {
  & $cleanup
}
