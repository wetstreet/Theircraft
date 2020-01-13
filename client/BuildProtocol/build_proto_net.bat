@echo off
for %%f in (protocol/*.proto) do (
	.\ProtoGen\protogen.exe -i:protocol\%%~nf.proto -o:%%~nf.cs
	move %%~nf.cs ..\Assets\Scripts\Protocol_Generated\%%~nf.cs
)
pause