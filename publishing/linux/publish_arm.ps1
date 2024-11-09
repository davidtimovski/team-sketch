$Version = "0.9.0"
$RepoDir = "R:\repos\team-sketch"
$TargetFramework = "net8.0"

cd "${RepoDir}\publishing\linux"

New-Item -Path . -Name "output\team-sketch_${Version}_armhf" -ItemType "directory"
Copy-Item -Path "${RepoDir}\publishing\linux\template_arm\*" -Destination "${RepoDir}\publishing\linux\output\team-sketch_${Version}_armhf\" -Recurse

New-Item -Path . -Name "output\team-sketch_${Version}_armhf\usr\lib\team-sketch" -ItemType "directory"
Copy-Item "${RepoDir}\src\TeamSketch\bin\Release\${TargetFramework}\linux-arm\*" -Destination "${RepoDir}\publishing\linux\output\team-sketch_${Version}_armhf\usr\lib\team-sketch\"
