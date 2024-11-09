# Publishing for Linux

1. Update the version and potentially target framework in the files
2. Publish the app for x64 and ARM with the following commands:
```
dotnet publish "./TeamSketch.csproj" --verbosity quiet --nologo --configuration Release --self-contained true --runtime linux-x64 --output "./bin/Release/net8.0/linux-x64"
dotnet publish "./TeamSketch.csproj" --verbosity quiet --nologo --configuration Release --self-contained true --runtime linux-arm --output "./bin/Release/net8.0/linux-arm"
```
3. Create the deployment files by running the PowerShell scripts
4. Move the produced files from the `output` folder to WSL and run the Debian packaging commands:

```
dpkg-deb --root-owner-group --build team-sketch_*_amd64
dpkg-deb --root-owner-group --build team-sketch_*_armhf
```

Follow the [official Avalonia UI guidance](https://docs.avaloniaui.net/docs/deployment/debian-ubuntu) for more details.
