public class Context
{
    public string display_name;
    public Context parent;
    public Position parent_entry_pos;
    public SymbolTable symbol_table;

    public Context(string display_name, Context parent = null, Position parent_entry_pos = null)
    {
        this.display_name = display_name;
        this.parent = parent;
        this.parent_entry_pos = parent_entry_pos;
    }
}