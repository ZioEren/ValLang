using System;
using System.Collections.Generic;

public class StructMath
{
    public bool already;

    public void execute_declare(Context exec_ctx)
    {
        if (!already)
        {
            exec_ctx.symbol_table.set("pi", new NumberValue((float)Math.PI));
            exec_ctx.symbol_table.set("sin", true);
            already = true;
        }    
    }

    public void execute_new_declaration(Context exec_ctx)
    {
        
    }

    public RuntimeResult execute_sin(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("angle");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Sin(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(null, null, "Angle must be a number", exec_ctx));
    }

    public List<string> get_sin()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("angle");

        return arg_names;
    }
}