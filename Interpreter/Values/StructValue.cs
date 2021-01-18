using System;
using System.Collections.Generic;

public class StructValue
{
    public Position pos_start, pos_end;
    public Context context;
    public string name;
    public object statements;

    public StructValue(string name, object statements)
    {
        this.name = name == null || name == "" ? "<anonymous>" : name;
        this.statements = statements;
        this.set_pos();
        this.set_context();
    }

    public StructValue set_pos(Position pos_start = null, Position pos_end = null)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;

        return this;
    }

    public RuntimeResult execute(List<object> args)
    {
        return new RuntimeResult().failure(this.illegal_operation(args));
    }

    public StructValue declare(Interpreter interpreter)
    {
        Context exec_ctx = new Context(this.name, this.context, pos_start);
        exec_ctx.symbol_table = new SymbolTable();
        exec_ctx.symbol_table.set("print", BuiltInFunctions.print);
        exec_ctx.symbol_table.set("print_ret", BuiltInFunctions.print_ret);
        exec_ctx.symbol_table.set("input", BuiltInFunctions.input);
        exec_ctx.symbol_table.set("input_int", BuiltInFunctions.input_int);
        exec_ctx.symbol_table.set("clear", BuiltInFunctions.clear);
        exec_ctx.symbol_table.set("append", BuiltInFunctions.append);
        exec_ctx.symbol_table.set("pop", BuiltInFunctions.pop);
        exec_ctx.symbol_table.set("extend", BuiltInFunctions.extend);
        exec_ctx.symbol_table.set("len", BuiltInFunctions.len);
        this.context = exec_ctx;
        interpreter.visit(this.statements, exec_ctx);

        return this;
    }

    public StructValue copy()
    {
        StructValue copy = new StructValue(this.name, this.statements);

        copy.set_pos(this.pos_start, this.pos_end);
        copy.set_context(this.context);

        return copy;
    }


    public StructValue set_context(Context context = null)
    {
        this.context = context;

        return this;
    }

    public Tuple<object, Error> added_to(object other)
    {
        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> subbed_by(object other)
    {
        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> multed_by(object other)
    {
        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> dived_by(object other)
    {
        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> powed_by(object other)
    {
        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> get_comparison_eq(object other)
    {
        return new Tuple<object, Error>(Values.FALSE, null); ;
    }

    public Tuple<object, Error> get_comparison_ne(object other)
    {
        return new Tuple<object, Error>(Values.TRUE, null);
    }

    public Tuple<object, Error> get_comparison_lt(object other)
    {
        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> get_comparison_gt(object other)
    {
        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> get_comparison_lte(object other)
    {
        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> get_comparison_gte(object other)
    {
        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> anded_by(object other)
    {
        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> ored_by(object other)
    {
        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> notted()
    {
        return new Tuple<object, Error>(null, this.illegal_operation(null));
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
        return "<struct " + this.name + ">";
    }
}