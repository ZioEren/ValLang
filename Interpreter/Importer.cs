using System.Collections.Generic;

public static class Importer
{
    public static List<string> imported = new List<string>();

    public static RuntimeResult import(string fn, string ftxt, Context ctx, bool found, Position pos_start, Position pos_end)
    {
        if (fn == "console")
        {
            add(ctx.symbol_table, 1);
        }
        else if (fn == "collections")
        {
            add(ctx.symbol_table, 2);
        }
        else if (fn == "network")
        {
            add(ctx.symbol_table, 3);
        }
        else if (fn == "strings")
        {
            add(ctx.symbol_table, 4);
        }
        else
        {
            if (found)
            {
                Program.import(System.IO.Path.GetFileNameWithoutExtension(fn), ftxt, ctx);
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(pos_start, pos_end, "Could not find the specified file", ctx));
            }
        }

        imported.Add(fn);

        return new RuntimeResult().success(Values.NULL);
    }

    public static void add(SymbolTable table, int toImport)
    {
        if (toImport == 0)
        {
            table.set("null", Values.NULL);
            table.set("true", Values.TRUE);
            table.set("false", Values.FALSE);
            table.set("isNumber", BuiltInFunctions.isNumber);
            table.set("isString", BuiltInFunctions.isString);
            table.set("isList", BuiltInFunctions.isList);
            table.set("isSet", BuiltInFunctions.isSet);
            table.set("isFunction", BuiltInFunctions.isFunction);
            table.set("isStruct", BuiltInFunctions.isStruct);
            table.set("isNamespace", BuiltInFunctions.isNamespace);
            table.set("isLabel", BuiltInFunctions.isLabel);
            table.set("isInteger", BuiltInFunctions.isInteger);
            table.set("isFloat", BuiltInFunctions.isFloat);
            table.set("run", BuiltInFunctions.run);
            //table.set("import", BuiltInFunctions.import);
            table.set("eval", BuiltInFunctions.eval);
            table.set("exit", BuiltInFunctions.exit);
            table.set("end", BuiltInFunctions.end);
            table.set("close", BuiltInFunctions.close);
            table.set("clearRam", BuiltInFunctions.clearRam);
            //table.set("use", BuiltInFunctions.use);
            table.set("Math", BuiltInNamespaces.Math);
            table.set("equals", BuiltInFunctions.equals);
        }
        else if (toImport == 1)
        {
            table.set("print", BuiltInFunctions.print);
            table.set("println", BuiltInFunctions.println);
            table.set("printReturn", BuiltInFunctions.printReturn);
            table.set("inputString", BuiltInFunctions.inputString);
            table.set("inputInteger", BuiltInFunctions.inputInteger);
            table.set("inputFloat", BuiltInFunctions.inputFloat);
            table.set("inputNumber", BuiltInFunctions.inputNumber);
            table.set("clearConsole", BuiltInFunctions.clearConsole);
        }
        else if (toImport == 2)
        {
            table.set("listAppend", BuiltInFunctions.listAppend);
            table.set("listPop", BuiltInFunctions.listPop);
            table.set("listExtend", BuiltInFunctions.listExtend);
            table.set("getListLength", BuiltInFunctions.getListLength);
            table.set("listClear", BuiltInFunctions.listClear);
            table.set("listSort", BuiltInFunctions.listSort);
            table.set("listReverse", BuiltInFunctions.listReverse);
            table.set("listContains", BuiltInFunctions.listContains);
            table.set("listGet", BuiltInFunctions.listGet);
            table.set("setAppend", BuiltInFunctions.setAppend);
            table.set("setPop", BuiltInFunctions.setPop);
            table.set("setExtend", BuiltInFunctions.setExtend);
            table.set("getSetLength", BuiltInFunctions.getSetLength);
            table.set("setClear", BuiltInFunctions.setClear);
            table.set("setSort", BuiltInFunctions.setSort);
            table.set("setReverse", BuiltInFunctions.setReverse);
            table.set("setContains", BuiltInFunctions.setContains);
            table.set("setGet", BuiltInFunctions.setGet);
        }
        else if (toImport == 3)
        {
            table.set("HttpClient", BuiltInStructs.HttpClient);
        }
        else
        {
            table.set("stringStartsWith", BuiltInFunctions.stringStartsWith);
            table.set("stringEndsWith", BuiltInFunctions.stringEndsWith);
            table.set("stringToUpper", BuiltInFunctions.stringToUpper);
            table.set("stringToLower", BuiltInFunctions.stringToLower);
            table.set("stringTrim", BuiltInFunctions.stringTrim);
            table.set("stringLength", BuiltInFunctions.stringLength);
            table.set("stringReplace", BuiltInFunctions.stringReplace);
            table.set("stringContains", BuiltInFunctions.stringContains);
            table.set("stringToUpperInvariant", BuiltInFunctions.stringToUpperInvariant);
            table.set("stringToLowerInvariant", BuiltInFunctions.stringToLowerInvariant);
            table.set("stringPadLeft", BuiltInFunctions.stringPadLeft);
            table.set("stringPadRight", BuiltInFunctions.stringPadRight);
            table.set("stringToList", BuiltInFunctions.stringToList);
            table.set("stringReverse", BuiltInFunctions.stringReverse);
            table.set("stringSplit", BuiltInFunctions.stringSplit);
            table.set("stringSpace", BuiltInFunctions.stringSpace);
            table.set("stringToBase64", BuiltInFunctions.stringToBase64);
            table.set("stringFromBase64", BuiltInFunctions.stringFromBase64);
            table.set("getStringLength", BuiltInFunctions.getStringLength);
            table.set("stringSubstring", BuiltInFunctions.stringSubstring);
            table.set("stringGetChar", BuiltInFunctions.stringGetChar);
        }
    }
}