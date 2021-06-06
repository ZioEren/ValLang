using System;

public class Token
{
    public TokenType type;
    public object value;
    public Position pos_start, pos_end;

    public Token(TokenType type, object value = null, Position pos_start = null, Position pos_end = null)
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
            return get_string_type() + ":" + this.value.ToString();
        }

        return get_string_type();
    }

    public string get_string_type()
    {
        return Enum.GetName(typeof(TokenType), this.type);
    }
}