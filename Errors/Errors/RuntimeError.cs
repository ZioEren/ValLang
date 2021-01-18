public class RuntimeError : Error
{
    public Context context;

    public RuntimeError(Position pos_start, Position pos_end, string details, Context context) : base(pos_start, pos_end, "Runtime error", details)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;
        this.details = details;
        this.context = context;
    }

    public string get_rt_error()
    {
        return this.generate_traceback() + this.error_name + ": " + this.details + "." + "\r\n";
    }

    public string generate_traceback()
    {
        string result = "";
        Position pos = this.pos_start;
        Context ctx = this.context;

        while (ctx != null)
        {
            result = "  File '" + pos.fn + "', line '" + (pos.ln + 1).ToString() + "', in '" + ctx.display_name + "'.\r\n" + result;
            pos = ctx.parent_entry_pos;
            ctx = ctx.parent;
        }

        return "Traceback (most recent call last):\r\n" + result;
    }
}