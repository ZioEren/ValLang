using System;
using System.Collections.Generic;

public class BuiltInNamespace
{
    public Position pos_start, pos_end;
    public Context context;
    public string name;
    public bool already_declared;
    public object theNewNamespace;

    public BuiltInNamespace(string name)
    {
        this.name = name == null || name == "" ? "<anonymous>" : name;
        this.set_pos();
        this.set_context();
        this.declare();
    }

    public void setNewNamespace()
    {
        if (this.name == "Math")
        {
            this.theNewNamespace = new NamespaceMath();
        }
    }

    public BuiltInNamespace set_pos(Position pos_start = null, Position pos_end = null)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;

        return this;
    }

    public BuiltInNamespace declare()
    {
        Context exec_ctx = new Context(this.name, this.context, pos_start);
        exec_ctx.symbol_table = new SymbolTable();

        Importer.add(exec_ctx.symbol_table, 0);

        if (Importer.imported.Contains("console"))
        {
            Importer.add(exec_ctx.symbol_table, 1);
        }

        if (Importer.imported.Contains("collections"))
        {
            Importer.add(exec_ctx.symbol_table, 2);
        }

        if (Importer.imported.Contains("network"))
        {
            Importer.add(exec_ctx.symbol_table, 3);
        }

        if (Importer.imported.Contains("strings"))
        {
            Importer.add(exec_ctx.symbol_table, 4);
        }

        this.context = exec_ctx;
        this.setNewNamespace();
        this.theNewNamespace.GetType().GetMethod("execute_declare").Invoke(this.theNewNamespace, new object[] { this.context });

        return this;
    }

    public BuiltInNamespace copy()
    {
        BuiltInNamespace copy = new BuiltInNamespace(this.name);

        copy.set_pos(this.pos_start, this.pos_end);
        copy.set_context(this.context);

        return copy;
    }


    public BuiltInNamespace set_context(Context context = null)
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
        return "<built-in struct " + this.name + ">";
    }

    public RuntimeResult exec_func(string name, List<object> args)
    {
        RuntimeResult res = new RuntimeResult();
        Context exec_ctx = this.generate_new_context();

        res.register(this.check_and_populate_args((List<string>)InvokeMethod("get_" + name, new List<object>()), args, exec_ctx, name));

        if (res.error != null)
        {
            return res;
        }

        List<object> theArgs = new List<object>();
        theArgs.Add(exec_ctx);

        object return_value = res.register((RuntimeResult)InvokeMethod("execute_" + name, theArgs));

        if (res.error != null)
        {
            return res;
        }

        return res.success(return_value);
    }

    public Context generate_new_context()
    {
        Context new_context = new Context(this.name, this.context, this.pos_start);
        new_context.symbol_table = new SymbolTable(new_context.parent.symbol_table);

        return new_context;
    }

    public RuntimeResult check_args(List<string> arg_names, List<object> args, string funcName)
    {
        RuntimeResult res = new RuntimeResult();

        if (args.Count > arg_names.Count)
        {
            return res.failure(new RuntimeError(this.pos_start, this.pos_end, (args.Count - arg_names.Count).ToString() + " too many args passed into '" + funcName + "'", this.context));
        }

        if (args.Count < arg_names.Count)
        {
            return res.failure(new RuntimeError(this.pos_start, this.pos_end, (arg_names.Count - args.Count).ToString() + " too few args passed into '" + funcName + "'", this.context));
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

    public RuntimeResult check_and_populate_args(List<string> arg_names, List<object> args, Context exec_ctx, string funcName)
    {
        RuntimeResult res = new RuntimeResult();
        res.register(this.check_args(arg_names, args, funcName));

        if (res.should_return())
        {
            return res;
        }

        this.populate_args(arg_names, args, exec_ctx);

        return res.success(null);
    }

    public object InvokeMethod(string methodName, List<object> args)
    {
        return this.theNewNamespace.GetType().GetMethod(methodName).Invoke(this.theNewNamespace, args.ToArray());
    }
}