public class GotoNode
{
    public Token var_name_tok;
    public Position pos_start, pos_end;

    public GotoNode(Token var_name_tok)
    {
        this.var_name_tok = var_name_tok;
        this.pos_start = this.var_name_tok.pos_start;
        this.pos_end = this.var_name_tok.pos_end;
    }
}