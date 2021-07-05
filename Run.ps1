Write-Host "Starting up all necessary processes"

$processes = [System.Collections.ArrayList]::new()

function Start-ELT-Job($desc, $wd, $cmd) { 
    Write-Host "Starting $desc"
    $proc = Start-Process "cmd.exe" "/c $cmd" -PassThru -WorkingDirectory $wd
    $null = $processes.Add($proc)
}

Start-ELT-Job "ELT discovery API" .\Eso.API.Discovery "Run.bat"
Start-ELT-Job "ELT statistics API" .\Eso.API.Statistics "Run-Stats.bat"
Start-ELT-Job "ELT statistics worker" .\Eso.API.Statistics "Run-Workers.bat"
Start-ELT-Job "ELT Editor API" .\Eso.API.Editor "dotnet run Eso.API.Editor.csproj"
Start-ELT-Job "ELT Execution API" .\Eso.API "dotnet run Eso.API.Execution.csproj"
Write-Host "Allow processes to start"
Start-Sleep -s 20
Start-ELT-Job "ELT Blazor Client" .\Eso.Blazor.SPA\Client "dotnet run Eso.Blazor.SPA.Client.csproj"
Write-Host "Start default browser on Blazor client port"
Start-Process "http://localhost:27000"
Read-Host "Press ENTER to end all processes ($($processes.Count) to end)"
Stop-Process -InputObject $processes.ToArray() -Force
