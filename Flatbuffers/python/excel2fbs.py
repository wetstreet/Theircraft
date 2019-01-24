# -*- coding: utf-8 -*-

# @auth: Roshan
# @date: 2018年11月26日15:07:34
# @func: excel表格转化成fbs文件,一个excel对应生成一个fbs文件
# @doc : https://google.github.io/flatbuffers/flatbuffers_guide_writing_schema.html

# Excel格式:
#     第一行:类型     ID	 Name	Desc	Atlas	Icon	DecomFlag	UseCondition
#     第二行:字段名   int    string    double       float
#     第三行:备注

# step1 : Read table

# step2 : Create fbs file

# step3 : Create json file

# step4 : Create .bin file by bash

import codecs
import xlrd

idl_list_name = "list"
idl_root_table_name = "Item"
idl_file_path = r"./fbs/"

# idl table;key-tableName value-idlCell
idl_table = []


class IdlCell(object):
    """生成schema文件的table"""

    def __init__(self, fieldName, fieldType):
        """
        fieldName:字段名
        fieldType:字段类型
        """
        self.fieldName = fieldName
        self.fieldType = fieldType


# 获取excel的字段
def getFields(table):
    for c in range(table.ncols):
        fieldName = table.cell_value(0, c)
        fieldType = table.cell_value(1, c)
        if fieldName is None or fieldType is None:
            print("字段名和字段类型不能为空！")
            return
        idlObj = IdlCell(fieldName, fieldType)
        idl_table.append(idlObj)


def table2fbs(excelName, sheetName, fbsPath):
    if idl_table is not None and len(idl_table) <= 0:
        print("idl_table is empty!")
        return

    f = codecs.open(fbsPath, "w", "utf-8")
    # f.write(r"namespace " + excelName + ";")
    clsName = sheetName + "_Item"
    f.write("table " + sheetName + " { " + idl_list_name + ":[" + clsName + "]; }\n");
    f.write("table " + clsName + " { \n")
    for i in range(len(idl_table)):
        item = idl_table[i]
        f.write("\t\t" + item.fieldName + ":" + item.fieldType + ";\n")
    f.write("}\n")

    f.write("root_type " + sheetName + ";")
    f.close()
    print("Write fbs finish~")
