public static class Importer
{
    public static RuntimeResult import(string fn, string ftxt, Context ctx, bool found)
    {
        ctx = ctx.parent;

        if (fn == "console")
        {
            ctx.symbol_table.set("print", BuiltInFunctions.print);
            ctx.symbol_table.set("print_ret", BuiltInFunctions.print_ret);
            ctx.symbol_table.set("input", BuiltInFunctions.input);
            ctx.symbol_table.set("input_int", BuiltInFunctions.input_int);
            ctx.symbol_table.set("clear", BuiltInFunctions.clear);
        }
        else if (fn == "lists")
        {
            ctx.symbol_table.set("append", BuiltInFunctions.append);
            ctx.symbol_table.set("pop", BuiltInFunctions.pop);
            ctx.symbol_table.set("extend", BuiltInFunctions.extend);
            ctx.symbol_table.set("len", BuiltInFunctions.len);
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

        return new RuntimeResult().success(Values.NULL);
    }
}