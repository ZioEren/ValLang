public class ReturnNode
{
    public object node_to_return;
    public Position pos_start, pos_end;

    public ReturnNode(object node_to_return, Position pos_start, Position pos_end)
    {
        this.node_to_return = node_to_return;
        this.pos_start = pos_start;
        this.pos_end = pos_end;
    }
}