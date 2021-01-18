public class BinOpNode
{
    public object left_node, right_node;
    public Token op_tok;
    public Position pos_start, pos_end;

    public BinOpNode(object left_node, Token op_tok, object right_node)
    {
        this.left_node = left_node;
        this.op_tok = op_tok;
        this.right_node = right_node;
        this.pos_start = (Position) left_node.GetType().GetField("pos_start").GetValue(left_node);
        this.pos_end = (Position)left_node.GetType().GetField("pos_end").GetValue(left_node);
    }

    public string as_string()
    {
        return "(" + left_node.GetType().GetMethod("as_string").Invoke(left_node, new object[] { }) + ", " + this.op_tok.as_string() + ", " + right_node.GetType().GetMethod("as_string").Invoke(right_node, new object[] { }) + ")";
    }
}