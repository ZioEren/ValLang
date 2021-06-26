public class ImportNode
{
    public Token string_tok;
    public Position pos_start, pos_end;

    public ImportNode(Token string_tok)
    {
        this.string_tok = string_tok;
        this.pos_start = this.string_tok.pos_start;
        this.pos_end = this.string_tok.pos_end;
    }
}