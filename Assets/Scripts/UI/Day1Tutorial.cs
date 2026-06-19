using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Day1Tutorial : MonoBehaviour
{
    [Header("Thought Bubble")]
    [SerializeField] private GameObject thoughtBubble;

    [Header("Keys")]
    [SerializeField] private SpriteRenderer keysPulseIndicator;
    [SerializeField] private ClickableObject keysClickable;
    [SerializeField] private GameObject keysObject;

    [Header("Map Button")]
    [SerializeField] private Button mapButton;
    [SerializeField] private Graphic mapBtnIndicator;

    [Header("End Panel")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Image blackOverlay;

    [Header("Settings")]
    [SerializeField] private float thoughtBubbleDelay = 2f;
    [SerializeField] private float pulseSpeed = 1.5f;
    [SerializeField] private int pulseCycles = 2;
    [SerializeField][Range(0f, 1f)] private float maxAlpha = 0.67f;

    private int tutorialProgressBacking;
    public int tutorialProgress
    {
        get => tutorialProgressBacking;
        set
        {
            tutorialProgressBacking = value;
            OnProgressChanged();
        }
    }

    private Coroutine pulseRoutine;
    private bool filterSet;

    private void Start()
    {
        thoughtBubble.SetActive(false);

        if (keysPulseIndicator != null)
            keysPulseIndicator.gameObject.SetActive(false);

        if (mapButton != null)
        {
            mapButton.interactable = false;
            mapButton.onClick.AddListener(OnMapButtonClicked);
        }

        if (mapBtnIndicator != null)
            mapBtnIndicator.gameObject.SetActive(false);

        if (keysClickable != null)
            keysClickable.OnClick.AddListener(OnKeysClicked);
    }

    public void StartThoughtBubbleSequence()
    {
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(thoughtBubbleDelay);

        thoughtBubble.SetActive(true);
        tutorialProgress = 1;
    }

    private void Update()
    {
        if (tutorialProgress == 1 && Input.GetMouseButtonDown(0))
        {
            tutorialProgress = 2;
            return;
        }

        if (tutorialProgress == 3 && GameManager.Instance.currentBuildingName == "school")
        {
            tutorialProgress = 4;
        }
    }

    private void OnKeysClicked()
    {
        if (tutorialProgress != 2) return;
        tutorialProgress = 3;
    }

    private void OnMapButtonClicked()
    {
        if (tutorialProgress != 3) return;

        StopPulse();
        if (mapBtnIndicator != null)
            mapBtnIndicator.gameObject.SetActive(false);

        filterSet = true;
        Destination.blockUnlessInList = true;
        Destination.allowedNames = new string[] { "school" };

        MapToggle toggle = FindFirstObjectByType<MapToggle>();
        if (toggle != null)
            toggle.OpenMap();
    }

    private void OnProgressChanged()
    {
        switch (tutorialProgress)
        {
            case 2:
                thoughtBubble.SetActive(false);
                if (keysPulseIndicator != null)
                {
                    keysPulseIndicator.gameObject.SetActive(true);
                    SetAlpha(keysPulseIndicator, 0f);
                }
                pulseRoutine = StartCoroutine(PulseRoutine(keysPulseIndicator, pulseSpeed, pulseCycles));
                break;

            case 3:
                StopPulse();
                if (keysPulseIndicator != null)
                    keysPulseIndicator.gameObject.SetActive(false);
                if (keysObject != null)
                    keysObject.SetActive(false);

                if (mapButton != null)
                    mapButton.interactable = true;
                if (mapBtnIndicator != null)
                {
                    mapBtnIndicator.gameObject.SetActive(true);
                    SetAlpha(mapBtnIndicator, 0f);
                    pulseRoutine = StartCoroutine(PulseRoutine(mapBtnIndicator, pulseSpeed, pulseCycles));
                }
                break;

            case 4:
                StartCoroutine(StartSchoolDialogue());
                break;

            case 5:
                StartCoroutine(EndDayTutorial());
                break;
        }
    }

    private IEnumerator StartSchoolDialogue()
    {
        yield return new WaitForSeconds(2f);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "day_1")
        {
            tutorialProgress = 5;
            yield break;
        }

        DialogueUI ui = FindFirstObjectByType<DialogueUI>();
        if (ui != null)
            ui.AutoAdvanceDelay = 4.5f;

        DialogueManager.Instance.PlayDialogueDirect("Secretary", OnSchoolDialogueDone, 0);
    }

    private void OnSchoolDialogueDone()
    {
        tutorialProgress = 5;
    }

    private IEnumerator EndDayTutorial()
    {
        yield return new WaitForSeconds(2f);

        if (endPanel != null)
        {
            CanvasGroup panelCg = endPanel.GetComponent<CanvasGroup>();
            if (panelCg == null) panelCg = endPanel.AddComponent<CanvasGroup>();
            panelCg.alpha = 0f;
            endPanel.SetActive(true);

            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                panelCg.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            panelCg.alpha = 1f;
        }

        yield return new WaitForSeconds(3.5f);

        if (blackOverlay != null)
        {
            CanvasGroup cg = blackOverlay.GetComponent<CanvasGroup>();
            if (cg == null) cg = blackOverlay.gameObject.AddComponent<CanvasGroup>();
            blackOverlay.gameObject.SetActive(true);
            cg.alpha = 0f;

            float elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, elapsed / 1f);
                yield return null;
            }
            cg.alpha = 1f;
        }

        if (endPanel != null)
            endPanel.SetActive(false);

        UnityEngine.SceneManagement.SceneManager.LoadScene("day_2");
    }

    private IEnumerator PulseRoutine(SpriteRenderer sr, float speed, int maxCycles)
    {
        if (sr == null) yield break;

        float t = 0f;
        float cycleDuration = (2f * maxAlpha) / speed;
        float totalDuration = maxCycles > 0 ? cycleDuration * maxCycles : float.MaxValue;

        while (t < totalDuration)
        {
            float alpha = Mathf.PingPong(t, maxAlpha);
            SetAlpha(sr, alpha);
            t += Time.deltaTime * speed;
            yield return null;
        }

        if (sr == null) yield break;
        SetAlpha(sr, 0f);
    }

    private IEnumerator PulseRoutine(Graphic graphic, float speed, int maxCycles)
    {
        if (graphic == null) yield break;

        float t = 0f;
        float cycleDuration = (2f * maxAlpha) / speed;
        float totalDuration = maxCycles > 0 ? cycleDuration * maxCycles : float.MaxValue;

        while (t < totalDuration)
        {
            float alpha = Mathf.PingPong(t, maxAlpha);
            SetAlpha(graphic, alpha);
            t += Time.deltaTime * speed;
            yield return null;
        }

        if (graphic == null) yield break;
        SetAlpha(graphic, 0f);
    }

    private void SetAlpha(SpriteRenderer sr, float alpha)
    {
        if (sr == null) return;
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }

    private void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic == null) return;
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }

    private void StopPulse()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }
    }

    private void OnDestroy()
    {
        if (keysClickable != null)
            keysClickable.OnClick.RemoveListener(OnKeysClicked);
        if (mapButton != null)
            mapButton.onClick.RemoveListener(OnMapButtonClicked);
        if (filterSet)
        {
            Destination.blockUnlessInList = false;
            Destination.allowedNames = null;
        }
        StopPulse();
    }
}
