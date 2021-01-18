public class IllegalCharError : Error
{
    public IllegalCharError(Position pos_start, Position pos_end, string details) : base(pos_start, pos_end, "Illegal character", details)
    {
    }
}