dotnet publish "Continuum Emulator.sln" -c Release -r win-x64 --self-contained -p:UseAppHost=true -p:PublishAot=true
rem dotnet publish "Continuum Emulator.sln" -c Release -r win-x64 --self-contained -p:UseAppHost=true -p:PublishAot=true -p:PublishSingleFile=true
rem dotnet publish "Continuum Emulator.sln" -c Release -r win-x64 --self-contained -p:PublishSingleFile=True -p:PublishTrimmed=True -p:TrimMode=CopyUsed

pause