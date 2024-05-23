using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FableStatsDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fableNameText;
    [SerializeField] TextMeshProUGUI fableLevelText;
    [SerializeField] TextMeshProUGUI fableHPText;
    [SerializeField] Image fableImage;
    [SerializeField] TextMeshProUGUI fableStatsText;
    [SerializeField] TextMeshProUGUI fableMovesetText;

    public void SetFableData(Fables fable)
    {
        fableNameText.text = fable.Base.FableName;
        fableLevelText.text = "Level: " + fable.Level;
        fableHPText.text = "HP: " + fable.HP + "/" + fable.MaxHp;
        fableImage.sprite = fable.Base.FrontSpriteName;

        fableStatsText.text = $"Attack: {fable.Base.Attack}\n" +
                              $"Defense: {fable.Base.Defense}\n" +
                              $"Sp. Attack: {fable.Base.SpAttack}\n" +
                              $"Sp. Defense: {fable.Base.SpDefense}\n" +
                              $"Speed: {fable.Base.Speed}";

        string moveset = "Moves:\n";
        foreach (var move in fable.Base.LearnableMoves)
        {
            moveset += $"{move.Base.name} (Lvl {move.Level})\n";
        }
        fableMovesetText.text = moveset;
    }
}
