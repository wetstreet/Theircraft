::公共参数
set fbPath=..\Flatbuffers
set jsonPath=./json
set fbsPath=./fbs
set binPath=./fb_bin 
set csPath=./fb_cs/
set scriptsPath = "../Assets/Scripts/Flatbuffer/Generate/"

:: 设置基础参数
set excelName="测试表格.xlsx"
set sheetName="test"

:: 调用excel2json转化成json,同时会转化出fbs文件  参数：表名 + sheet名 + json文件输出目录 + fbs输出位置
python %fbPath%\python\excel2json.py  %excelName% %sheetName% %jsonPath%/%sheetName%.json %fbsPath%/%sheetName%.fbs

:: 调用flatc将转化成bin文件: -n 生成c# , -b 生成bin文件
%fbPath%\flatc -n -b -o %binPath% %fbsPath%/%sheetName%.fbs %jsonPath%/%sheetName%.json

:: 生成的cs文件拷贝到scripts下
rem xcopy %binPath%/%sheetName%.cs %csPath% /k
rem xcopy %binPath%/%sheetName%_Item.cs %csPath% /k

rem move %binPath%/%sheetName%.cs %scriptsPath%
rem move %binPath%/%sheetName%"_Item".cs %scriptsPath%

pause