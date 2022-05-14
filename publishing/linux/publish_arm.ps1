$Version = "0.6.0"

cd "C:\Users\david\source\repos\team-sketch\publishing\linux"

New-Item -Path . -Name "output\team-sketch_${Version}_armhf" -ItemType "directory"
Copy-Item -Path "C:\Users\david\source\repos\team-sketch\publishing\linux\template_arm\*" -Destination "C:\Users\david\source\repos\team-sketch\publishing\linux\output\team-sketch_${Version}_armhf\" -Recurse

Copy-Item "C:\Users\david\source\repos\team-sketch\publishing\linux\template_arm\DEBIAN\control" -Destination "C:\Users\david\source\repos\team-sketch\publishing\linux\output\team-sketch_${Version}_armhf\DEBIAN\control"

Copy-Item "C:\Users\david\source\repos\team-sketch\Src\TeamSketch\bin\Release\net6.0\publish\TeamSketch" -Destination "C:\Users\david\source\repos\team-sketch\publishing\linux\output\team-sketch_${Version}_armhf\usr\bin\team-sketch"
