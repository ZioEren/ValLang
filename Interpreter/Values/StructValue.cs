using System;
using System.Collections.Generic;

public class StructValue
{
    public Position pos_start, pos_end;
    public Context context;
    public string name;
    public object statements;
    public bool already_declared;

    public StructValue(string name, object statements)
    {
        if (!already_declared)
        {
            this.name = name == null || name == "" ? "<anonymous>" : name;
            this.statements = statements;
            this.set_pos();
            this.set_context();
        }
    }

    public StructValue set_pos(Position pos_start = null, Position pos_end = null)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;

        return this;
    }

    public StructValue declare(Interpreter interpreter)
    {
        if (already_declared)
        {
            return this;
        }

        already_declared = true;
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

    public Tuple<object, Error> get_comparison_ee(object other)
    {
        return new Tuple<object, Error>(Values.FALSE, null); ;
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
        return "<struct " + this.name + ">";
    }
}