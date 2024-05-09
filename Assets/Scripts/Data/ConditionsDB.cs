using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach(var kvp in Conditions)
        {
            var conditonId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditonId;
        }
    }
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
{
    {
        ConditionID.Poison,
        new Condition()
        {
            Name = "Poison",
            StartMessage = "Has been poisoned",
            OnAfterTurn = (Fables fables) =>
            {
                fables.UpdateHP(fables.MaxHp / 8);
                fables.StatusChanges.Enqueue($"{fables.Base.FableName} hurt itself due to poison");
            }
        }
    },
    {
        ConditionID.Bruise,
        new Condition()
        {
            Name = "Bruise",
            StartMessage = "Has suffered a mild Bruised",
            OnAfterTurn = (Fables fables) =>
            {
                fables.UpdateHP(fables.MaxHp / 5);
                fables.StatusChanges.Enqueue($"{fables.Base.FableName} felt a sharp pain due to its bruise");
            }
        }
    },
    {
        ConditionID.Dizziness,
        new Condition()
        {
            Name = "Dizziness",
            StartMessage = "Is dizzy! It may stumble or miss its next action.",
            OnBeforeMove = (Fables fables) =>
            {
                if (Random.Range(1, 4) == 1)
                {
                    fables.StatusChanges.Enqueue($"{fables.Base.FableName} stumbled and missed its attack.");
                    return false;
                }
                fables.CureStatus();
                return true;
            }
        }
    },
    {
        ConditionID.Allergy,
        new Condition()
        {
            Name = "Allergy",
            StartMessage = "Has developed an allergy! it skip a turn waiting to recover",
            OnBeforeMove = (Fables fables) =>
            {
                if (Random.Range(1, 4) == 1)
                {
                    fables.CureStatus();
                    fables.StatusChanges.Enqueue($"{fables.Base.FableName} recover from its allergy.");
                    return true;
                }

                return false;
            }
        }
    },
    {
        ConditionID.Sprain,
        new Condition()
        {
            Name = "Sprain",
            StartMessage = "Fell and sprained itself! Its movements seem restricted.",
            OnStart = (Fables fables) =>
            {
                fables.StatusTime = Random.Range(1, 4);
                Debug.Log($"Will skip turn for {fables.StatusTime} round");
            },

            OnBeforeMove = (Fables fables) =>
            {
                if (fables.StatusTime <= 0)
                {
                    fables.CureStatus();
                    fables.StatusChanges.Enqueue($"{fables.Base.FableName} recover from its sprain");
                    return true;
                }
                fables.StatusTime--;
                fables.StatusChanges.Enqueue($"{fables.Base.FableName} is still sprained.");
                return false;
            }
        }
    }
};

    public static void ApplyCondition(ConditionID conditionId, Fables fables, Sprite statusSprite)
    {
        if (Conditions.TryGetValue(conditionId, out Condition condition))
        {
            // Apply the condition to the fables instance
            condition.Apply(fables);
        }
    }
}

public enum ConditionID
{
    None, Poison, Bruise, Dizziness, Sprain, Allergy
}
