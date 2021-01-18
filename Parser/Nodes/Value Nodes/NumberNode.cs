public class NumberNode
{
    public Token tok;
    public Position pos_start, pos_end;

    public NumberNode(Token tok)
    {
        this.tok = tok;
        this.pos_start = this.tok.pos_start;
        this.pos_end = this.tok.pos_end;
    }

    public string as_string()
    {
        return this.tok.as_string();
    }
}