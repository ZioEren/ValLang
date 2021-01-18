public class ForNode
{
    public Token var_name_tok;
    public object start_value_node, end_value_node, step_value_node, body_node;
    public Position pos_start, pos_end;
    public bool should_return_null;

    public ForNode(Token var_name_tok, object start_value_node, object end_value_node, object step_value_node, object body_node, bool should_return_null)
    {
        this.var_name_tok = var_name_tok;
        this.start_value_node = start_value_node;
        this.end_value_node = end_value_node;
        this.step_value_node = step_value_node;
        this.body_node = body_node;
        this.should_return_null = should_return_null;
        this.pos_start = this.var_name_tok.pos_start;
        this.pos_end = (Position) this.body_node.GetType().GetField("pos_end").GetValue(this.body_node);
    }
}