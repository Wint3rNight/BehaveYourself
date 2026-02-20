using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;
    
    public delegate Status TickMultiple(int val);
    public TickMultiple ProcessMethodMultiple;
    
    public int Index;

    public Leaf(){ }

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
        if (ProcessMethod != null)
        {
            return ProcessMethod();
        }
        else if (ProcessMethodMultiple != null)
        {
            return ProcessMethodMultiple(Index);
        }
        return Status.Failure;
    }
}
