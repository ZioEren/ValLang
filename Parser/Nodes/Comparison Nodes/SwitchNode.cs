using System.Collections.Generic;
using System;

public class SwitchNode
{
    public object comparisonExpr;
    public List<Tuple<object, object>> cases = new List<Tuple<object, object>>();
    public object default_case;
    public Position pos_start, pos_end;

    public SwitchNode(object comparisonExpr, List<Tuple<object, object>> cases, object default_case)
    {
        this.comparisonExpr = comparisonExpr;
        this.cases = cases;
        this.default_case = default_case;
        this.pos_start = new Position(0, 0, 0, "", "");

        if (this.default_case != null)
        {
            this.pos_end = (Position) this.default_case.GetType().GetField("pos_end").GetValue(this.default_case);
        }
        else
        {
            object theElement = cases[0].Item1;
            this.pos_end = (Position)theElement.GetType().GetField("pos_end").GetValue(theElement);
        }
    }
}