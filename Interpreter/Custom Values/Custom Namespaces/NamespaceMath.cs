using System;
using System.Collections.Generic;

public class NamespaceMath
{
    public bool already;

    public void execute_declare(Context exec_ctx)
    {
        if (!already)
        {
            exec_ctx.symbol_table.set("pi", new NumberValue((float)Math.PI));
            exec_ctx.symbol_table.set("e", new NumberValue((float)Math.E));
            exec_ctx.symbol_table.set("sin", true);
            exec_ctx.symbol_table.set("sinh", true);
            exec_ctx.symbol_table.set("cos", true);
            exec_ctx.symbol_table.set("cosh", true);
            exec_ctx.symbol_table.set("tan", true);
            exec_ctx.symbol_table.set("tanh", true);
            exec_ctx.symbol_table.set("log", true);
            exec_ctx.symbol_table.set("log10", true);
            exec_ctx.symbol_table.set("floor", true);
            exec_ctx.symbol_table.set("sqrt", true);
            exec_ctx.symbol_table.set("asin", true);
            exec_ctx.symbol_table.set("acos", true);
            exec_ctx.symbol_table.set("atan", true);
            exec_ctx.symbol_table.set("ceiling", true);
            exec_ctx.symbol_table.set("exp", true);
            exec_ctx.symbol_table.set("round", true);
            exec_ctx.symbol_table.set("truncate", true);
            exec_ctx.symbol_table.set("abs", true);
            exec_ctx.symbol_table.set("sign", true);
            exec_ctx.symbol_table.set("pow", true);
            exec_ctx.symbol_table.set("min", true);
            exec_ctx.symbol_table.set("max", true);
            exec_ctx.symbol_table.set("sqr", true);
            exec_ctx.symbol_table.set("rem", true);
            exec_ctx.symbol_table.set("cbrt", true);
            exec_ctx.symbol_table.set("expm1", true);
            exec_ctx.symbol_table.set("hypot", true);
            exec_ctx.symbol_table.set("fma", true);
            exec_ctx.symbol_table.set("degrees", true);
            exec_ctx.symbol_table.set("radians", true);
            exec_ctx.symbol_table.set("sec", true);
            exec_ctx.symbol_table.set("cosec", true);
            exec_ctx.symbol_table.set("asec", true);
            exec_ctx.symbol_table.set("acosec", true);
            exec_ctx.symbol_table.set("sech", true);
            exec_ctx.symbol_table.set("cosech", true);
            exec_ctx.symbol_table.set("asinh", true);
            exec_ctx.symbol_table.set("acosh", true);
            exec_ctx.symbol_table.set("asech", true);
            exec_ctx.symbol_table.set("acosech", true);
            already = true;
        }
    }

    public RuntimeResult execute_sin(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("angle");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Sin(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Angle must be a number", exec_ctx));
    }

    public List<string> get_sin()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("angle");

        return arg_names;
    }

    public RuntimeResult execute_sinh(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Sinh(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_sinh()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_cos(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("angle");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Cos(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Angle must be a number", exec_ctx));
    }

    public List<string> get_cos()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("angle");

        return arg_names;
    }

    public RuntimeResult execute_cosh(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Cosh(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_cosh()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_tan(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("angle");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Tan(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Angle must be a number", exec_ctx));
    }

    public List<string> get_tan()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("angle");

        return arg_names;
    }

    public RuntimeResult execute_tanh(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Tanh(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_tanh()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_log(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Log(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_log()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_log10(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Log10(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_log10()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_floor(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Floor(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_floor()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_sqrt(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Sqrt(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_sqrt()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_asin(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Asin(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_asin()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_acos(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Acos(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_acos()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_atan(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Atan(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_atan()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_ceiling(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Ceiling(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_ceiling()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_exp(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Exp(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_exp()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_round(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Round(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_round()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_truncate(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Truncate(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_truncate()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_abs(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Abs (double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_abs()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_sign(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Sign(double.Parse(((NumberValue)value).as_string()))));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_sign()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_pow(Context exec_ctx)
    {
        object value1 = exec_ctx.symbol_table.get("value1");
        object value2 = exec_ctx.symbol_table.get("value2");

        if (value1.GetType() == typeof(NumberValue))
        {
            if (value2.GetType() == typeof(NumberValue))
            {
                return new RuntimeResult().success(new NumberValue(Math.Pow(double.Parse(((NumberValue)value1).as_string()), double.Parse(((NumberValue)value2).as_string()))));
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value2 must be a number", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value1 must be a number", exec_ctx));
        }
    }

    public List<string> get_pow()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value1");
        arg_names.Add("value2");

        return arg_names;
    }

    public RuntimeResult execute_min(Context exec_ctx)
    {
        object value1 = exec_ctx.symbol_table.get("value1");
        object value2 = exec_ctx.symbol_table.get("value2");

        if (value1.GetType() == typeof(NumberValue))
        {
            if (value2.GetType() == typeof(NumberValue))
            {
                return new RuntimeResult().success(new NumberValue(Math.Min(double.Parse(((NumberValue)value1).as_string()), double.Parse(((NumberValue)value2).as_string()))));
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value2 must be a number", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value1 must be a number", exec_ctx));
        }
    }

    public List<string> get_min()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value1");
        arg_names.Add("value2");

        return arg_names;
    }

    public RuntimeResult execute_max(Context exec_ctx)
    {
        object value1 = exec_ctx.symbol_table.get("value1");
        object value2 = exec_ctx.symbol_table.get("value2");

        if (value1.GetType() == typeof(NumberValue))
        {
            if (value2.GetType() == typeof(NumberValue))
            {
                return new RuntimeResult().success(new NumberValue(Math.Max(double.Parse(((NumberValue)value1).as_string()), double.Parse(((NumberValue)value2).as_string()))));
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value2 must be a number", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value1 must be a number", exec_ctx));
        }
    }

    public List<string> get_max()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value1");
        arg_names.Add("value2");

        return arg_names;
    }

    public RuntimeResult execute_sqr(Context exec_ctx)
    {
        object value1 = exec_ctx.symbol_table.get("value1");
        object value2 = exec_ctx.symbol_table.get("value2");

        if (value1.GetType() == typeof(NumberValue))
        {
            if (value2.GetType() == typeof(NumberValue))
            {
                return new RuntimeResult().success(new NumberValue(Math.Pow(double.Parse(((NumberValue)value1).as_string()), 1.0D / double.Parse(((NumberValue)value2).as_string()))));
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value2 must be a number", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value1 must be a number", exec_ctx));
        }
    }

    public List<string> get_sqr()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value1");
        arg_names.Add("value2");

        return arg_names;
    }

    public RuntimeResult execute_rem(Context exec_ctx)
    {
        object value1 = exec_ctx.symbol_table.get("value1");
        object value2 = exec_ctx.symbol_table.get("value2");

        if (value1.GetType() == typeof(NumberValue))
        {
            if (value2.GetType() == typeof(NumberValue))
            {
                return new RuntimeResult().success(new NumberValue(Math.IEEERemainder(double.Parse(((NumberValue)value1).as_string()), 1.0D / double.Parse(((NumberValue)value2).as_string()))));
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value2 must be a number", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value1 must be a number", exec_ctx));
        }
    }

    public List<string> get_rem()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value1");
        arg_names.Add("value2");

        return arg_names;
    }

    public RuntimeResult execute_cbrt(Context exec_ctx)
    {
        object value1 = exec_ctx.symbol_table.get("value");

        if (value1.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Pow(double.Parse(((NumberValue)value1).as_string()), 1.0D / 3.0D)));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value1 must be a number", exec_ctx));
        }
    }

    public List<string> get_cbrt()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_expm1(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Math.Exp(double.Parse(((NumberValue)value).as_string())) - 1.0D));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
    }

    public List<string> get_expm1()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_hypot(Context exec_ctx)
    {
        object value1 = exec_ctx.symbol_table.get("value1");
        object value2 = exec_ctx.symbol_table.get("value2");

        if (value1.GetType() == typeof(NumberValue))
        {
            if (value2.GetType() == typeof(NumberValue))
            {
                return new RuntimeResult().success(new NumberValue(Math.Sqrt(Math.Pow(double.Parse(((NumberValue)value1).as_string()), 2.0D) + Math.Pow(double.Parse(((NumberValue)value2).as_string()), 2.0D))));
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value2 must be a number", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value1 must be a number", exec_ctx));
        }
    }

    public List<string> get_hypot()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value1");
        arg_names.Add("value2");

        return arg_names;
    }

    public RuntimeResult execute_atan2(Context exec_ctx)
    {
        object value1 = exec_ctx.symbol_table.get("value1");
        object value2 = exec_ctx.symbol_table.get("value2");

        if (value1.GetType() == typeof(NumberValue))
        {
            if (value2.GetType() == typeof(NumberValue))
            {
                return new RuntimeResult().success(new NumberValue(Math.Atan2(double.Parse(((NumberValue)value1).as_string()), double.Parse(((NumberValue)value2).as_string()))));
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value2 must be a number", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value1 must be a number", exec_ctx));
        }
    }

    public List<string> get_atan2()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value1");
        arg_names.Add("value2");

        return arg_names;
    }

    public RuntimeResult execute_fma(Context exec_ctx)
    {
        object x = exec_ctx.symbol_table.get("x");
        object y = exec_ctx.symbol_table.get("y");
        object z = exec_ctx.symbol_table.get("z");

        if (x.GetType() == typeof(NumberValue))
        {
            if (y.GetType() == typeof(NumberValue))
            {
                if (z.GetType() == typeof(NumberValue))
                {
                    return new RuntimeResult().success(new NumberValue(double.Parse(((NumberValue) x).as_string()) * double.Parse(((NumberValue)y).as_string()) + double.Parse(((NumberValue)z).as_string())));
                }
                else
                {
                    return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "z must be a number", exec_ctx));
                }
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "y must be a number", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "x must be a number", exec_ctx));
        }
    }

    public List<string> get_fma()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("x");
        arg_names.Add("y");
        arg_names.Add("z");

        return arg_names;
    }

    public RuntimeResult execute_radians(Context exec_ctx)
    {
        object value1 = exec_ctx.symbol_table.get("value");

        if (value1.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue((double.Parse(((NumberValue)value1).as_string()) * Math.PI) / 180.0D));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_radians()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_degrees(Context exec_ctx)
    {
        object value1 = exec_ctx.symbol_table.get("value");

        if (value1.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue((double.Parse(((NumberValue)value1).as_string()) * 180.0D) / Math.PI));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_degrees()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_sec(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Sec(double.Parse(((NumberValue) value).as_string()))));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_sec()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_cosec(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Cosec(double.Parse(((NumberValue)value).as_string()))));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_cosec()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_asec(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Arcsec(double.Parse(((NumberValue)value).as_string()))));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_asec()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_acosec(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(Arccosec(double.Parse(((NumberValue)value).as_string()))));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_acosec()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_sech(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(HSec(double.Parse(((NumberValue)value).as_string()))));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_sech()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_cosech(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(HCosec(double.Parse(((NumberValue)value).as_string()))));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_cosech()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_asinh(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(HArcsin(double.Parse(((NumberValue)value).as_string()))));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_asinh()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_acosh(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(HArccos(double.Parse(((NumberValue)value).as_string()))));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_acosh()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_asech(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(HArcsec(double.Parse(((NumberValue)value).as_string()))));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_asech()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public RuntimeResult execute_acosech(Context exec_ctx)
    {
        object value = exec_ctx.symbol_table.get("value");

        if (value.GetType() == typeof(NumberValue))
        {
            return new RuntimeResult().success(new NumberValue(HArccosec(double.Parse(((NumberValue)value).as_string()))));
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Value must be a number", exec_ctx));
        }
    }

    public List<string> get_acoasech()
    {
        List<string> arg_names = new List<string>();
        arg_names.Add("value");

        return arg_names;
    }

    public static double Sec(double x)
    {
        return 1 / Math.Cos(x);
    }

    public static double Cosec(double x)
    {
        return 1 / Math.Sin(x);
    }

    public static double Arcsec(double x)
    {
        return 2 * Math.Atan(1) - Math.Atan(Math.Sign(x) / Math.Sqrt(x * x - 1));
    }

    public static double Arccosec(double x)
    {
        return Math.Atan(Math.Sign(x) / Math.Sqrt(x * x - 1));
    }

    public static double HSec(double x)
    {
        return 2 / (Math.Exp(x) + Math.Exp(-x));
    }

    public static double HCosec(double x)
    {
        return 2 / (Math.Exp(x) - Math.Exp(-x));
    }

    public static double HArcsin(double x)
    {
        return Math.Log(x + Math.Sqrt(x * x + 1));
    }

    public static double HArccos(double x)
    {
        return Math.Log(x + Math.Sqrt(x * x - 1));
    }

    public static double HArcsec(double x)
    {
        return Math.Log((Math.Sqrt(-x * x + 1) + 1) / x);
    }

    public static double HArccosec(double x)
    {
        return Math.Log((Math.Sign(x) * Math.Sqrt(x * x + 1) + 1) / x);
    }
}