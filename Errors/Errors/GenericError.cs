public class GenericError : Error
{
    public GenericError(Position pos_start, Position pos_end, string details = "") : base(pos_start, pos_end, "Generic error", details)
    {
    }
}