using System;
using System.Collections.Generic;
using System.Diagnostics;

public class BuiltInFunction
{
    public Position pos_start, pos_end;
    public Context context;
    public string name;

    public BuiltInFunction(string name)
    {
        this.name = name == "" || name == null ? "<anonymous>" : name;
        this.set_pos();
        this.set_context();
    }

    public BuiltInFunction set_pos(Position pos_start = null, Position pos_end = null)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;

        return this;
    }

    public RuntimeResult execute(List<object> args)
    {
        RuntimeResult res = new RuntimeResult();
        Context exec_ctx = this.generate_new_context();

        res.register(this.check_and_populate_args((List<string>)InvokeMethod("get_" + this.name, new List<object>()), args, exec_ctx));

        if (res.error != null)
        {
            return res;
        }

        List<object> theArgs = new List<object>();
        theArgs.Add(exec_ctx);

        object return_value = res.register((RuntimeResult)InvokeMethod("execute_" + this.name, theArgs));

        if (res.error != null)
        {
            return res;
        }

        return res.success(return_value);
    }

    public object InvokeMethod(string methodName, List<object> args)
    {
        return GetType().GetMethod(methodName).Invoke(this, args.ToArray());
    }

    public BuiltInFunction copy()
    {
        BuiltInFunction copy = new BuiltInFunction(this.name);
        copy.set_pos(this.pos_start, this.pos_end);
        copy.set_context(this.context);

        return copy;
    }

    public BuiltInFunction set_context(Context context = null)
    {
        this.context = context;

        return this;
    }

    public Tuple<object, Error> get_comparison_ee(object other)
    {
        return new Tuple<object, Error>(Values.FALSE, null);
    }

    public Tuple<object, Error> get_comparison_ne(object other)
    {
        return new Tuple<object, Error>(Values.TRUE, null);
    }

    public bool is_true()
    {
        return true;
    }

    public Error illegal_operation(object other = null)
    {
        if (other == null)
        {
            other = this;
        }

        return new RuntimeError(this.pos_start, (Position)other.GetType().GetField("pos_end").GetValue(other), "Illegal operation", this.context);
    }

    public string as_string()
    {
        return "<built-in function " + this.name + ">";
    }

    public Context generate_new_context()
    {
        Context new_context = new Context(this.name, this.context, this.pos_start);
        new_context.symbol_table = new SymbolTable(new_context.parent.symbol_table);

        return new_context;
    }

    public RuntimeResult check_args(List<string> arg_names, List<object> args)
    {
        RuntimeResult res = new RuntimeResult();

        if (args.Count > arg_names.Count)
        {
            return res.failure(new RuntimeError(this.pos_start, this.pos_end, (args.Count - arg_names.Count).ToString() + " too many args passed into '" + this.name + "'", this.context));
        }

        if (args.Count < arg_names.Count)
        {
            return res.failure(new RuntimeError(this.pos_start, this.pos_end, (arg_names.Count - args.Count).ToString() + " too few args passed into '" + this.name + "'", this.context));
        }

        return res.success(null);
    }

    public void populate_args(List<string> arg_names, List<object> args, Context exec_ctx)
    {
        for (int i = 0; i < args.Count; i++)
        {
            string arg_name = arg_names[i];
            object arg_value = args[i];
            arg_value.GetType().GetMethod("set_context").Invoke(arg_value, new object[] { exec_ctx });
            exec_ctx.symbol_table.set(arg_name, arg_value);
        }
    }

    public RuntimeResult check_and_populate_args(List<string> arg_names, List<object> args, Context exec_ctx)
    {
        RuntimeResult res = new RuntimeResult();
        res.register(this.check_args(arg_names, args));

        if (res.should_return())
        {
            return res;
        }

        this.populate_args(arg_names, args, exec_ctx);

        return res.success(null);
    }

    public RuntimeResult execute_print(Context exec_ctx)
    {
        Console.Write((string) exec_ctx.symbol_table.get("value").GetType().GetMethod("as_string").Invoke(exec_ctx.symbol_table.get("value"), new object[] { }));
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_print()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_print_ret(Context exec_ctx)
    {
        return new RuntimeResult().success(new StringValue((string) exec_ctx.symbol_table.get("value").GetType().GetMethod("as_string").Invoke(exec_ctx.symbol_table.get("value"), new object[] { })));
    }

    public List<string> get_print_ret()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_input(Context exec_ctx)
    {
        return new RuntimeResult().success(new StringValue(Console.ReadLine()));
    }

    public List<string> get_input()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_input_int(Context exec_ctx)
    {
        int number = 0;

        try
        {
            number = int.Parse(Console.ReadLine());
        }
        catch (Exception)
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value cannot be parsed to int.", exec_ctx));
        }

        return new RuntimeResult().success(new NumberValue(number));
    }

    public List<string> get_input_int()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_clear(Context exec_ctx)
    {
        Console.Clear();
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_clear()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_is_number(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("value").GetType() == typeof(NumberValue) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_is_number()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_is_string(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("value").GetType() == typeof(StringValue) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_is_string()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_is_list(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("value").GetType() == typeof(ListValue) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_is_list()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_is_function(Context exec_ctx)
    {
        return new RuntimeResult().success((exec_ctx.symbol_table.get("value").GetType() == typeof(FunctionValue) || exec_ctx.symbol_table.get("value").GetType() == typeof(BuiltInFunction) || exec_ctx.symbol_table.get("value").GetType() == typeof(BuiltInFunction)) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_is_function()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_append(Context exec_ctx)
    {
        object list = exec_ctx.symbol_table.get("list");
        object value = exec_ctx.symbol_table.get("value");

        if (list.GetType() != typeof(ListValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "First argument must be list", exec_ctx));
        }

        ((ListValue)list).elements.Add(value);
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_append()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("list");
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_pop(Context exec_ctx)
    {
        object list = exec_ctx.symbol_table.get("list");
        object index = exec_ctx.symbol_table.get("index");

        if (list.GetType() != typeof(ListValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "First argument must be list", exec_ctx));
        }

        if (index.GetType() != typeof(NumberValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Second argument must be list", exec_ctx));
        }

        object element = null;

        try
        {
            element = ((ListValue)list).elements[(int)((NumberValue)index).value];
            ((ListValue)list).elements.RemoveAt((int)((NumberValue)index).value);
        }
        catch (Exception)
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Element at this index could not be removed from list because index is out of range", exec_ctx));
        }

        return new RuntimeResult().success(element);
    }

    public List<string> get_pop()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("list");
        arg_names.Add("index");
        return arg_names;
    }

    public RuntimeResult execute_extend(Context exec_ctx)
    {
        object listA = exec_ctx.symbol_table.get("listA");
        object listB = exec_ctx.symbol_table.get("listB");

        if (listA.GetType() != typeof(ListValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "First argument must be list", exec_ctx));
        }

        if (listB.GetType() != typeof(ListValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "First argument must be list", exec_ctx));
        }

        ((ListValue)listA).elements.AddRange(((ListValue)listB).elements);
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_extend()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("listA");
        arg_names.Add("listB");
        return arg_names;
    }

    public RuntimeResult execute_run(Context exec_ctx)
    {
        object fileName = exec_ctx.symbol_table.get("fn");

        if (fileName.GetType() != typeof(StringValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Argument must be string", exec_ctx));
        }

        string fn = ((StringValue)fileName).value;
        string script = "";

        try
        {
            if (System.IO.File.Exists(fn))
            {
                script = System.IO.File.ReadAllText(fn);
            }
            else if (System.IO.File.Exists(fn + ".v"))
            {
                script = System.IO.File.ReadAllText(fn + ".v");
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Could not find the specified file", exec_ctx));
            }
        }
        catch (Exception)
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Failed to open the specified file", exec_ctx));
        }

        Program.completeRun(System.IO.Path.GetFileNameWithoutExtension(fn), script);
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_run()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("fn");
        return arg_names;
    }

    public RuntimeResult execute_len(Context exec_ctx)
    {
        object list = exec_ctx.symbol_table.get("list");

        if (list.GetType() != typeof(ListValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Argument must be list", exec_ctx));
        }

        return new RuntimeResult().success(new NumberValue(((ListValue)list).elements.Count));
    }

    public List<string> get_len()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("list");
        return arg_names;
    }

    public RuntimeResult execute_import(Context exec_ctx)
    {
        object fileName = exec_ctx.symbol_table.get("fn");

        if (fileName.GetType() != typeof(StringValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Argument must be string", exec_ctx));
        }

        string fn = ((StringValue)fileName).value;
        string script = "";
        bool found = true;

        try
        {
            if (System.IO.File.Exists(fn))
            {
                script = System.IO.File.ReadAllText(fn);
            }
            else if (System.IO.File.Exists(fn + ".v"))
            {
                script = System.IO.File.ReadAllText(fn + ".v");
            }
            else
            {
                found = false;
            }
        }
        catch (Exception)
        {
            found = false;
        }

        return Importer.import(fn, script, exec_ctx, found);
    }

    public List<string> get_import()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("fn");
        return arg_names;
    }

    public RuntimeResult execute_exit(Context exec_ctx)
    {
        Process.GetCurrentProcess().Kill();
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_exit()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_close(Context exec_ctx)
    {
        Process.GetCurrentProcess().Kill();
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_close()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_end(Context exec_ctx)
    {
        Process.GetCurrentProcess().Kill();
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_end()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_clear_ram(Context exec_ctx)
    {
        Context ctx = exec_ctx;

        while (ctx != null)
        {
            ctx.symbol_table.clear();
            ctx = ctx.parent;
        }

        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_clear_ram()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_input_float(Context exec_ctx)
    {
        float number = 0.0F;

        while (true)
        {
            try
            {
                number = float.Parse(Console.ReadLine().Replace(".", ","));
                break;
            }
            catch (Exception)
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value cannot be parsed to float.", exec_ctx));
            }
        }

        return new RuntimeResult().success(new NumberValue(number));
    }
    public List<string> get_input_float()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_input_num(Context exec_ctx)
    {
        try
        {
            string text = Console.ReadLine();

            if (text.Contains(".") || text.Contains(","))
            {
                text = text.Replace(".", ",");

                return new RuntimeResult().success(new NumberValue(float.Parse(text)));
            }
            else
            {
                return new RuntimeResult().success(new NumberValue(int.Parse(text)));
            }
        }
        catch (Exception)
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value cannot be parsed to float.", exec_ctx));
        }
    }

    public List<string> get_input_num()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_is_struct(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("value").GetType() == typeof(NumberValue) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_is_struct()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_println(Context exec_ctx)
    {
        Console.WriteLine((string)exec_ctx.symbol_table.get("value").GetType().GetMethod("as_string").Invoke(exec_ctx.symbol_table.get("value"), new object[] { }));
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_println()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }
}