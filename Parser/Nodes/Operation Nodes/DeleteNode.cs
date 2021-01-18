public class DeleteNode
{
    public Token var_name_tok;
    public Position pos_start, pos_end;

    public DeleteNode(Token var_name_tok)
    {
        this.var_name_tok = var_name_tok;
        this.pos_start = var_name_tok.pos_start;
        this.pos_end = var_name_tok.pos_end;
    }
}