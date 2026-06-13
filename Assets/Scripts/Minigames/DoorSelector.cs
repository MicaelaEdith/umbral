using UnityEngine;
using System.Collections;

public class DoorSelector : MonoBehaviour
{
    [SerializeField] private bool isCorrect;
    [SerializeField] private string autoDialogueNpcId;
    [SerializeField] private GameObject feedbackSprite;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private MinigameTrigger triggerToReactivate;
    [SerializeField] private GameObject secondTerminal;
    [SerializeField] private GameObject ticketButton;

    private SpriteRenderer doorSr;
    private Color defaultColor;
    private SpriteRenderer feedbackSr;
    private bool isExecuting;

    private void Awake()
    {
        doorSr = GetComponent<SpriteRenderer>();
        defaultColor = doorSr.color;
        if (feedbackSprite == null)
        {
            Transform t = transform.Find("sprite_dr");
            if (t != null) feedbackSprite = t.gameObject;
        }
        if (feedbackSprite != null)
        {
            feedbackSr = feedbackSprite.GetComponent<SpriteRenderer>();
            Color c = feedbackSr.color;
            c.a = 0f;
            feedbackSr.color = c;
        }
    }

    private void OnMouseEnter()
    {
        if (isExecuting) return;
        doorSr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        doorSr.color = defaultColor;
    }

    private void OnMouseDown()
    {
        if (isExecuting) return;
        isExecuting = true;
        StartCoroutine(ExecuteSequence());
    }

    private IEnumerator ExecuteSequence()
    {
        doorSr.color = defaultColor;

        if (feedbackSprite != null) feedbackSprite.SetActive(true);
        if (feedbackSr != null)
            yield return FadeSprite(feedbackSr, 0f, 1f, fadeDuration);

        bool completed = false;
        if (!string.IsNullOrEmpty(autoDialogueNpcId))
        {
            DialogueManager.Instance.PlayDialogueDirect(autoDialogueNpcId, () => completed = true);
            yield return new WaitUntil(() => completed);
        }
        else completed = true;

        if (feedbackSr != null)
            yield return FadeSprite(feedbackSr, 1f, 0f, fadeDuration);
        if (feedbackSprite != null) feedbackSprite.SetActive(false);

        if (!isCorrect && GameManager.Instance != null)
        {
            GameManager.Instance.ScheduleTime(2);
            GameManager.Instance.SetShameTimed(0.25f, 6);
        }
        else if (isCorrect)
        {
            if (ticketButton != null)
                ticketButton.SetActive(false);

            if (GameManager.Instance != null)
                GameManager.Instance.questProgress++;

            if (triggerToReactivate != null)
            {
                triggerToReactivate.SetCanActivate(true);
                if (secondTerminal != null)
                    triggerToReactivate.SetMinigameObject(secondTerminal);
            }
        }

        isExecuting = false;
    }

    private IEnumerator FadeSprite(SpriteRenderer sr, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Color c = sr.color;
            c.a = Mathf.Lerp(from, to, t);
            sr.color = c;
            yield return null;
        }
        Color final = sr.color;
        final.a = to;
        sr.color = final;
    }
}
