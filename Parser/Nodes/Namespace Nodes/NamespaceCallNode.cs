using System.Collections.Generic;
public class NamespaceCallNode
{
    public Token struct_var_name_tok, access_var_name_tok;
    public List<object> arg_nodes;
    public Position pos_start, pos_end;

    public NamespaceCallNode(Token struct_var_name_tok, Token access_var_name_tok, List<object> arg_nodes)
    {
        this.struct_var_name_tok = struct_var_name_tok;
        this.access_var_name_tok = access_var_name_tok;
        this.arg_nodes = arg_nodes;
        this.pos_start = struct_var_name_tok.pos_start;
        this.pos_end = access_var_name_tok.pos_end;
    }
}