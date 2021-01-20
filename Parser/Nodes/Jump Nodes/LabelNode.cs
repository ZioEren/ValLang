public class LabelNode
{
    public Token var_name_tok;
    public object statements;
    public Position pos_start, pos_end;
    
    public LabelNode(Token var_name_tok, object statements)
    {
        this.var_name_tok = var_name_tok;
        this.statements = statements;
        this.pos_start = this.var_name_tok.pos_start;
        this.pos_end = (Position) this.statements.GetType().GetField("pos_end").GetValue(this.statements);
    }
}