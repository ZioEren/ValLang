using System;
using System.Collections.Generic;

public class NumberValue
{
    public object value;
    public Position pos_start, pos_end;
    public Context context;

    public NumberValue(object value)
    {
        this.value = value;
        this.set_pos();
        this.set_context();
    }

    public NumberValue set_pos(Position pos_start = null, Position pos_end = null)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;

        return this;
    }

    public NumberValue copy()
    {
        NumberValue copy = new NumberValue(this.value);

        copy.set_pos(this.pos_start, this.pos_end);
        copy.set_context(this.context);

        return copy;
    }

    public NumberValue set_context(Context context = null)
    {
        this.context = context;

        return this;
    }

    public Tuple<object, Error> logic_anded_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value & (int)value).set_context(this.context), null);
                }
                else
                {
                    return new Tuple<object, Error>(null, this.illegal_operation());
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(null, this.illegal_operation());
                }
                else
                {
                    return new Tuple<object, Error>(null, this.illegal_operation());
                }
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> logic_ored_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int) && value.GetType() == typeof(int))
            {
                return new Tuple<object, Error>(new NumberValue((int)this.value | (int)value).set_context(this.context), null);
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> left_shifted_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int) && value.GetType() == typeof(int))
            {
                return new Tuple<object, Error>(new NumberValue((int)this.value << (int)value).set_context(this.context), null);
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> right_shifted_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int) && value.GetType() == typeof(int))
            {
                return new Tuple<object, Error>(new NumberValue((int)this.value >> (int)value).set_context(this.context), null);
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> moduled_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int) && value.GetType() == typeof(int))
            {
                return new Tuple<object, Error>(new NumberValue((int)this.value % (int)value).set_context(this.context), null);
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> added_to(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value + (int)value).set_context(this.context), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value + (float)value).set_context(this.context), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value + (int)value).set_context(this.context), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value + (float)value).set_context(this.context), null);
                }
            }
        }
        else if (other.GetType() == typeof(StringValue))
        {
            return new Tuple<object, Error>(new StringValue(this.as_string() + ((StringValue)other).value), null);
        }
        else if (other.GetType() == typeof(ListValue))
        {
            return new Tuple<object, Error>(((ListValue) other).added_to(this).Item1, null);
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> subbed_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value - (int)value).set_context(this.context), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value - (float)value).set_context(this.context), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value - (int)value).set_context(this.context), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value - (float)value).set_context(this.context), null);
                }
            }
        }
        else if (other.GetType() == typeof(ListValue))
        {
            return new Tuple<object, Error>(((ListValue)other).subbed_by(this).Item1, null);
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> multed_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value * (int)value).set_context(this.context), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value * (float)value).set_context(this.context), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value * (int)value).set_context(this.context), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value * (float)value).set_context(this.context), null);
                }
            }
        }
        else if (other.GetType() == typeof(StringValue))
        {
            return new Tuple<object, Error>(((StringValue)other).multed_by(this).Item1, null);
        }
        else if (other.GetType() == typeof(ListValue))
        {
            return new Tuple<object, Error>(((ListValue)other).multed_by(this).Item1, null);
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> dived_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    if ((int) value == 0)
                    {
                        return new Tuple<object, Error>(null, new RuntimeError(((NumberValue)other).pos_start, ((NumberValue) other).pos_end, "Division by zero", this.context));
                    }

                    return new Tuple<object, Error>(new NumberValue((int)this.value / (int)value).set_context(this.context), null);
                }
                else
                {
                    if ((float)value == 0)
                    {
                        return new Tuple<object, Error>(null, new RuntimeError(((NumberValue)other).pos_start, ((NumberValue)other).pos_end, "Division by zero", this.context));
                    }

                    return new Tuple<object, Error>(new NumberValue((int)this.value / (float)value).set_context(this.context), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    if ((int)value == 0)
                    {
                        return new Tuple<object, Error>(null, new RuntimeError(((NumberValue)other).pos_start, ((NumberValue)other).pos_end, "Division by zero", this.context));
                    }

                    return new Tuple<object, Error>(new NumberValue((float)this.value / (int)value).set_context(this.context), null);
                }
                else
                {
                    if ((float)value == 0)
                    {
                        return new Tuple<object, Error>(null, new RuntimeError(((NumberValue)other).pos_start, ((NumberValue)other).pos_end, "Division by zero", this.context));
                    }

                    return new Tuple<object, Error>(new NumberValue((float)this.value / (float)value).set_context(this.context), null);
                }
            }
        }
        else if (other.GetType() == typeof(ListValue))
        {
            return new Tuple<object, Error>(((ListValue)other).dived_by(this).Item1, null);
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> powed_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)(Math.Pow((double)((int)this.value), (double)((int) value)))).set_context(this.context), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)(Math.Pow((double)((int)this.value), (double)((float)value)))).set_context(this.context), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)(Math.Pow((double)((float)this.value), (double)((int)value)))).set_context(this.context), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)(Math.Pow((double)((float)this.value), (double)((float)value)))).set_context(this.context), null);
                }
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> get_comparison_eq(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value == (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value == (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value == (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value == (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
        }
        else if (other.GetType() == typeof(StringValue))
        {
            return new Tuple<object, Error>(this.as_string() == ((StringValue) other).value ? Values.TRUE : Values.FALSE, null);
        }

        return new Tuple<object, Error>(Values.FALSE, null);
    }

    public Tuple<object, Error> get_comparison_ne(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value != (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value != (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value != (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value != (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
        }
        else if (other.GetType() == typeof(StringValue))
        {
            return new Tuple<object, Error>(this.as_string() != ((StringValue)other).value ? Values.TRUE : Values.FALSE, null);
        }

        return new Tuple<object, Error>(Values.TRUE, null);
    }

    public Tuple<object, Error> get_comparison_lt(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value < (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value < (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value < (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value < (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> get_comparison_gt(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value > (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value > (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value > (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value > (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> get_comparison_lte(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value <= (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value <= (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value <= (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value <= (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> get_comparison_gte(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value >= (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value >= (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value >= (int)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value >= (float)((NumberValue)other).value ? (int)1 : (int)0), null);
                }
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> anded_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value == 1 && (int)((NumberValue)other).value == 1 ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value == 1 && (float)((NumberValue)other).value == 1.0F ? (int)1 : (int)0), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value == 1.0F && (int)((NumberValue)other).value == 1 ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value == 1.0F && (float)((NumberValue)other).value == 1.0F ? (int)1 : (int)0), null);
                }
            }
        }

        return new Tuple<object, Error>(this.is_true() && (bool)other.GetType().GetMethod("is_true").Invoke(other, new object[] { }) ? Values.TRUE : Values.FALSE, null);
    }

    public Tuple<object, Error> ored_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            object value = ((NumberValue)other).value;

            if (this.value.GetType() == typeof(int))
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value == 1 || (int)((NumberValue)other).value == 1 ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((int)this.value == 1 || (float)((NumberValue)other).value == 1.0F ? (int)1 : (int)0), null);
                }
            }
            else
            {
                if (value.GetType() == typeof(int))
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value == 1.0F || (int)((NumberValue)other).value == 1 ? (int)1 : (int)0), null);
                }
                else
                {
                    return new Tuple<object, Error>(new NumberValue((float)this.value == 1.0F || (float)((NumberValue)other).value == 1.0F ? (int)1 : (int)0), null);
                }
            }
        }

        return new Tuple<object, Error>(this.is_true() || (bool)other.GetType().GetMethod("is_true").Invoke(other, new object[] { }) ? Values.TRUE : Values.FALSE, null);
    }

    public Tuple<object, Error> notted()
    {
        if (this.value.GetType() == typeof(int))
        {
            return new Tuple<object, Error>(new NumberValue((int)this.value == 1 ? (int)0 : (int)1), null);
        }
        else
        {
            return new Tuple<object, Error>(new NumberValue((float)this.value == 1.0F ? (int) 0 : (int)1), null);
        }
    }

    public Tuple<object, Error> logic_notted()
    {
        if (this.value.GetType() == typeof(int))
        {
            return new Tuple<object, Error>(new NumberValue((int)~(((int)(this.value)))), null);
        }
        else
        {
            return new Tuple<object, Error>(null, this.illegal_operation());
        }
    }

    public bool is_true()
    {
        if (this.value.GetType() == typeof(int))
        {
            return (int)this.value == 1;
        }
        else
        {
            return (float)this.value == 1.0F;
        }
    }

    public Error illegal_operation(object other = null)
    {
        if (other == null)
        {
            other = this;
        }

        return new RuntimeError(this.pos_start, (Position)other.GetType().GetField("pos_end").GetValue(other), "Illegal operation", this.context);
    }

    public RuntimeResult execute(List<object> args)
    {
        return new RuntimeResult().failure(this.illegal_operation(null));
    }

    public string as_string()
    {
        return this.value.ToString();
    }
}