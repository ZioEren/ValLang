using System;
using System.Collections.Generic;

public class IfNode
{
    public List<Tuple<object, object, bool>> cases;
    public Tuple<object, bool> else_case;
    public Position pos_start, pos_end;

    public IfNode(List<Tuple<object, object, bool>> cases, Tuple<object, bool> else_case)
    {
        this.cases = cases;
        this.else_case = else_case;
        this.pos_start = (Position)this.cases[0].Item1.GetType().GetField("pos_start").GetValue(this.cases[0].Item1);

        if (else_case != null)
        {
            object theObject = else_case.Item1;
            this.pos_end = (Position)theObject.GetType().GetField("pos_end").GetValue(theObject);
        }
        else
        {
            this.pos_end = (Position)this.cases[this.cases.Count - 1].Item1.GetType().GetField("pos_end").GetValue(this.cases[this.cases.Count - 1].Item1);
        }
    }
}