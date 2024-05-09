using System;
using UnityEngine;
using UnityEngine.UI;

public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }

    public Action<Fables> OnStart { get; set; }
    public Func<Fables, bool> OnBeforeMove { get; set; }
    public Action<Fables> OnAfterTurn { get; set; }

    public void Apply(Fables fables)
    {
        // Call any start action if available
        OnStart?.Invoke(fables);
    }
}
