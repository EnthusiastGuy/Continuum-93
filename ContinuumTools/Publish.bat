@echo off
setlocal

:: Get the start time
set "start=%TIME%"

:: Windows 64 bit
	dotnet publish "ContinuumTools.sln" -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:UseAppHost=true

:: Windows 32 bit
	dotnet publish "ContinuumTools.sln" -c Release -r win-x86 --self-contained -p:PublishSingleFile=true -p:UseAppHost=true


:: Linux 64-bit
	dotnet publish "ContinuumTools.sln" -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -p:UseAppHost=true

:: Linux on ARM32
	dotnet publish "ContinuumTools.sln" -c Release -r linux-arm --self-contained -p:PublishSingleFile=true -p:UseAppHost=true

:: Linux on ARM64
	dotnet publish "ContinuumTools.sln" -c Release -r linux-arm64 --self-contained -p:PublishSingleFile=true -p:UseAppHost=true
	
:: macOS 64-bit (for Intel Macs)
	dotnet publish "ContinuumTools.sln" -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true -p:UseAppHost=true

:: macOS ARM64 (for Apple Silicon Macs)
	dotnet publish "ContinuumTools.sln" -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true -p:UseAppHost=true
	
rem iOS on ARM64 devices
	::dotnet publish "ContinuumTools.sln" -c Release -r ios-arm64 --self-contained true
	
rem iOS simulator on x64
	::dotnet publish "ContinuumTools.sln" -c Release -r ios-x64 --self-contained true

rem Android on x86 devices
	::dotnet publish "ContinuumTools.sln" -c Release -r android-x86 --self-contained true

rem Android on x64 devices
	::dotnet publish "ContinuumTools.sln" -c Release -r android-x64 --self-contained true

rem Android on ARM32 devices
	::dotnet publish "ContinuumTools.sln" -c Release -r android-arm --self-contained true

rem Android on ARM64 devices
	::dotnet publish "ContinuumTools.sln" -c Release -r android-arm64 --self-contained true

rem WebAssembly for browser applications
	::dotnet publish "ContinuumTools.sln" -c Release -r browser-wasm --self-contained true


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
	