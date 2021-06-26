public class StructDefNode
{
    public Token var_name_tok;
    public object statements_node;
    public Position pos_start, pos_end;

    public StructDefNode(Token var_name_tok, object statements_node)
    {
        this.var_name_tok = var_name_tok;
        this.statements_node = statements_node;
        this.pos_start = this.var_name_tok.pos_start;
        this.pos_end = (Position)statements_node.GetType().GetField("pos_end").GetValue(statements_node);
    }
}