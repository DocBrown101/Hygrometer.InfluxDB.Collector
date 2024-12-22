#! /bin/sh
dotnet publish ../src/Hygrometer.InfluxDB.Collector.csproj \
	   --runtime linux-arm64 \
	   --self-contained false \
	   -c Release \
	   -v minimal \
	   -o ./build \
	   -f net8.0 \
	   -p:PublishReadyToRun=false \
	   -p:PublishSingleFile=true \
	   -p:CopyOutputSymbolsToPublishDirectory=false \
	   --nologo
