
Write-Output "Esoteric language toolkit installer"

function Wait-For-Condition($desc, $test, [Int] $awp = 0, [Int] $maxAttempts = 13, [Int] $sleepWait = 5) { 
    $open = $false
    $attempts = 1
    Write-Output "Check for condition $desc....up to $maxAttempts attempts" 
    do { 
        $open = & $test
        if ($open -eq $true) {
            Write-Output "Condition $desc satisfied...." 
        }
        else {
            Write-Output "Attempt $attempts - waiting $sleepWait seconds...." 
            Start-Sleep -s $sleepWait
        }
        $attempts++
    } while ($open -eq $false -and $attempts -lt $maxAttempts)
    if ($open -eq $false) { 
        throw "Condition $desc not satisfied, cannot continue"
    }
    elseif ($awp -gt 0) { 
        Write-Output "Additional wait period of $awp requested...." 
        Start-Sleep -s $awp
    }
}

function Build-Solution {
    Write-Output "Building solution...."
    dotnet build
}

function Publish-Solution {
    Write-Output "Publishing solution...."
    dotnet publish
}

function Find-Container([String] $containerName) {
    Write-Host "Looking for container $containerName...."
    $dockerInfo = docker container ls -a
    $match = $dockerInfo | Select-String "$containerName"
    $running = $false
    $exists = $false
    if ($match -ne $null) { 
        $running = ($match | Select-String "Exited" | measure-object -line).Lines -eq 0
        $exists = $true
    }
    Write-Host "Does $containerName exist? $exists"
    Write-Host "Is $containerName running? $running"
    return $exists, $running
}

function Find-Image([String] $imageName) {
    Write-Host "Looking for image $imageName...."
    $exists = docker images | Select-String "$imageName"
    return $exists -ne $null
}


function Generic-Container-Operation([String] $imageName, [String] $containerName, $creator) { 
    Write-Output "$imageName checks...."
    $exists, $running = Find-Container($containerName)
    if ($exists -eq $false) { 
        $imageAlready = Find-Image $imageName
        if ($imageAlready -eq $false) { 
            Write-Output "Pull $imageName image...."
            docker pull $imageName
        }
        Write-Output "Create $imageName container...."
        & $creator
    }
    elseif ($running -eq $false) { 
        Write-Output "Start existing container...."
        docker start $containerName
    }
    else { 
        Write-Output "Container appears to be running...."
    }
}

function MySql-Container-Creator { 
    docker run -d -p 3306:3306 --name elt-eso-mysql -e MYSQL_ROOT_PASSWORD=pass123 -d mysql:latest --secure-file-priv=
    Wait-For-Condition "MySql up" { (docker exec elt-eso-mysql sh -c "mysqladmin  -u root --password=pass123 ping 2>&1" | Select-String alive) -ne $null } -sleepWait 30
}

function RabbitMQ-Creator { 
    docker run -d -p 15672:15672 -p 5672:5672 --hostname elt-local-rabbit --name elt-local-rabbit rabbitmq:3-management
    Write-Output "Create queues and bindings...."
    docker cp rabbit_definitions.json elt-local-rabbit:/rabbit_definitions.json
    Write-Output "Wait for RabbitMQ to spin up...."
    Wait-For-Condition "Rabbit port open" { (Test-NetConnection -ComputerName localhost -Port 15672 | Where-Object TcpTestSucceeded) -ne $null }
    Wait-For-Condition "Rabbit up" { docker exec -ti elt-local-rabbit sh -c "rabbitmqctl status"; Write-Output "$($LASTEXITCODE -eq 0)" }
    docker exec -ti elt-local-rabbit sh -c "rabbitmqadmin import rabbit_definitions.json"
}

function Mongo-Container-Creator { 
    docker volume create --name=eltmongodata
    docker run -p 27017:27017 --name elt-eso-mongo -v eltmongodata:/data/db -d mongo:latest
}

function Add-EF-Artefacts { 
    dotnet tool install --global dotnet-ef
    dotnet ef database update --project Eso.API.Editor 
}

function MySql-Seed { 
    docker cp Languages.csv elt-eso-mysql:/Languages.csv
    docker cp mysql.init.sql elt-eso-mysql:/mysql.init.sql
    docker exec -it elt-eso-mysql sh -c "mysql -u root --password=pass123 esoteric_languages < mysql.init.sql"
}

Generic-Container-Operation rabbitmq elt-local-rabbit { RabbitMQ-Creator }
Generic-Container-Operation mysql elt-eso-mysql { MySql-Container-Creator; Add-EF-Artefacts; MySql-Seed }
Generic-Container-Operation mongo elt-eso-mongo { Mongo-Container-Creator }
Build-Solution
Publish-Solution
