using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueAction : CutsceneAction
{
    [SerializeField] string npcName;
    [SerializeField] Sprite npcSprite;
    [SerializeField] Dialog dialog;

    public override IEnumerator Play()
    {
        yield return DialogManager.Instance.ShowDialog(dialog, npcName, npcSprite, null); // Pass npcName, npcSprite, and null for onComplete
    }
}
