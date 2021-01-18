public class ExpectedCharError : Error
{
    public ExpectedCharError(Position pos_start, Position pos_end, string details) : base(pos_start, pos_end, "Expected character", details)
    {
    }
}