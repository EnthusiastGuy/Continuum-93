@echo off
setlocal

:: Get the start time
set "start=%TIME%"

rem Windows 64 bit
	rem dotnet publish "Continuum Emulator.sln" -c Release -r win-x64 --self-contained true
	rem dotnet publish "Continuum Emulator.sln" -c Release -r win-x64 --self-contained true -o ./published
	rem dotnet publish "Continuum Emulator.sln" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./published
	rem dotnet publish "Continuum Emulator.sln" -c Release -r win-x64 --self-contained -p:PublishSingleFile=True -p:PublishTrimmed=True -p:TrimMode=CopyUsed -p:PublishReadyToRun=True
	dotnet publish "Continuum Emulator.sln" -c Release -r win-x64 --self-contained -p:UseAppHost=true -p:PublishAot=true

rem Windows 32 bit
	dotnet publish "Continuum Emulator.sln" -c Release -r win-x86 --self-contained -p:PublishSingleFile=true -p:UseAppHost=true

rem Linux 64-bit
	dotnet publish "Continuum Emulator.sln" -c Release -r linux-x64 --self-contained -p:UseAppHost=true -p:PublishAot=true

rem Linux on ARM32
	dotnet publish "Continuum Emulator.sln" -c Release -r linux-arm --self-contained -p:PublishSingleFile=true -p:UseAppHost=true

rem Linux on ARM64
	dotnet publish "Continuum Emulator.sln" -c Release -r linux-arm64 --self-contained -p:UseAppHost=true -p:PublishAot=true
	
rem macOS 64-bit (for Intel Macs)
	dotnet publish "Continuum Emulator.sln" -c Release -r osx-x64 --self-contained -p:UseAppHost=true -p:PublishAot=true

rem macOS ARM64 (for Apple Silicon Macs)
	dotnet publish "Continuum Emulator.sln" -c Release -r osx-arm64 --self-contained -p:UseAppHost=true -p:PublishAot=true
	
rem iOS on ARM64 devices
	::dotnet publish "Continuum Emulator.sln" -c Release -r ios-arm64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=CopyUsed -p:PublishReadyToRun=true -p:UseAppHost=true
	
rem iOS simulator on x64
	::dotnet publish "Continuum Emulator.sln" -c Release -r ios-x64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=CopyUsed -p:PublishReadyToRun=true -p:UseAppHost=true


:: android https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/publish-cli?view=net-maui-8.0
rem Android on x86 devices
	::dotnet publish "Continuum Emulator.sln" -c Release -r android-x86 --self-contained true -o ./published

rem Android on x64 devices
	::dotnet publish "Continuum Emulator.sln" -c Release -r android-x64 --self-contained true -o ./published

rem Android on ARM32 devices
	::dotnet publish "Continuum Emulator.sln" -c Release -r android-arm --self-contained true -o ./published

rem Android on ARM64 devices
	rem dotnet publish "Continuum Emulator.sln" -c Release -r android-arm64 --self-contained true -o ./published
	::dotnet publish "Continuum Emulator.sln" -c Release -r android-arm64 --self-contained true


rem WebAssembly for browser applications
	::dotnet publish "Continuum Emulator.sln" -c Release -r browser-wasm --self-contained true -o ./published


:: Get the end time
set "end=%TIME%"

:: Display the start and end times for reference
echo Start Time: %start%
echo End Time: %end%

:: Calculate the elapsed time
call :ElapsedTime "%start%" "%end%"
goto :eof

:ElapsedTime
    setlocal
    set "start=%~1"
    set "end=%~2"

    :: Convert start and end times to seconds
    set /A "startSeconds=(((1%start:~0,2%-100)*3600) + ((1%start:~3,2%-100)*60) + (1%start:~6,2%-100))"
    set /A "endSeconds=(((1%end:~0,2%-100)*3600) + ((1%end:~3,2%-100)*60) + (1%end:~6,2%-100))"

    :: Calculate the difference
    set /A "elapsedSeconds=endSeconds-startSeconds"
    
    :: Handle crossing midnight
    if %elapsedSeconds% lss 0 set /A elapsedSeconds+=86400

    :: Convert elapsed time back to hours, minutes, and seconds
    set /A "hours=elapsedSeconds/3600"
    set /A "minutes=(elapsedSeconds%%3600)/60"
    set /A "seconds=elapsedSeconds%%60"

    :: Display elapsed time
    echo Elapsed Time: %hours% hours, %minutes% minutes, %seconds% seconds
	pause
	
    endlocal & goto :eof
	
