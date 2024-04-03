using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Fable/Create new Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] fableType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;

    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public fableType Type { get { return type; } }
    public int Power { get { return power; } }
    public int Accuracy { get { return accuracy; } }
    public int PP { get { return pp; } } // PP property remains the same

    // No changes needed for other properties

    // If you want to update the move's PP during the battle, you can add a method like this:
    public void UpdatePP(int newPP)
    {
        pp = newPP;
    }
}
