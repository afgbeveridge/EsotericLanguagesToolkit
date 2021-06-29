
Write-Output "Esoteric language toolkit installer"

function Wait-For-Port([Int] $port, [Int] $awp) { 
    $open = $false
    $attempts = 1
    Write-Output "Check for accessible port $port...." 
    do { 
        $open = (Test-NetConnection -ComputerName localhost -Port $port | Where-Object TcpTestSucceeded) -ne $null
        if ($open -eq $true) {
            Write-Output "Port $port seems open...." 
        }
        else {
            Write-Output "Attempt $attempts - waiting for port $port...." 
            Start-Sleep -s 5
        }
        $attempts++
    } while ($open -eq $false -and $attempts -lt 13)
    if ($open -eq $false) { 
        throw "Port $port is required to be open but is not"
    }
    else { 
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
    docker run -d -p 3306:3306 --name elt-eso-mysql -e MYSQL_ROOT_PASSWORD=pass123 -d mysql:latest
}

function RabbitMQ-Creator { 
    docker run -d -p 15672:15672 -p 5672:5672 --hostname elt-local-rabbit --name elt-local-rabbit rabbitmq:3-management
    Write-Output "Create queues and bindings...."
    docker cp rabbit_definitions.json elt-local-rabbit:/rabbit_definitions.json
    Write-Output "Wait for RabbitMQ to spin up...."
    Wait-For-Port 15672 45
    docker exec -ti elt-local-rabbit sh -c "rabbitmqadmin import rabbit_definitions.json"
}

#Build-Solution
#Publish-Solution
Generic-Container-Operation rabbitmq elt-local-rabbit "RabbitMQ-Creator"
Generic-Container-Operation mysql elt-eso-mysql "MySql-Container-Creator"
