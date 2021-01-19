public class ParseResult
{
    public Error error;
    public object node;
    public int advance_count, last_registered_advance_count, to_reverse_count;

    public void register_advancement()
    {
        this.advance_count++;
    }

    public object register(ParseResult res)
    {
        this.last_registered_advance_count = res.advance_count;
        this.advance_count += res.advance_count;

        if (res.error != null)
        {
            this.error = res.error;
        }

        return res.node;
    }

    public ParseResult success(object node)
    {
        this.node = node;

        return this;
    }

    public ParseResult failure(Error error)
    {
        if (this.error == null || this.advance_count == 0)
        {
            this.error = error;
        }
		
        return this;
    }

    public object try_register(ParseResult res)
    {
        if (res.error != null)
        {
            this.to_reverse_count = res.advance_count;
            return null;
        }

        return this.register(res);
    }
}