using System;
using System.Collections.Generic;
using System.Linq;

public class ListValue
{
    public List<object> elements;
    public Position pos_start, pos_end;
    public Context context;

    public ListValue(List<object> elements)
    {
        this.elements = elements;
        this.set_pos();
        this.set_context();
    }

    public ListValue set_pos(Position pos_start = null, Position pos_end = null)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;

        return this;
    }

    public ListValue copy()
    {
        ListValue copy = new ListValue(this.elements);

        copy.set_pos(this.pos_start, this.pos_end);
        copy.set_context(this.context);

        return copy;
    }

    public ListValue set_context(Context context = null)
    {
        this.context = context;
        return this;
    }

    public Tuple<object, Error> plused_by(object other)
    {
        ListValue new_list = this.copy();
        new_list.elements.Add(other);
        return new Tuple<object, Error>(new_list, null);
    }

    public Tuple<object, Error> minused_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            ListValue new_list = this.copy();

            try
            {
                new_list.elements.Remove(new_list.elements[(int)((NumberValue)other).value]);
                return new Tuple<object, Error>(new_list, null);
            }
            catch (Exception)
            {
                return new Tuple<object, Error>(null, new RuntimeError(((NumberValue) other).pos_start, ((NumberValue) other).pos_end, "Element at this index could not be removed from list because index is out of bounds", this.context));
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> muled_by(object other)
    {
        if (other.GetType() == typeof(ListValue))
        {
            ListValue new_list = this.copy();
            new_list.elements.AddRange(((ListValue) other).elements);

            return new Tuple<object, Error>(new_list, null);
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }

    public Tuple<object, Error> dived_by(object other)
    {
        if (other.GetType() == typeof(NumberValue))
        {
            try
            {
                return new Tuple<object, Error>(this.elements[(int)((NumberValue) other).value], null);
            }
            catch (Exception)
            {
                return new Tuple<object, Error>(null, new RuntimeError(((NumberValue)other).pos_start, ((NumberValue)other).pos_end, "Element at this index could not be retrieved from list because index is out of bounds", this.context));
            }
        }

        return new Tuple<object, Error>(null, this.illegal_operation(other));
    }


    public Tuple<object, Error> get_comparison_ee(object other)
    {
        if (other.GetType() == typeof(ListValue))
        {
            for (int i = 0; i < this.elements.Count; i++)
            {
                if ((string) this.elements[i].GetType().GetMethod("as_string").Invoke(this.elements[i], new object[] { }) != (string)((ListValue) other).elements[i].GetType().GetMethod("as_string").Invoke(((ListValue) other).elements[i], new object[] { }))
                {
                    return new Tuple<object, Error>(Values.FALSE, null);
                }
            }

            return new Tuple<object, Error>(Values.TRUE, null);
        }

        return new Tuple<object, Error>(Values.FALSE, null);
    }

    public Tuple<object, Error> get_comparison_ne(object other)
    {
        if (other.GetType() == typeof(ListValue))
        {
            for (int i = 0; i < this.elements.Count; i++)
            {
                if ((string)this.elements[i].GetType().GetMethod("as_string").Invoke(this.elements[i], new object[] { }) != (string)((ListValue)other).elements[i].GetType().GetMethod("as_string").Invoke(((ListValue)other).elements[i], new object[] { }))
                {
                    return new Tuple<object, Error>(Values.TRUE, null);
                }
            }

            return new Tuple<object, Error>(Values.FALSE, null);
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
        foreach (object element in this.elements)
        {
            if ((bool) element.GetType().GetMethod("is_true").Invoke(element, new object[] { }) == false)
            {
                return false;
            }
        }

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
        string listStr = "[";

        foreach (object element in this.elements)
        {
            if (listStr == "[")
            {
                listStr += element.GetType().GetMethod("as_string").Invoke(element, new object[] { });
            }
            else
            {
                listStr += ", " + element.GetType().GetMethod("as_string").Invoke(element, new object[] { });
            }
        }

        return listStr + "]";
    }
}