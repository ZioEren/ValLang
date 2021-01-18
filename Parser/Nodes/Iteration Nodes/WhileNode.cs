public class WhileNode
{
    public object condition_node, body_node;
    public Position pos_start, pos_end;
    public bool should_return_null;

    public WhileNode(object condition_node, object body_node, bool should_return_null)
    {
        this.condition_node = condition_node;
        this.body_node = body_node;
        this.should_return_null = should_return_null;
        this.pos_start = (Position) this.condition_node.GetType().GetField("pos_start").GetValue(this.condition_node);
        this.pos_end = (Position)this.body_node.GetType().GetField("pos_end").GetValue(this.body_node);
    }
}