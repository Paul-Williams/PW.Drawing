@ECHO OFF
pushd "%~dp0"

ECHO Publishing
dotnet nuget push "bin/Release/PW.Drawing.1.0.2.nupkg" --api-key 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 --source "Personal"
