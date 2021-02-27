using System.Collections.Generic;

public class SetNode
{
    public List<object> element_nodes;
    public Position pos_start, pos_end;

    public SetNode(List<object> element_nodes, Position pos_start, Position pos_end)
    {
        this.element_nodes = element_nodes;
        this.pos_start = pos_start;
        this.pos_end = pos_end;
    }
}