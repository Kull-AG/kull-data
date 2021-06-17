Set-Location $PSScriptRoot
dotnet publish -o Deploy
if(Test-Path Deploy/runtimes){
    $xplatdlls = Get-ChildItem Deploy/runtimes -Recurse *.dll | Select-Object -ExpandProperty Name | Get-Unique
    $runtimes = Get-ChildItem Deploy/runtimes
    foreach($runtimeItem in $runtimes) {
        $runtime = $runtimeItem.Name
	    New-Item "Deploy/$runtime"  -ItemType "directory" -Force
        foreach($dll in $xplatdlls){
            if(Test-Path "Deploy/$dll") {
                Copy-Item "Deploy/$dll" -Destination Deploy/$runtime -Force
            }
            if($runtime.StartsWith("win-")){
                if(Test-Path "Deploy/runtimes/win/$dll") {
                    Copy-Item "Deploy/runtimes/win/$dll" -Destination Deploy/$runtime -Force
                }
            }
        }
	    Get-ChildItem  "Deploy/runtimes/$runtime" -Recurse *.dll  | Copy-Item -Destination Deploy/$runtime -Force
	
    }
    foreach($dll in $xplatdlls) {        
        if(Test-Path "Deploy/$dll") {
            Remove-Item "Deploy/$dll" -Force
        }
    }
    Remove-Item "Deploy/win" -Recurse -Force
    Remove-Item Deploy/runtimes -Recurse -Force
}

Copy-Item Kull.Data.Powershell.psd1 Deploy/Kull.Data.Powershell.psd1

$assembliesPlaceholder = "@('Assemblies')"

$deployPath = [IO.Path]::Combine( (Get-Location).Path, "Deploy").Replace("\", "/") + "/"
$names  = (Get-ChildItem Deploy *.dll -Recurse) | Select-Object @{ Name='prop'; Expression = { "'" + $_.FullName.Replace("\", "/").Substring($deployPath.Length) + "'" } } | Select-Object -ExpandProperty prop
$psar = "@(" + [string]::Join(", ", $names) + ")"
$psar = $psar.Replace(", 'System.Management.Automation.dll'", "")
$cnt =  Get-Content Deploy/Kull.Data.Powershell.psd1 

$cnt = $cnt.Replace($assembliesPlaceholder, $psar)

Set-Content -Value $cnt -Path Deploy/Kull.Data.Powershell.psd1 
Set-Location Deploy
Publish-Module -Name .\Kull.Data.Powershell.psd1 -NuGetApiKey ($env:Nuget_API_KEY)
Set-Location ..