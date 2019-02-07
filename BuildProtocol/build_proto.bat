@echo off

echo start compiling protos

echo compiling theircraft.proto
.\protoc-3.6.1-win32\bin\protoc.exe protocol\theircraft.proto --csharp_out=.
move "Theircraft.cs" ..\Assets\Scripts\Protocol_Generated\Theircraft.cs

echo compile finish

pause