@echo off
for %%f in (protocol/*.proto) do (
	.\ProtoGen\protogen.exe -i:protocol\%%~nf.proto -o:%%~nf.cs
	copy %%~nf.cs ..\client\Assets\Scripts\Protocol_Generated\
	copy %%~nf.cs ..\server\src\Protocol_Generated\
	del %%~nf.cs
)
pause