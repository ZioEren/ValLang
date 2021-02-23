public class Error
{
    public string error_name, details;
    public Position pos_start, pos_end;

    public Error(Position pos_start, Position pos_end, string error_name, string details)
    {
        this.pos_start = pos_start;
        this.pos_end = pos_end;
        this.error_name = error_name;
        this.details = details;
    }

    public string as_string()
    {
        return this.error_name + ": " + this.details + "." + "\r\nFile '" + this.pos_start.fn + "', line '" + (this.pos_start.ln + 1).ToString() + "'.";
    }
}