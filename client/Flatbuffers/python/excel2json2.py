import os
import sys
import codecs
import json
from xlrd import open_workbook


# "<type 'unicode'>"
# "<type 'float'>"
def my_text_(text):
    """Return the translated text from json text."""
    v = ("<type '", "'>")
    if text[:len(v[0])] != v[0]: return text
    if text[-len(v[1]):] != v[1]: return text
    return text[len(v[0]): -len(v[1])]


def str2Utf8(file):
    return unicode(file, "utf-8")


def sheet2json(sheet, jsonfile):
    row = 0
    attribute_row = []
    for col in range(sheet.ncols):
        attribute_row.append(sheet.cell(row, col).value)

    attribute = {}
    row = 1
    for col in range(sheet.ncols):
        attribute[attribute_row[col]] = my_text_(repr(type(sheet.cell_value(row, col))))

    entities = []
    for row in range(2, sheet.nrows):

        entity = {}
        for col in range(sheet.ncols):
            entity[attribute_row[col]] = sheet.cell(row, col).value

        row_dict = {}
        row_dict["entity"] = entity

        entities.append(row_dict)

    sheet_dict = {}
    sheet_dict["entities"] = entities

    filed = codecs.open("./jsonsFromExcel/" + jsonfile, "w", "utf-8")
    filed.write("%s" % json.dumps(sheet_dict, ensure_ascii=False, indent=4))
    # print json.dumps(sheet_dict, ensure_ascii=False, indent=4)
    filed.close()


if len(sys.argv) != 2:
    print "usage: %s [input xls file]" % sys.argv[0]
    print "e.g. : %s myinput.xls" % sys.argv[0]
    print "Note : the input file should be MS excel doc.\n"

    sys.exit()

xls_file = sys.argv[1]

workbook = open_workbook(str2Utf8(xls_file))

for sheet in workbook.sheets():
    jsonfile =  sheet.name + ".json"
    # print "Converting %s's %s from EXCEL's sheet to JSON as %s" % (xls_file, sheet.name, jsonfile)

    sheet2json(sheet, jsonfile)
