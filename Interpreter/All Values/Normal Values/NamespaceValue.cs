using System;
using System.Collections.Generic;

public class NamespaceValue
{
    public Position pos_start, pos_end;
    public Context context;
    public string name;

    public NamespaceValue(string name, object statements, Interpreter interpreter, Position pos_start, Position pos_end, Context ctx)
    {
        this.name = name == null || name == "" ? "<anonymous>" : name;
        this.set_pos(pos_start, pos_end);
        this.set_context(ctx);
        this.declare(interpreter, statements);
    }

    public NamespaceValue set_pos(Position pos_start = null, Position pos_end = null)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;

        return this;
    }

    public NamespaceValue declare(Interpreter interpreter, object statements)
    {
        Context exec_ctx = new Context(this.name, this.context, pos_start);
        exec_ctx.symbol_table = new SymbolTable();

        Importer.add(exec_ctx.symbol_table, 0);

        if (Importer.imported.Contains("console"))
        {
            Importer.add(exec_ctx.symbol_table, 1);
        }

        if (Importer.imported.Contains("lists"))
        {
            Importer.add(exec_ctx.symbol_table, 2);
        }

        this.context = exec_ctx;
        interpreter.visit(statements, exec_ctx);

        return this;
    }

    public NamespaceValue set_context(Context context = null)
    {
        if (this.context == null)
        {
            this.context = context;
        }

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
        return "<namespace " + this.name + ">";
    }
}