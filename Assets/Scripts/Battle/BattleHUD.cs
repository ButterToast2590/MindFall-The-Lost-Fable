using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Image typeImage;
    [SerializeField] Image statusImage;

    [SerializeField] Color psnColor;
    [SerializeField] Color brsColor;
    [SerializeField] Color dizColor;
    [SerializeField] Color sprColor;
    [SerializeField] Color algColor;

    Fables _fables;
    Dictionary<ConditionID, Color> StatusColors;

    public void SetData(Fables fable)
    {
        _fables = fable;
        nameText.text = fable.Base.FableName;
        levelText.text = "Lvl " + fable.Level;
        hpBar.SetHP((float)fable.HP / fable.MaxHp);

        StatusColors = new Dictionary<ConditionID, Color>()
        {
            { ConditionID.Poison, psnColor },
            { ConditionID.Bruise, brsColor },
            { ConditionID.Dizziness, dizColor },
            { ConditionID.Allergy, algColor },
            { ConditionID.Sprain, sprColor }
        };

        // Set fable type image
        typeImage.sprite = fable.Base.TypeSprite;
        SetStatus();
        _fables.OnStatusChanged += SetStatus;
    }
    void SetStatus()
    {
        if (_fables.Status == null)
        {
            statusText.text = "";
            statusImage.sprite = null;
        }
        else
        {
            statusText.text = _fables.Status.Id.ToString().ToUpper();
            statusText.color = StatusColors[_fables.Status.Id];
            statusImage.sprite = _fables.Base.TypeSprite;
        }
    }
    public IEnumerator UpdateHP()
    {
        if (_fables.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_fables.HP / _fables.MaxHp);
            _fables.HpChanged = false;
        }
    }

    public void UpdateStatusImage(Sprite statusSprite)
    {
        if (statusSprite != null)
        {
            statusImage.sprite = statusSprite;
            statusImage.gameObject.SetActive(true);
        }
        else
        {
            statusImage.gameObject.SetActive(false);
        }
    }
}
