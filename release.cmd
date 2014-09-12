@echo off

".\tools\NuGet\NuGet.exe" "install" "FAKE" "-OutputDirectory" ".\bin\tools" "-ExcludeVersion" "-Prerelease"
".\tools\Nuget\NuGet.exe" "install" "FSharp.Formatting.CommandTool" "-OutputDirectory" ".\bin\tools" "-ExcludeVersion" "-Prerelease"
".\tools\Nuget\NuGet.exe" "install" "SourceLink.Fake" "-OutputDirectory" ".\bin\tools" "-ExcludeVersion"

SET TARGET="Release"
SET NUGETKEY="965c768e-c7ac-4cf6-bf29-67e5db2cc121"

IF NOT [%1]==[] (set TARGET="%1")
IF NOT [%1]==[] (set NUGETKEY="%1")

".\bin\tools\FAKE\tools\Fake.exe" "build.fsx" "target=%TARGET%" "nugetkey=%NUGETKEY%"

PAUSE