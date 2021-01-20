using System.Collections.Generic;

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
            table.set("is_num", BuiltInFunctions.is_number);
            table.set("is_str", BuiltInFunctions.is_string);
            table.set("is_list", BuiltInFunctions.is_list);
            table.set("is_fun", BuiltInFunctions.is_function);
            table.set("is_struct", BuiltInFunctions.is_struct);
            table.set("run", BuiltInFunctions.run);
            table.set("import", BuiltInFunctions.import);
            table.set("exit", BuiltInFunctions.exit);
            table.set("end", BuiltInFunctions.end);
            table.set("close", BuiltInFunctions.close);
            table.set("clear_ram", BuiltInFunctions.clear_ram);
            table.set("Math", BuiltInStructs.Math);
            table.set("use", BuiltInFunctions.use);
            table.set("is_namespace", BuiltInFunctions.is_namespace);
        }
        else if (toImport == 1)
        {
            table.set("print", BuiltInFunctions.print);
            table.set("println", BuiltInFunctions.println);
            table.set("print_ret", BuiltInFunctions.print_ret);
            table.set("input", BuiltInFunctions.input);
            table.set("input_int", BuiltInFunctions.input_int);
            table.set("clear", BuiltInFunctions.clear);
            table.set("input_float", BuiltInFunctions.input_float);
            table.set("input_num", BuiltInFunctions.input_num);
        }
        else
        {
            table.set("append", BuiltInFunctions.append);
            table.set("pop", BuiltInFunctions.pop);
            table.set("extend", BuiltInFunctions.extend);
            table.set("len", BuiltInFunctions.len);
        }
    }
}