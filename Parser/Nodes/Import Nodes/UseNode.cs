public class UseNode
{
    public Token namespace_tok;
    public Position pos_start, pos_end;

    public UseNode(Token namespace_tok)
    {
        this.namespace_tok = namespace_tok;
        this.pos_start = this.namespace_tok.pos_start;
        this.pos_end = this.namespace_tok.pos_end;
    }
}