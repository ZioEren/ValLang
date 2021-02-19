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

    public RuntimeResult execute_printReturn(Context exec_ctx)
    {
        return new RuntimeResult().success(new StringValue((string) exec_ctx.symbol_table.get("value").GetType().GetMethod("as_string").Invoke(exec_ctx.symbol_table.get("value"), new object[] { })));
    }

    public List<string> get_printReturn()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_inputString(Context exec_ctx)
    {
        return new RuntimeResult().success(new StringValue(Console.ReadLine()));
    }

    public List<string> get_inputString()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_inputInteger(Context exec_ctx)
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

    public List<string> get_inputInteger()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_clearConsole(Context exec_ctx)
    {
        Console.Clear();
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_clearConsole()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_isNumber(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("value").GetType() == typeof(NumberValue) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_isNumber()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_isString(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("value").GetType() == typeof(StringValue) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_isString()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_isList(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("value").GetType() == typeof(ListValue) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_isList()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_isFunction(Context exec_ctx)
    {
        return new RuntimeResult().success((exec_ctx.symbol_table.get("value").GetType() == typeof(FunctionValue) || exec_ctx.symbol_table.get("value").GetType() == typeof(BuiltInFunction) || exec_ctx.symbol_table.get("value").GetType() == typeof(BuiltInFunction)) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_isFunction()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_listAppend(Context exec_ctx)
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

    public List<string> get_listAppend()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("list");
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_listPop(Context exec_ctx)
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

    public List<string> get_listPop()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("list");
        arg_names.Add("index");
        return arg_names;
    }

    public RuntimeResult execute_listExtend(Context exec_ctx)
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

    public List<string> get_listExtend()
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

    public RuntimeResult execute_getListLength(Context exec_ctx)
    {
        object list = exec_ctx.symbol_table.get("list");

        if (list.GetType() != typeof(ListValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Argument must be list", exec_ctx));
        }

        return new RuntimeResult().success(new NumberValue(((ListValue)list).elements.Count));
    }

    public List<string> get_getListLength()
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

    public RuntimeResult execute_clearRam(Context exec_ctx)
    {
        Context ctx = exec_ctx;

        while (ctx != null)
        {
            ctx.symbol_table.clear();
            ctx = ctx.parent;
        }

        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_clearRam()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_inputFloat(Context exec_ctx)
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
    public List<string> get_inputFloat()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_inputNumber(Context exec_ctx)
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

    public List<string> get_inputNumber()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_isStruct(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("value").GetType() == typeof(NumberValue) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_isStruct()
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

    public RuntimeResult execute_isNamespace(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("value").GetType() == typeof(NamespaceValue) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_isNamespace()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_use(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("namespace");

        if (value.GetType() == typeof(NamespaceValue))
        {
            NamespaceValue namespaceValue = (NamespaceValue)value;
            Context ctx = exec_ctx;

            foreach (object element in namespaceValue.context.symbol_table.symbols.Keys)
            {
                Tuple<object, bool> other = null;
                namespaceValue.context.symbol_table.symbols.TryGetValue(element, out other);
                ctx.symbol_table.set(element, other.Item1, other.Item2);
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a namespace", exec_ctx));
        }

        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_use()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("namespace");

        return arg_names;
    }

    public RuntimeResult execute_eval(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("code");

        if (value.GetType() == typeof(StringValue))
        {
            StringValue code = (StringValue)value;
            Tuple<object, Error> executed = Program.import(exec_ctx.parent_entry_pos.fn, code.as_string(), exec_ctx);

            if (executed.Item2 != null)
            {
                return new RuntimeResult().failure(executed.Item2);
            }

            return new RuntimeResult().success(executed.Item1);
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(null, null, "Code must be a string", exec_ctx));
        }

        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_eval()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("code");

        return arg_names;
    }

    public RuntimeResult execute_isLabel(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("value").GetType() == typeof(LabelValue) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_isLabel()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_isInteger(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            if (((NumberValue) value).value.GetType() == typeof(int))
            {
                return new RuntimeResult().success(Values.TRUE);
            }
        }

        return new RuntimeResult().success(Values.FALSE);
    }

    public List<string> get_isInteger()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_isFloat(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            if (((NumberValue)value).value.GetType() == typeof(float))
            {
                return new RuntimeResult().success(Values.TRUE);
            }
        }

        return new RuntimeResult().success(Values.FALSE);
    }

    public List<string> get_isFloat()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }
}