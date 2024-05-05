using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
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
        }
    };
}

public enum ConditionID
{
    None, Poison, Bruise, Dizziness, Sprain, Allergy
}
