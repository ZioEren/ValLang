public class VarListAccessNode
{
    public Token var_name_tok;
    public object access_node;
    public Position pos_start, pos_end;

    public VarListAccessNode(Token var_name_tok, object access_node)
    {
        this.var_name_tok = var_name_tok;
        this.access_node = access_node;
        this.pos_start = var_name_tok.pos_start;
        this.pos_end = var_name_tok.pos_end;
    }
}