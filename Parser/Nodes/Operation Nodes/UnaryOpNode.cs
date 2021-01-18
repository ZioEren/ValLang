public class UnaryOpNode
{
    public Token op_tok;
    public object node;
    public Position pos_start, pos_end;

    public UnaryOpNode(Token op_tok, object node)
    {
        this.op_tok = op_tok;
        this.node = node;
        this.pos_start = op_tok.pos_start;
        this.pos_end = (Position)node.GetType().GetField("pos_end").GetValue(node);
    }

    public string as_string()
    {
        return "(" + this.op_tok.as_string() + ", " + node.GetType().GetMethod("as_string").Invoke(node, new object[] { }) + ")";
    }
}