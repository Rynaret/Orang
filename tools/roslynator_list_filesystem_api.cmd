@echo off

set _roslynatorPath=..\..\Roslynator\src
set _msbuildPath="C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin"

%_msbuildPath%\msbuild "%_roslynatorPath%\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

"%_roslynatorPath%\CommandLine\bin\Debug\net472\roslynator" list-symbols "..\src\FileSystem\FileSystem.csproj" ^
 --msbuild-path %_msbuildPath% ^
 --depth member ^
 --visibility public ^
 --empty-line-between-members ^
 --ignored-parts containing-namespace assemblies assembly-attributes ^
 --output orang_filesystem_api.txt ^
 --verbosity d ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag

pause
