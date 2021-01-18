public class RuntimeResult
{
    public object value, func_return_value;
    public Error error;
    public bool loop_should_continue, loop_should_break;

    public void reset()
    {
        this.value = null;
        this.error = null;
        this.func_return_value = null;
        this.loop_should_continue = false;
        this.loop_should_break = false;
    }

    public object register(RuntimeResult res)
    {
        this.error = res.error;
        this.func_return_value = res.func_return_value;
        this.loop_should_continue = res.loop_should_continue;
        this.loop_should_break = res.loop_should_break;

        return res.value;
    }

    public RuntimeResult success(object value)
    {
        this.reset();
        this.value = value;

        return this;
    }

    public RuntimeResult failure(Error error)
    {
        this.reset();
        this.error = error;

        return this;
    }

    public RuntimeResult success_return(object value)
    {
        this.reset();
        this.func_return_value = value;

        return this;
    }

    public RuntimeResult success_continue()
    {
        this.reset();
        this.loop_should_continue = true;

        return this;
    }

    public RuntimeResult success_break()
    {
        this.reset();
        this.loop_should_break = true;

        return this;
    }

    public bool should_return()
    {
        return this.error != null || this.func_return_value != null || this.loop_should_continue || this.loop_should_break;
    }
}