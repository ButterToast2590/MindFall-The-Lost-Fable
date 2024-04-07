using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Color highlightedColor;
    [SerializeField] Button button;

    Fables _fables;
    int memberIndex;

    public void Initialize(Fables fable, int index)
    {
        _fables = fable;
        memberIndex = index; // Assign member index
        nameText.text = fable.Base.FableName;
        levelText.text = "Lvl " + fable.Level;
        hpBar.SetHP((float)fable.HP / fable.MaxHp);

        // Add listener to the button click event
        button.onClick.AddListener(OnClick);
    }
    public void SetData(Fables fable)
    {
        nameText.text = fable.Base.FableName;
        levelText.text = "Lvl " + fable.Level;
        hpBar.SetHP((float)fable.HP / fable.MaxHp);
    }
    void OnClick()
    {
        FindObjectOfType<BattleSystem>().HandlePartySelection(memberIndex); // Pass the memberIndex
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = highlightedColor;
            // Optionally, you can implement highlighting effects or animations here
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}
