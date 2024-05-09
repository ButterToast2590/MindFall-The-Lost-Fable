using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHUD hud;
    [SerializeField] GameObject particleEffectPrefab;

    public bool IsPlayerUnit { get { return isPlayerUnit; } }
    public BattleHUD Hud { get { return hud; } }

    public Fables fables { get; set; }

    Image image;
    Vector3 originalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Fables fable)
    {
        this.fables = fable;
        if (isPlayerUnit)
            image.sprite = fable.Base.BackSpriteName;
        else
            image.sprite = fable.Base.FrontSpriteName;

        hud.gameObject.SetActive(true);
        hud.SetData(fable);

        transform.localScale = Vector3.one;
        image.color = originalColor;
        PlayEnterAnimation();
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPos.y);

        image.transform.DOLocalMoveX(originalPos.x, 1f).SetEase(Ease.OutQuad);
    }

    public void PlayAttackAnimation(BattleUnit sourceUnit, Vector3 targetPosition, float throwDuration)
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        sequence.AppendInterval(throwDuration); // Wait for the attack animation duration
        sequence.Append(image.transform.DOLocalMove(originalPos, 0.25f)); // Return to original position
        sequence.OnComplete(() => PlayParticleEffect(targetPosition)); // Trigger particle effect
    }


    private void PlayParticleEffect(Vector3 targetPosition)
    {
        if (particleEffectPrefab != null)
        {
            // Instantiate the particle effect at the target position with the prefab's rotation
            GameObject particleEffect = Instantiate(particleEffectPrefab, targetPosition, particleEffectPrefab.transform.rotation);

            // Get the duration of the particle effect animation
            ParticleSystem particleSystem = particleEffect.GetComponent<ParticleSystem>();
            float particleDuration = particleSystem ? particleSystem.main.duration : 0f;

            // Calculate the total duration of the particle effect animation
            float totalDuration = particleDuration + 1f; // Additional 1 second for safety

            // Destroy the particle effect after the total duration
            Destroy(particleEffect, totalDuration);

            // Wait for the total duration before proceeding
            StartCoroutine(WaitForParticleEffect(totalDuration));
        }
    }


    IEnumerator WaitForParticleEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        // Perform any action after the particle effect finishes
        Debug.Log("Particle effect finished!");
    }



    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
        sequence.Play();
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
        sequence.Play();
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, 0.5f));
        sequence.Join(transform.DOScale(Vector3.one, 0.5f));
        yield return sequence.WaitForCompletion();
    }
}
