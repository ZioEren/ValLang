public class InvalidSyntaxError : Error
{
    public InvalidSyntaxError(Position pos_start, Position pos_end, string details = "") : base(pos_start, pos_end, "Invalid syntax", details)
    {
    }
}