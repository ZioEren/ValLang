public class VarReAssignNode
{
    public Token var_name_tok, op_tok;
    public object value_node;
    public Position pos_start, pos_end;

    public VarReAssignNode(Token var_name_tok, Token op_tok, object value_node)
    {
        this.var_name_tok = var_name_tok;
        this.op_tok = op_tok;
        this.value_node = value_node;
        this.pos_start = var_name_tok.pos_start;

        if (this.value_node != null)
        {
            this.pos_end = (Position)value_node.GetType().GetField("pos_end").GetValue(value_node);
        }
        else
        {
            this.pos_end = this.var_name_tok.pos_end;
        }
    }
}