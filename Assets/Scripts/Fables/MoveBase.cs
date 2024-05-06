using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Move", menuName = "Fable/Create new Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] new string name; // Use 'new' keyword to hide inherited member
    [TextArea]
    [SerializeField] string description;
    [SerializeField] fableType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;
    [SerializeField] int priority;
    [SerializeField] Sprite statusSprite;
    [SerializeField] GameObject particleEffectPrefab;

    public GameObject ParticleEffectPrefab { get { return particleEffectPrefab; } }

    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public fableType Type { get { return type; } }
    public int Power { get { return power; } }
    public int Accuracy { get { return accuracy; } }
    public int PP { get { return pp; } }
    public int Priority { get { return priority; } }
    public MoveCategory Category { get { return category; } }
    public MoveEffects Effects { get { return effects; } }
    public MoveTarget Target { get { return target; } }
    public Sprite StatusSprite { get { return statusSprite; } }


    public void UpdatePP(int newPP)
    {
        pp = newPP;
    }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    public List<StatBoost> Boosts { get { return boosts; } }
    public ConditionID Status { get { return status; } }

}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    Foe, Self
}
