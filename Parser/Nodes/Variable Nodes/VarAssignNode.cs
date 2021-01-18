public class VarAssignNode
{
    public Token var_name_tok;
    public object value_node;
    public Position pos_start, pos_end;
    public bool rewritten;

    public VarAssignNode(Token var_name_tok, object value_node, bool rewritten = true)
    {
        this.var_name_tok = var_name_tok;
        this.value_node = value_node;
        this.rewritten = rewritten;
        this.pos_start = var_name_tok.pos_start;
        this.pos_end = (Position) value_node.GetType().GetField("pos_end").GetValue(value_node);
    }
}