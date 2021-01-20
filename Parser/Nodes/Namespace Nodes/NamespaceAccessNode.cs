public class NamespaceAccessNode
{
    public Token var_name_tok, access_var_name_tok;
    public Position pos_start, pos_end;

    public NamespaceAccessNode(Token var_name_tok, Token access_var_name_tok)
    {
        this.var_name_tok = var_name_tok;
        this.access_var_name_tok = access_var_name_tok;
        this.pos_start = this.var_name_tok.pos_start;
        this.pos_end = this.access_var_name_tok.pos_end;
    }
}