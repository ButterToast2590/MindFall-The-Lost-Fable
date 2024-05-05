using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Image typeImage;

    Fables _fables;

    public void SetData(Fables fable)
    {
        _fables = fable;
        nameText.text = fable.Base.FableName;
        levelText.text = "Lvl " + fable.Level;
        hpBar.SetHP((float)fable.HP / fable.MaxHp);

        // Set fable type image
        typeImage.sprite = fable.Base.TypeSprite;
    }

    public IEnumerator UpdateHP()
    {
        if (_fables.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_fables.HP / _fables.MaxHp);
            _fables.HpChanged = false;  
        }
    }
}
