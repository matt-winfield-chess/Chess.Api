SET targetDll=Chess.Api.Tests\bin\Debug\netcoreapp3.1\Chess.Api.Tests.dll
SET targetProject=Chess.Api.Tests\Chess.Api.Tests.csproj
SET excludeArgs=--exclude "[*]*Repository*" --exclude "[*]*Models*"

dotnet tool install --global coverlet.console
dotnet tool install -g dotnet-reportgenerator-globaltool

dotnet build
coverlet %targetDll% --target "dotnet" --targetargs "test %targetProject% --no-build" --format cobertura %excludeArgs% --exclude "[*]*Program" --exclude "[*]*Startup*"
reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html

START coveragereport\index.html