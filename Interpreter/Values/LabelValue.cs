public class LabelValue
{
    public string labelName;
    public object statements;

    public LabelValue(string labelName, object statements)
    {
        this.labelName = labelName;
        this.statements = statements;
    }
}