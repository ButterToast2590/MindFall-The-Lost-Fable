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

    // Define sprite variables for each condition
    [SerializeField] Sprite poisonSprite;
    [SerializeField] Sprite bruiseSprite;
    [SerializeField] Sprite dizzinessSprite;
    [SerializeField] Sprite allergySprite;
    [SerializeField] Sprite sprainSprite;

    public void SetData(Fables fable)
    {
        _fables = fable;

        Debug.Log("SetData method called for Fable: " + fable.Base.FableName);

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

        typeImage.sprite = fable.Base.TypeSprite;
        SetStatus();
        _fables.OnStatusChanged += SetStatus;

        if (fable.Status != null)
        {
            UpdateStatusImage(GetStatusSprite(fable.Status.Id));
        }
    }


    void SetStatus()
    {
        Debug.Log("SetStatus method called.");

        if (_fables.Status == null)
        {
            statusText.text = "";
            statusImage.sprite = null;
            statusImage.gameObject.SetActive(false); 
            statusText.gameObject.SetActive(false); 
        }
        else
        {
            statusText.text = _fables.Status.Id.ToString().ToUpper();
            statusText.color = StatusColors[_fables.Status.Id];

            statusImage.sprite = GetStatusSprite(_fables.Status.Id);
            statusImage.gameObject.SetActive(true); 
            statusText.gameObject.SetActive(true); 
        }
    }


    void UpdateStatusImage(Sprite statusSprite)
    {
        Debug.Log("UpdateStatusImage method called.");

        if (statusSprite != null)
        {
            statusImage.gameObject.SetActive(true);
            statusImage.sprite = statusSprite;
        }
        else
        {
            statusImage.gameObject.SetActive(false);
        }
    }

    Sprite GetStatusSprite(ConditionID conditionId)
    {
        switch (conditionId)
        {
            case ConditionID.Poison:
                return poisonSprite;
            case ConditionID.Bruise:
                return bruiseSprite;
            case ConditionID.Dizziness:
                return dizzinessSprite;
            case ConditionID.Allergy:
                return allergySprite;
            case ConditionID.Sprain:
                return sprainSprite;
            default:
                return null;
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
}
