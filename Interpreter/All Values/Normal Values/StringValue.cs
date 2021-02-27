using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StringValue
{
    public Position pos_start, pos_end;
    public Context context;
    public string value;

    public StringValue(string value)
    {
        this.value = value;
        this.set_pos();
        this.set_context();
    }

    public StringValue set_pos(Position pos_start = null, Position pos_end = null)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;

        return this;
    }

    public RuntimeResult execute(List<object> args)
    {
        return new RuntimeResult().failure(this.illegal_operation(args));
    }

    public StringValue copy()
    {
        StringValue copy = new StringValue(this.value);

        copy.set_pos(this.pos_start, this.pos_end);
        copy.set_context(this.context);

        return copy;
    }

    public StringValue set_context(Context context = null)
    {
        this.context = context;

        return this;
    }

    public Tuple<object, Error> plused_by(object other)
    {
        if (other.GetType() == typeof(StringValue))
        {
            return new Tuple<object, Error>(new StringValue(this.value + ((StringValue)other).value), null);
        }
        else if (other.GetType() == typeof(NumberValue))
        {
            return new Tuple<object, Error>(new StringValue(this.value + ((NumberValue)other).as_string()), null);
        }
        else if (other.GetType() == typeof(ListValue))
        {
            return new Tuple<object, Error>(((ListValue)other).plused_by(this).Item1, null);
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> muled_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            return new Tuple<object, Error>(new StringValue(String.Concat(Enumerable.Repeat(value, (int)((NumberValue)other).value))).set_context(this.context), null);
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> ined_by(object other)
    {
        if (other.GetType() == typeof(StringValue))
        {
            if (((StringValue)other).value.Contains(this.value))
            {
                return new Tuple<object, Error>(Values.TRUE, null);
            }

            return new Tuple<object, Error>(Values.FALSE, null);
        }
        else if (other.GetType() == typeof(ListValue))
        {
            foreach (object element in ((ListValue)other).elements)
            {
                if ((string)element.GetType().GetMethod("as_string").Invoke(element, new object[] { }) == this.value && element.GetType() == typeof(StringValue))
                {
                    return new Tuple<object, Error>(Values.TRUE, null);
                }
            }

            return new Tuple<object, Error>(Values.FALSE, null);
        }
        else if (other.GetType() == typeof(SetValue))
        {
            foreach (object element in ((SetValue)other).elements)
            {
                if ((string)element.GetType().GetMethod("as_string").Invoke(element, new object[] { }) == this.value && element.GetType() == typeof(StringValue))
                {
                    return new Tuple<object, Error>(Values.TRUE, null);
                }
            }

            return new Tuple<object, Error>(Values.FALSE, null);
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> get_comparison_ee(object other)
    {
        if (other.GetType() == typeof(StringValue))
        {
            if (this.value == ((StringValue) other).value)
            {
                return new Tuple<object, Error>(Values.TRUE, null);
            }
            else
            {
                return new Tuple<object, Error>(Values.FALSE, null);
            }
        }
        else if (other.GetType() == typeof(NumberValue))
        {
            if (this.value == ((NumberValue)other).as_string())
            {
                return new Tuple<object, Error>(Values.TRUE, null);
            }
            else
            {
                return new Tuple<object, Error>(Values.FALSE, null);
            }
        }

        return new Tuple<object, Error>(Values.FALSE, null);
    }

    public Tuple<object, Error> get_comparison_ne(object other)
    {
        if (other.GetType() == typeof(StringValue))
        {
            if (this.value != ((StringValue)other).value)
            {
                return new Tuple<object, Error>(Values.TRUE, null);
            }
            else
            {
                return new Tuple<object, Error>(Values.FALSE, null);
            }
        }
        else if (other.GetType() == typeof(NumberValue))
        {
            if (this.value != ((NumberValue)other).as_string())
            {
                return new Tuple<object, Error>(Values.TRUE, null);
            }
            else
            {
                return new Tuple<object, Error>(Values.FALSE, null);
            }
        }

        return new Tuple<object, Error>(Values.TRUE, null);
    }

    public Tuple<object, Error> anded_by(object other)
    {
        return new Tuple<object, Error>(this.is_true() && (bool)other.GetType().GetMethod("is_true").Invoke(other, new object[] { }) ? Values.TRUE : Values.FALSE, null);
    }

    public Tuple<object, Error> ored_by(object other)
    {
        return new Tuple<object, Error>(this.is_true() || (bool)other.GetType().GetMethod("is_true").Invoke(other, new object[] { }) ? Values.TRUE : Values.FALSE, null);
    }

    public Tuple<object, Error> notted()
    {
        return new Tuple<object, Error>(this.is_true() ? Values.FALSE : Values.TRUE, null);
    }

    public bool is_true()
    {
        return this.value.Length > 0;
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
        return value;
    }
}