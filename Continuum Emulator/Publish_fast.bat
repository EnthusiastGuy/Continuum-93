
::Windows 64 bit
dotnet publish "Continuum Emulator.sln" -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:UseAppHost=true


::dotnet publish "Continuum Emulator.sln" -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=CopyUsed -p:PublishReadyToRun=true -p:UseAppHost=true
