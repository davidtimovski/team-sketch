$Version = "0.9.0"
$RepoDir = "R:\repos\team-sketch"
$TargetFramework = "net8.0"

cd "${RepoDir}\publishing\linux"

New-Item -Path . -Name "output\team-sketch_${Version}_amd64" -ItemType "directory"
Copy-Item -Path "${RepoDir}\publishing\linux\template_64\*" -Destination "${RepoDir}\publishing\linux\output\team-sketch_${Version}_amd64\" -Recurse

New-Item -Path . -Name "output\team-sketch_${Version}_amd64\usr\lib\team-sketch" -ItemType "directory"
Copy-Item "${RepoDir}\src\TeamSketch\bin\Release\${TargetFramework}\linux-x64\*" -Destination "${RepoDir}\publishing\linux\output\team-sketch_${Version}_amd64\usr\lib\team-sketch\"
