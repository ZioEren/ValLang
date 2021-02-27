public class LabelValue
{
    public string labelName;
    public object statements;
    public Context context;
    public Position pos_start, pos_end;

    public LabelValue(string labelName, object statements)
    {
        this.labelName = labelName;
        this.statements = statements;
        this.set_pos();
        this.set_context();
    }

    public LabelValue set_pos(Position pos_start = null, Position pos_end = null)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;

        return this;
    }

    public LabelValue copy()
    {
        LabelValue copy = new LabelValue(this.labelName, this.statements);

        copy.set_pos(this.pos_start, this.pos_end);
        copy.set_context(this.context);

        return copy;
    }

    public LabelValue set_context(Context context = null)
    {
        this.context = context;

        return this;
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
        return "<label " + this.labelName + ">";
    }
}