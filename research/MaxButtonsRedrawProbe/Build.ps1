param(
    [Parameter(Mandatory=$true)][string]$GameRoot,
    [Parameter(Mandatory=$true)][string]$OutputPath,
    [Parameter(Mandatory=$true)][string]$CompilerPath
)
$ErrorActionPreference = 'Stop'
$managed = Join-Path $GameRoot 'Graveyard Keeper_Data/Managed'
$refs = @('mscorlib.dll','System.dll','System.Core.dll','Assembly-CSharp.dll','Assembly-CSharp-firstpass.dll','Rewired_Core.dll','UnityEngine.dll','UnityEngine.CoreModule.dll') | ForEach-Object { '/reference:' + (Join-Path $managed $_) }
$refs += @('BepInEx.dll','0Harmony.dll') | ForEach-Object { '/reference:' + (Join-Path $GameRoot ('BepInEx/core/' + $_)) }
$source = Join-Path $PSScriptRoot 'Probe.cs'
& dotnet $CompilerPath /nologo /target:library /nostdlib+ /noconfig /langversion:7.3 /warnaserror+ /optimize+ /deterministic+ ('/out:' + $OutputPath) @refs $source
if ($LASTEXITCODE -ne 0) { throw 'Native-reference diagnostic build failed' }
