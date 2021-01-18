public class Token
{
    public string type;
    public object value;
    public Position pos_start, pos_end;

    public Token(string type, object value = null, Position pos_start = null, Position pos_end = null)
    {
        this.type = type;
        this.value = value;

        if (pos_start != null)
        {
            this.pos_start = pos_start.copy();
            this.pos_end = pos_start.copy().advance();
        }

        if (pos_end != null)
        {
            this.pos_end = pos_end;
        }
    }

    public string as_string()
    {
        if (this.value != null)
        {
            return this.type + ":" + this.value.ToString();
        }

        return this.type;
    }
}