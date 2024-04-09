using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Color highlightedColor;

    Fables _fable;
    bool isSelected;

    public void SetData(Fables fables)
    {
        _fable = fables;

        nameText.text = _fable.Base.FableName;
        levelText.text = "Lvl " + _fable.Level;
        hpBar.SetHP((float)_fable.HP / _fable.MaxHp);
    }
    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = highlightedColor;
        else
            nameText.color = Color.black;
    }
}
