using UnityEngine;
using System.Collections;

public class DoorSelector : MonoBehaviour
{
    [SerializeField] private bool isCorrect;
    [SerializeField] private string autoDialogueNpcId;
    [SerializeField] private string reproachNpcId = "Doctor_01";
    [SerializeField] private GameObject feedbackSprite;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float consultationDelay = 3f;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private MinigameTrigger triggerToReactivate;
    [SerializeField] private GameObject secondTerminal;
    [SerializeField] private GameObject ticketButton;

    private static bool isAnyDoorExecuting;

    private SpriteRenderer doorSr;
    private Color defaultColor;
    private SpriteRenderer feedbackSr;
    private bool isExecuting;
    private bool ticketWasActive;

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
        if (isExecuting || isAnyDoorExecuting) return;
        doorSr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        doorSr.color = defaultColor;
    }

    private void OnMouseDown()
    {
        if (isExecuting || isAnyDoorExecuting) return;
        isAnyDoorExecuting = true;
        isExecuting = true;
        GameManager.IsInputLocked = true;

        ticketWasActive = ticketButton != null && ticketButton.activeSelf;
        if (ticketButton != null) ticketButton.SetActive(false);

        StartCoroutine(ExecuteSequence());
    }

    private IEnumerator ExecuteSequence()
    {
        doorSr.color = defaultColor;

        if (GameManager.Instance != null && GameManager.Instance.questProgress >= 3)
        {
            yield return ShowDoctorAndPlayDialogue(GameManager.Instance.questProgress, reproachNpcId);
            yield return HideDoctor();

            GameManager.Instance.SetShameTimed(0.25f, 3);
        }
        else if (isCorrect)
        {
            yield return ShowDoctorAndPlayDialogue(1);

            yield return HideDoctor();

            yield return new WaitForSeconds(consultationDelay);

            if (GameManager.Instance != null)
                GameManager.Instance.questProgress = 42;

            yield return ShowDoctorAndPlayDialogue(42);

            yield return HideDoctor();

            if (GameManager.Instance != null)
                GameManager.Instance.questProgress = 3;

            if (ticketButton != null)
                ticketButton.SetActive(false);

            if (triggerToReactivate != null)
            {
                triggerToReactivate.SetCanActivate(true);
                if (secondTerminal != null)
                    triggerToReactivate.SetMinigameObject(secondTerminal);
            }
        }
        else
        {
            int progress = GameManager.Instance != null ? GameManager.Instance.questProgress : 2;
            yield return ShowDoctorAndPlayDialogue(progress);
            yield return HideDoctor();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ScheduleTime(2);
                GameManager.Instance.SetShameTimed(0.25f, 6);
            }

            if (ticketButton != null && ticketWasActive)
                ticketButton.SetActive(true);
        }

        GameManager.IsInputLocked = false;
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.CanSkipInput = true;
        isAnyDoorExecuting = false;
        isExecuting = false;
    }

    private IEnumerator ShowDoctorAndPlayDialogue(int requiredProgress = -1, string npcIdOverride = null)
    {
        if (feedbackSprite != null) feedbackSprite.SetActive(true);
        if (feedbackSr != null)
            yield return FadeSprite(feedbackSr, 0f, 1f, fadeDuration);

        string npcId = npcIdOverride ?? autoDialogueNpcId;
        bool completed = false;
        if (!string.IsNullOrEmpty(npcId))
        {
            DialogueManager.Instance.CanSkipInput = false;
            DialogueManager.Instance.PlayDialogueDirect(npcId, () => completed = true, requiredProgress);
            yield return new WaitUntil(() => completed);
        }
        else completed = true;
    }

    private IEnumerator HideDoctor()
    {
        if (feedbackSr != null)
            yield return FadeSprite(feedbackSr, 1f, 0f, fadeDuration);
        if (feedbackSprite != null) feedbackSprite.SetActive(false);
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
