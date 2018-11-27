# -*- coding: utf-8 -*-

# @func:
#   1-excel表格转换成utf-8格式的json文件
#   2-生成fbs文件
# @date:2018-11-23 21:40:53
# @from:https://www.cnblogs.com/navy-wang/p/3274114.html
# https://blog.csdn.net/zhuangyou123/article/details/10068729
# @edit:Roshan

import os
import sys
import excel2fbs
import codecs
import xlrd

excel_start_row = 2


def str2Utf8(file):
    return unicode(file, "utf-8")


def FloatToString(aFloat):
    if type(aFloat) != float:
        return ""
    strTemp = str(aFloat)
    strList = strTemp.split(".")
    if len(strList) == 1:
        return strTemp
    else:
        if strList[1] == "0":
            return strList[0]
        else:
            return strTemp


def table2jsn(table, jsonfilename):
    nrows = table.nrows
    ncols = table.ncols
    f = codecs.open(jsonfilename, "w", "utf-8")
    # f.write(u"{\n\t\"list\":[\n")
    f.write(u"{\n\t \"list\":[\n")
    for r in range(nrows - 1):
        # 前三行不读取
        if r < excel_start_row:
            if r == 0:
                excel2fbs.getFields(table)
            continue
        f.write(u"\t\t{ ")
        for c in range(ncols):
            strCellValue = u""
            CellObj = table.cell_value(r + 1, c)

            if type(CellObj) == unicode:
                strCellValue = "\"" + CellObj + "\""
            elif type(CellObj) == float:
                strCellValue = FloatToString(CellObj)
                celltype = table.cell_value(1, c)
                if celltype == "string":
                    strCellValue = "\"" + strCellValue + "\""
            else:
                strCellValue = str(CellObj)
            strTmp = u"\"" + table.cell_value(0, c) + u"\":" + strCellValue
            if c < ncols - 1:
                strTmp += u", "
            print("strTmp==" + strTmp)
            f.write(strTmp)
        f.write(u" }")
        if r < nrows - 2:
            f.write(u",")
        f.write(u"\n")
    f.write(u"\t]\n}\n")
    f.close()
    print "Create ", jsonfilename, " OK"
    return


print "--- start trans excel file to json ---"
excelFileName = sys.argv[1]
sheetName = sys.argv[2]
jsonFileName = sys.argv[3]
fbsPath = sys.argv[4]

if (excelFileName is None) or (sheetName is None) or (jsonFileName is None):
    print ("input params error ,please check - excelFileName/sheetName/jsonFileName ")
else:

    excelFileName = str2Utf8(excelFileName)
    sheetName = str2Utf8(sheetName)
    jsonFileName = str2Utf8(jsonFileName)
    fbsPath = str2Utf8(fbsPath)

    excel = xlrd.open_workbook(excelFileName)
    table = excel.sheet_by_name(sheetName)
    table2jsn(table, jsonFileName)

    # 生成fbs文件
    excel2fbs.table2fbs(excelFileName, sheetName,fbsPath)

print "--- all work done ---"
