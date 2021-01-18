using System.Collections.Generic;

public class CallNode
{
    public object node_to_call;
    public List<object> arg_nodes;
    public Position pos_start, pos_end;

    public CallNode(object node_to_call, List<object> arg_nodes)
    {
        this.node_to_call = node_to_call;
        this.arg_nodes = arg_nodes;
        this.pos_start = (Position) this.node_to_call.GetType().GetField("pos_start").GetValue(this.node_to_call);

        if (this.arg_nodes.Count > 0)
        {
            this.pos_end = (Position) this.arg_nodes[this.arg_nodes.Count - 1].GetType().GetField("pos_end").GetValue(this.arg_nodes[this.arg_nodes.Count - 1]);
        }
        else
        {
            this.pos_end = (Position)this.node_to_call.GetType().GetField("pos_end").GetValue(this.node_to_call);
        }
    }
}