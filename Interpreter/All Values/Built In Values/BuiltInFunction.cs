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

    public RuntimeResult execute_listGet(Context exec_ctx)
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

        object element = Values.NULL;

        try
        {
            element = ((ListValue)list).elements[(int)((NumberValue)index).value];
        }
        catch (Exception)
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Element at this index could not be removed from list because index is out of range", exec_ctx));
        }

        return new RuntimeResult().success(element);
    }

    public List<string> get_listGet()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("list");
        arg_names.Add("index");
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

    public RuntimeResult execute_listClear(Context exec_ctx)
    {
        object list = exec_ctx.symbol_table.get("list");

        if (list.GetType() == typeof(ListValue))
        {
            ((ListValue)list).elements.Clear();
            return new RuntimeResult().success(Values.TRUE);
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a list", exec_ctx));
    }

    public List<string> get_listClear()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("list");
        return arg_names;
    }

    public RuntimeResult execute_listSort(Context exec_ctx)
    {
        object list = exec_ctx.symbol_table.get("list");

        if (list.GetType() == typeof(ListValue))
        {
            ((ListValue)list).elements.Sort();
            return new RuntimeResult().success(Values.TRUE);
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a list", exec_ctx));
    }

    public List<string> get_listSort()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("list");
        return arg_names;
    }

    public RuntimeResult execute_listReverse(Context exec_ctx)
    {
        object list = exec_ctx.symbol_table.get("list");

        if (list.GetType() == typeof(ListValue))
        {
            ((ListValue)list).elements.Reverse();
            return new RuntimeResult().success(Values.TRUE);
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a list", exec_ctx));
    }

    public List<string> get_listReverse()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("list");
        return arg_names;
    }

    public RuntimeResult execute_listContains(Context exec_ctx)
    {
        object list = exec_ctx.symbol_table.get("list");
        object value = exec_ctx.symbol_table.get("value");

        if (list.GetType() == typeof(ListValue))
        {
            foreach (object element in ((ListValue) list).elements)
            {
                if ((string)element.GetType().GetMethod("as_string").Invoke(element, new object[] { }) == (string) (value.GetType().GetMethod("as_string").Invoke(value, new object[] { })) && element.GetType() == value.GetType())
                {
                    return new RuntimeResult().success(Values.TRUE);
                }
            }

            return new RuntimeResult().success(Values.FALSE);
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a list", exec_ctx));
    }

    public List<string> get_listContains()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("list");
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_equals(Context exec_ctx)
    {
        object firstValue = exec_ctx.symbol_table.get("firstValue");
        object secondValue = exec_ctx.symbol_table.get("secondValue");

        if ((string)firstValue.GetType().GetMethod("as_string").Invoke(firstValue, new object[] { }) == (string)(secondValue.GetType().GetMethod("as_string").Invoke(secondValue, new object[] { })) && firstValue.GetType() == secondValue.GetType())
        {
            return new RuntimeResult().success(Values.TRUE);
        }

        return new RuntimeResult().success(Values.FALSE);
    }

    public List<string> get_equals()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("firstValue");
        arg_names.Add("secondValue");
        return arg_names;
    }

    public RuntimeResult execute_stringStartsWith(Context exec_ctx)
    {
        object firstValue = exec_ctx.symbol_table.get("firstValue");
        object secondValue = exec_ctx.symbol_table.get("secondValue");

        if (firstValue.GetType() == typeof(StringValue))
        {
            if (secondValue.GetType() == typeof(StringValue))
            {
                if (((StringValue)firstValue).value.StartsWith(((StringValue)secondValue).value))
                {
                    return new RuntimeResult().success(Values.TRUE);
                }
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(null, null, "Second value must be a string", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(null, null, "First value must be a string", exec_ctx));
        }

        return new RuntimeResult().success(Values.FALSE);
    }

    public List<string> get_stringStartsWith()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("firstValue");
        arg_names.Add("secondValue");
        return arg_names;
    }

    public RuntimeResult execute_stringEndsWith(Context exec_ctx)
    {
        object firstValue = exec_ctx.symbol_table.get("firstValue");
        object secondValue = exec_ctx.symbol_table.get("secondValue");

        if (firstValue.GetType() == typeof(StringValue))
        {
            if (secondValue.GetType() == typeof(StringValue))
            {
                if (((StringValue)firstValue).value.EndsWith(((StringValue)secondValue).value))
                {
                    return new RuntimeResult().success(Values.TRUE);
                }
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(null, null, "Second value must be a string", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(null, null, "First value must be a string", exec_ctx));
        }

        return new RuntimeResult().success(Values.FALSE);
    }

    public List<string> get_stringEndsWith()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("firstValue");
        arg_names.Add("secondValue");
        return arg_names;
    }

    public RuntimeResult execute_stringContains(Context exec_ctx)
    {
        object firstValue = exec_ctx.symbol_table.get("firstValue");
        object secondValue = exec_ctx.symbol_table.get("secondValue");

        if (firstValue.GetType() == typeof(StringValue))
        {
            if (secondValue.GetType() == typeof(StringValue))
            {
                if (((StringValue) firstValue).value.Contains(((StringValue)secondValue).value))
                {
                    return new RuntimeResult().success(Values.TRUE);
                }
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(null, null, "Second value must be a string", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(null, null, "First value must be a string", exec_ctx));
        }

        return new RuntimeResult().success(Values.FALSE);
    }

    public List<string> get_stringContains()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("firstValue");
        arg_names.Add("secondValue");
        return arg_names;
    }

    public RuntimeResult execute_stringToUpper(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(StringValue))
        {
            return new RuntimeResult().success(new StringValue(((StringValue)value).value.ToUpper()));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringToUpper()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_stringToLower(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(StringValue))
        {
            return new RuntimeResult().success(new StringValue(((StringValue)value).value.ToLower()));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringToLower()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_stringTrim(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(StringValue))
        {
            return new RuntimeResult().success(new StringValue(((StringValue)value).value.Trim()));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringTrim()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_stringToUpperInvariant(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(StringValue))
        {
            return new RuntimeResult().success(new StringValue(((StringValue)value).value.ToUpperInvariant()));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringToUpperInvariant()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_stringToLowerInvariant(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(StringValue))
        {
            return new RuntimeResult().success(new StringValue(((StringValue)value).value.ToLowerInvariant()));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringToLowerInvariant()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    public RuntimeResult execute_stringReverse(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(StringValue))
        {
            return new RuntimeResult().success(new StringValue(Reverse(((StringValue)value).value)));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringReverse()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_stringReplace(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");
        object toReplace = exec_ctx.symbol_table.get("toReplace");
        object replaceWith = exec_ctx.symbol_table.get("replaceWith");

        if (value.GetType() == typeof(StringValue))
        {
            if (toReplace.GetType() == typeof(StringValue))
            {
                if (replaceWith.GetType() == typeof(StringValue))
                {
                    return new RuntimeResult().success(new StringValue(((StringValue)value).value.Replace(((StringValue)toReplace).value, ((StringValue)replaceWith).value)));
                }
                else
                {
                    return new RuntimeResult().failure(new RuntimeError(null, null, "Replace with string must be a string", exec_ctx));
                }
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(null, null, "String to replace must be a string", exec_ctx));
            }
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringReplace()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        arg_names.Add("toReplace");
        arg_names.Add("replaceWith");
        return arg_names;
    }

    public RuntimeResult execute_stringToList(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(StringValue))
        {
            List<object> elements = new List<object>();

            foreach (char c in ((StringValue) value).value.ToCharArray())
            {
                elements.Add(new StringValue(c.ToString()));
            }

            return new RuntimeResult().success(new ListValue(elements));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringToList()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_getStringLength(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(StringValue))
        {
            return new RuntimeResult().success(new NumberValue(((StringValue)value).value.Length));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_getStringLength()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_stringSubstring(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");
        object startIndex = exec_ctx.symbol_table.get("startIndex");
        object count = exec_ctx.symbol_table.get("count");

        if (value.GetType() == typeof(StringValue))
        {
            if (startIndex.GetType() == typeof(NumberValue))
            {
                if (count.GetType() == typeof(NumberValue))
                {
                    if ((int) ((NumberValue) startIndex).value < 0)
                    {
                        return new RuntimeResult().failure(new RuntimeError(null, null, "Start index must be greater or equal than zero", exec_ctx));
                    }

                    if ((int)((NumberValue)count).value < 0)
                    {
                        return new RuntimeResult().failure(new RuntimeError(null, null, "Count must be greater or equal than zero", exec_ctx));
                    }

                    if ((int)((NumberValue)count).value > ((StringValue)value).value.Length)
                    {
                        return new RuntimeResult().failure(new RuntimeError(null, null, "Count must not be greater than the string length", exec_ctx));
                    }

                    try
                    {
                        return new RuntimeResult().success(new StringValue(((StringValue)value).value.Substring((int)((NumberValue)startIndex).value, (int)((NumberValue)count).value)));
                    }
                    catch
                    {
                        return new RuntimeResult().failure(new RuntimeError(null, null, "Count must be a number between the start index and the length of the string", exec_ctx));
                    }
                }
                else
                {
                    return new RuntimeResult().failure(new RuntimeError(null, null, "Count must be a number", exec_ctx));
                }
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(null, null, "Start index must be a number", exec_ctx));
            }
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringSubstring()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        arg_names.Add("startIndex");
        arg_names.Add("count");
        return arg_names;
    }

    public RuntimeResult execute_stringGetChar(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");
        object index = exec_ctx.symbol_table.get("index");

        if (value.GetType() == typeof(StringValue))
        {
            if (index.GetType() == typeof(NumberValue))
            {
                if ((int)((NumberValue)index).value < 0)
                {
                    return new RuntimeResult().failure(new RuntimeError(null, null, "Index must be greater or equal than zero", exec_ctx));
                }

                if ((int)((NumberValue)index).value >= ((StringValue)value).value.Length)
                {
                    return new RuntimeResult().failure(new RuntimeError(null, null, "Index must not be greater than the string length", exec_ctx));
                }

                return new RuntimeResult().success(new StringValue(((StringValue)value).value[(int)((NumberValue)index).value].ToString()));
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(null, null, "Index must be a number", exec_ctx));
            }
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringGetChar()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        arg_names.Add("index");
        return arg_names;
    }

    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public RuntimeResult execute_stringToBase64(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(StringValue))
        {
            return new RuntimeResult().success(new StringValue(Base64Encode(((StringValue)value).value)));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringToBase64()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_stringFromBase64(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(StringValue))
        {
            return new RuntimeResult().success(new StringValue(Base64Decode(((StringValue)value).value)));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringFromBase64()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_stringSplit(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");
        object valueToSplit = exec_ctx.symbol_table.get("valueToSplit");

        if (value.GetType() == typeof(StringValue))
        {
            if (valueToSplit.GetType() == typeof(StringValue))
            {
                List<object> elements = new List<object>();

                foreach (string element in ((StringValue)value).value.Split(new[] { ((StringValue)valueToSplit).value }, StringSplitOptions.None))
                {
                    elements.Add(new StringValue(element));
                }

                return new RuntimeResult().success(new ListValue(elements));
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(null, null, "Value to split must be a string", exec_ctx));
            }    
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringSplit()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        arg_names.Add("valueToSplit");
        return arg_names;
    }

    public RuntimeResult execute_stringPadLeft(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");
        object width = exec_ctx.symbol_table.get("width");

        if (value.GetType() == typeof(StringValue))
        {
            return new RuntimeResult().success(new StringValue(Base64Decode(((StringValue)value).value.PadLeft((int)((NumberValue) width).value))));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringPadLeft()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        arg_names.Add("width");
        return arg_names;
    }

    public RuntimeResult execute_stringPadRight(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");
        object width = exec_ctx.symbol_table.get("width");

        if (value.GetType() == typeof(StringValue))
        {
            return new RuntimeResult().success(new StringValue(Base64Decode(((StringValue)value).value.PadRight((int)((NumberValue)width).value))));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a string", exec_ctx));
    }

    public List<string> get_stringPadRight()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        arg_names.Add("width");
        return arg_names;
    }

    public RuntimeResult execute_stringSpace(Context exec_ctx)
    {
        object numberOfSpaces = exec_ctx.symbol_table.get("numberOfSpaces");

        if (numberOfSpaces.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new StringValue("".PadLeft((int)((NumberValue)numberOfSpaces).value)));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Number of spaces must be a number", exec_ctx));
    }

    public List<string> get_stringSpace()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("numberOfSpaces");
        return arg_names;
    }

    public RuntimeResult execute_isSet(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("value").GetType() == typeof(SetValue) ? Values.TRUE : Values.FALSE);
    }

    public List<string> get_isSet()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_setAppend(Context exec_ctx)
    {
        object list = exec_ctx.symbol_table.get("set");
        object value = exec_ctx.symbol_table.get("value");

        if (list.GetType() != typeof(SetValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "First argument must be a set", exec_ctx));
        }

    ((SetValue)list).elements.Add(value);
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_setAppend()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("set");
        arg_names.Add("value");
        return arg_names;
    }

    public RuntimeResult execute_setGet(Context exec_ctx)
    {
        object set = exec_ctx.symbol_table.get("set");
        object index = exec_ctx.symbol_table.get("index");

        if (set.GetType() != typeof(SetValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "First argument must be a set", exec_ctx));
        }

        if (index.GetType() != typeof(NumberValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Second argument must be a set", exec_ctx));
        }

        object element = Values.NULL;

        try
        {
            element = ((SetValue)set).elements[(int)((NumberValue)index).value];
        }
        catch (Exception)
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Element at this index could not be removed from the set because index is out of range", exec_ctx));
        }

        return new RuntimeResult().success(element);
    }

    public List<string> get_setGet()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("set");
        arg_names.Add("index");
        return arg_names;
    }

    public RuntimeResult execute_setPop(Context exec_ctx)
    {
        object set = exec_ctx.symbol_table.get("set");
        object index = exec_ctx.symbol_table.get("index");

        if (set.GetType() != typeof(SetValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "First argument must be a set", exec_ctx));
        }

        if (index.GetType() != typeof(NumberValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Second argument must be a set", exec_ctx));
        }

        object element = null;

        try
        {
            element = ((SetValue)set).elements[(int)((NumberValue)index).value];
            ((SetValue)set).elements.RemoveAt((int)((NumberValue)index).value);
        }
        catch (Exception)
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Element at this index could not be removed from a set because index is out of range", exec_ctx));
        }

        return new RuntimeResult().success(element);
    }

    public List<string> get_setPop()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("set");
        arg_names.Add("index");
        return arg_names;
    }

    public RuntimeResult execute_setExtend(Context exec_ctx)
    {
        object setA = exec_ctx.symbol_table.get("setA");
        object setB = exec_ctx.symbol_table.get("setB");

        if (setA.GetType() != typeof(SetValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "First argument must be a set", exec_ctx));
        }

        if (setB.GetType() != typeof(SetValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "First argument must be a set", exec_ctx));
        }

        ((SetValue)setA).elements.AddRange(((SetValue)setB).elements);
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_setExtend()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("setA");
        arg_names.Add("setB");
        return arg_names;
    }

    public RuntimeResult execute_getSetLength(Context exec_ctx)
    {
        object list = exec_ctx.symbol_table.get("set");

        if (list.GetType() != typeof(SetValue))
        {
            return new RuntimeResult().failure(new RuntimeError(this.pos_start, this.pos_end, "Argument must be a set", exec_ctx));
        }

        return new RuntimeResult().success(new NumberValue(((SetValue)list).elements.Count));
    }

    public List<string> get_getSetLength()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("set");
        return arg_names;
    }

    public RuntimeResult execute_setContains(Context exec_ctx)
    {
        object set = exec_ctx.symbol_table.get("set");
        object value = exec_ctx.symbol_table.get("value");

        if (set.GetType() == typeof(SetValue))
        {
            foreach (object element in ((SetValue)set).elements)
            {
                if ((string)element.GetType().GetMethod("as_string").Invoke(element, new object[] { }) == (string)(value.GetType().GetMethod("as_string").Invoke(value, new object[] { })) && element.GetType() == value.GetType())
                {
                    return new RuntimeResult().success(Values.TRUE);
                }
            }

            return new RuntimeResult().success(Values.FALSE);
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Value must be a set", exec_ctx));
    }

    public List<string> get_setContains()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("set");
        arg_names.Add("value");
        return arg_names;
    }
}