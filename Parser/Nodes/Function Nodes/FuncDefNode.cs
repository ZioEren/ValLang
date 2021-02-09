using System.Collections.Generic;
using System;

public class FuncDefNode
{
    public Token var_name_tok;
    public List<Tuple<Token, object>> arg_name_toks;
    public object body_node;
    public Position pos_start, pos_end;
    public bool should_auto_return;

    public FuncDefNode(Token var_name_tok, List<Tuple<Token, object>> arg_name_toks, object body_node, bool should_auto_return)
    {
        this.var_name_tok = var_name_tok;
        this.arg_name_toks = arg_name_toks;
        this.body_node = body_node;
        this.should_auto_return = should_auto_return;

        if (this.var_name_tok != null)
        {
            this.pos_start = this.var_name_tok.pos_start;
        }
        else if (this.arg_name_toks.Count > 0)
        {
            this.pos_start = this.arg_name_toks[0].Item1.pos_start;
        }
        else
        {
            this.pos_start = (Position) this.body_node.GetType().GetField("pos_start").GetValue(this.body_node);
        }

        this.pos_end = (Position)this.body_node.GetType().GetField("pos_end").GetValue(this.body_node);
    }
}