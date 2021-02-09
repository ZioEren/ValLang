public class Position
{
    public int idx, ln, col;
    public string fn, ftxt;

    public Position(int idx, int ln, int col, string fn, string ftxt)
    {
        this.idx = idx;
        this.ln = ln;
        this.col = col;
        this.fn = fn;
        this.ftxt = ftxt;
    }

    public Position advance(char current_char = default(char))
    {
        this.idx++;
        this.col++;

        if (current_char == '↩')
        {
            this.ln++;
            this.col = 0;
        }

        return this;
    }

    public Position copy()
    {
        return new Position(this.idx, this.ln, this.col, this.fn, this.ftxt);
    }
}