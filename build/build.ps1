$Version = Get-Date -Format "yyyy-MM-dd" # 2020-11-1
$VersionDot = $Version -replace '-','.'

# Dotnet restore and build
dotnet publish "$PSScriptRoot\..\src\Hygrometer.InfluxDB.Collector.csproj" `
	   --runtime win-x64 `
	   --self-contained false `
	   -c Release `
	   -v minimal `
	   -o ./win-x64 `
	   -f net9.0 `
	   -p:PublishReadyToRun=false `
	   -p:PublishSingleFile=true `
	   -p:CopyOutputSymbolsToPublishDirectory=false `
	   -p:Version=$VersionDot `
	   --nologo
	   
dotnet publish "$PSScriptRoot\..\src\Hygrometer.InfluxDB.Collector.csproj" `
	   --runtime linux-arm64 `
	   --self-contained false `
	   -c Release `
	   -v minimal `
	   -o ./linux-arm64 `
	   -f net9.0 `
	   -p:PublishReadyToRun=false `
	   -p:PublishSingleFile=true `
	   -p:CopyOutputSymbolsToPublishDirectory=false `
	   -p:Version=$VersionDot `
	   --nologo
