using System;
using System.Collections.Generic;

public class FunctionValue
{
    public Position pos_start, pos_end;
    public Context context;
    public string name;
    public object body_node;
    public List<Tuple<string, object>> arg_names;
    public bool should_auto_return;

    public FunctionValue(string name, object body_node, List<Tuple<string, object>> arg_names, bool should_auto_return)
    {
        this.name = name == null || name == "" ? "<anonymous>" : name;
        this.body_node = body_node;
        this.arg_names = arg_names;
        this.should_auto_return = should_auto_return;
        this.set_pos();
        this.set_context();
    }

    public FunctionValue set_pos(Position pos_start = null, Position pos_end = null)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;

        return this;
    }

    public RuntimeResult execute(List<object> args)
    {
        RuntimeResult res = new RuntimeResult();
        Interpreter interpreter = new Interpreter();
        Context exec_ctx = this.generate_new_context();
        int necessary = 0, necessary1 = 0;

        foreach (Tuple<string, object> arg in arg_names)
        {
            if (arg.Item2 != null)
            {
                necessary++;
            }
            else
            {
                break;
            }
        }

        foreach (Tuple<string, object> arg in arg_names)
        {
            if (arg.Item2 == null)
            {
                necessary1++;
            }
            else
            {
                break;
            }
        }

        int initialCount = args.Count, newCount = arg_names.Count;

        if (newCount != 0)
        {
            if (necessary == args.Count)
            {
                for (int i = 0; i <= (necessary1 - args.Count); i++)
                {
                    args.Add(null);
                }
            }
            else if (necessary1 == args.Count && necessary1 != newCount)
            {
                for (int i = 0; i <= (necessary1 - args.Count); i++)
                {
                    args.Add(null);
                }
            }
            else
            {
                for (int i = 0; i <= (necessary - args.Count); i++)
                {
                    args.Add(null);
                }
            }

            if (necessary1 == initialCount)
            {
                for (int i = 0; i < necessary - 1; i++)
                {
                    args.Add(null);
                }
            }
        }

        res.register(this.check_and_populate_args(this.arg_names, args, exec_ctx));

        if (res.should_return())
        {
            return res;
        }

        object value = res.register(interpreter.visit(this.body_node, exec_ctx));

        if (res.should_return() && res.func_return_value == null)
        {
            return res;
        }

        object ret_value = this.should_auto_return ? value : (res.func_return_value != null ? res.func_return_value : Values.NULL);

        return res.success(ret_value);
    }

    public FunctionValue copy()
    {
        FunctionValue copy = new FunctionValue(this.name, this.body_node, this.arg_names, this.should_auto_return);

        copy.set_pos(this.pos_start, this.pos_end);
        copy.set_context(this.context);

        return copy;
    }

    public FunctionValue set_context(Context context = null)
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
        return "<function " + this.name + ">";
    }

    public Context generate_new_context()
    {
        Context new_context = new Context(this.name, this.context, this.pos_start);
        new_context.symbol_table = new SymbolTable(new_context.parent.symbol_table);

        return new_context;
    }

    public RuntimeResult check_args(List<Tuple<string, object>> arg_names, List<object> args)
    {
        RuntimeResult res = new RuntimeResult();
        int necessary = 0;

        foreach (Tuple<string, object> arg in arg_names)
        {
            if (arg.Item2 == null)
            {
                necessary++;
            }
            else
            {
                break;
            }
        }

        if (args.Count > arg_names.Count)
        {
            return res.failure(new RuntimeError(this.pos_start, this.pos_end, (args.Count - arg_names.Count).ToString() + " too many args passed into '" + this.name + "'", this.context));
        }

        if (args.Count < necessary)
        {
            return res.failure(new RuntimeError(this.pos_start, this.pos_end, (arg_names.Count - args.Count).ToString() + " too few args passed into '" + this.name + "'", this.context));
        }

        return res.success(null);
    }

    public void populate_args(List<Tuple<string, object>> arg_names, List<object> args, Context exec_ctx)
    {
        for (int i = 0; i < args.Count; i++)
        {
            string arg_name = arg_names[i].Item1;
            object arg_value = args[i];

            if (arg_value == null)
            {
                arg_value = arg_names[i].Item2;
            }

            arg_value.GetType().GetMethod("set_context").Invoke(arg_value, new object[] { exec_ctx });
            exec_ctx.symbol_table.set(arg_name, arg_value);
        }
    }

    public RuntimeResult check_and_populate_args(List<Tuple<string, object>> arg_names, List<object> args, Context exec_ctx)
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
}