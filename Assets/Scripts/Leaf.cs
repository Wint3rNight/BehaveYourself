using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;
    
    public delegate Status TickMultiple(int val);
    public TickMultiple ProcessMethodMultiple;
    
    public int Index;

    public Leaf(string v) { }

    public Leaf(string name, Tick processMethod)
    {
        Name = name;
        ProcessMethod = processMethod;
    }
    
    public Leaf(string name, int i,TickMultiple processMethod)
    {
        Name = name;
        ProcessMethodMultiple = processMethod;
        Index = i;
    }
    
    public Leaf(string name, Tick processMethod, int sortOrder)
    {
        Name = name;
        ProcessMethod = processMethod;
        SortOrder = sortOrder;
    }
    
    public override Status Process()
    {
        Node.Status s;
        if (ProcessMethod != null)
        {
            s = ProcessMethod();
        }
        else if (ProcessMethodMultiple != null)
        {
            s = ProcessMethodMultiple(Index);
        }
        else
        {
            s = Status.Failure;
        }
        Debug.Log(Name+ " "+ s);
        return s;
    }
}
