using System.Collections.Generic;
using System;

public class SwitchNode
{
    public Token var_name_tok;
    public List<Tuple<object, object>> cases = new List<Tuple<object, object>>();
    public object default_case;
    public Position pos_start, pos_end;

    public SwitchNode(Token var_name_tok, List<Tuple<object, object>> cases, object default_case)
    {
        this.var_name_tok = var_name_tok;
        this.cases = cases;
        this.default_case = default_case;
        this.pos_start = this.var_name_tok.pos_start.copy();

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