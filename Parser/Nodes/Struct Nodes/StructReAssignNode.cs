public class StructReAssignNode
{
    public Token var_name_tok, access_var_name_tok, op_tok;
    public object node;
    public Position pos_start, pos_end;

    public StructReAssignNode(Token var_name_tok, Token access_var_name_tok, Token op_tok, object node)
    {
        this.var_name_tok = var_name_tok;
        this.access_var_name_tok = access_var_name_tok;
        this.op_tok = op_tok;
        this.node = node;
        this.pos_start = (Position)this.var_name_tok.pos_start;
        this.pos_end = (Position)this.node.GetType().GetField("pos_end").GetValue(this.node);
    }
}