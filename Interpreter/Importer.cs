﻿using System.Collections.Generic;

public static class Importer
{
    public static List<string> imported = new List<string>();

    public static RuntimeResult import(string fn, string ftxt, Context ctx, bool found)
    {
        ctx = ctx.parent;

        if (fn == "console")
        {
            add(ctx.symbol_table, 1);
        }
        else if (fn == "lists")
        {
            add(ctx.symbol_table, 2);
        }
        else if (fn == "network")
        {
            add(ctx.symbol_table, 3);
        }
        else
        {
            if (found)
            {
                Program.import(System.IO.Path.GetFileNameWithoutExtension(fn), ftxt, ctx);
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, fn, ftxt), new Position(0, 0, 0, fn, ftxt), "Could not find the specified file", ctx));
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
            table.set("isFunction", BuiltInFunctions.isFunction);
            table.set("isStruct", BuiltInFunctions.isStruct);
            table.set("isNamespace", BuiltInFunctions.isNamespace);
            table.set("isLabel", BuiltInFunctions.isLabel);
            table.set("isInteger", BuiltInFunctions.isInteger);
            table.set("isFloat", BuiltInFunctions.isFloat);

            table.set("run", BuiltInFunctions.run);
            table.set("import", BuiltInFunctions.import);
            table.set("eval", BuiltInFunctions.eval);

            table.set("exit", BuiltInFunctions.exit);
            table.set("end", BuiltInFunctions.end);
            table.set("close", BuiltInFunctions.close);

            table.set("clearRam", BuiltInFunctions.clearRam);

            table.set("use", BuiltInFunctions.use);

            table.set("Math", BuiltInNamespaces.Math);
        }
        else if (toImport == 1)
        {
            table.set("print", BuiltInFunctions.print);
            table.set("println", BuiltInFunctions.println);
            table.set("printReturn", BuiltInFunctions.printReturn);

            table.set("input", BuiltInFunctions.input);
            table.set("inputInteger", BuiltInFunctions.inputInteger);
            table.set("inputFloat", BuiltInFunctions.inputFloat);
            table.set("inputNumber", BuiltInFunctions.inputNumber);

            table.set("clear", BuiltInFunctions.clear);
        }
        else if (toImport == 2)
        {
            table.set("append", BuiltInFunctions.append);
            table.set("pop", BuiltInFunctions.pop);
            table.set("extend", BuiltInFunctions.extend);
            table.set("len", BuiltInFunctions.len);
        }
        else
        {
            table.set("HttpClient", BuiltInStructs.HttpClient);
        }
    }
}