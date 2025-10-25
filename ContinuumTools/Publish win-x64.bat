::dotnet publish "ContinuumTools.sln" -c Release -r win-x64 --self-contained -p:UseAppHost=true -p:PublishAot=true -p:PublishSingleFile=true
dotnet publish "ContinuumTools.sln" -c Release -r win-x64 --self-contained -p:PublishSingleFile=True -p:PublishTrimmed=True -p:TrimMode=CopyUsed

pause