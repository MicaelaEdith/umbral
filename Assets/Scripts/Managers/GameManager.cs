using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private MoneyFeedback moneyFeedback;
    [SerializeField] private TimeFeedback timeFeedback;

    [SerializeField] private GameObject mapButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject[] entranceInteractiveObjects;

    private readonly Stack<GameObject> subLocationStack = new Stack<GameObject>();
    private GameObject currentSubRoom;

    public string currentBuildingName;
    public Waypoint currentDestinationWaypoint;
    public string currentLocation;

    [SerializeField] private GameObject btnNote;
    [SerializeField] private GameObject btnNoteProgress5;
    [SerializeField] private MinigameTrigger triggerOnProgress5;
    [SerializeField] private int questProgressBacking;
    public int questProgress
    {
        get => questProgressBacking;
        set
        {
            questProgressBacking = value;
            if (btnNote != null)
            {
                btnNote.SetActive(value == 3);
                if (value == 3) AnimateUIElement(btnNote);
            }
            if (btnNoteProgress5 != null)
            {
                btnNoteProgress5.SetActive(value == 5);
                if (value == 5) AnimateUIElement(btnNoteProgress5);
            }
            if (value == 5 && triggerOnProgress5 != null)
                triggerOnProgress5.SetCanActivate(true);
            if (value == 6)
            {
                if (!isDayEnding)
                {
                    isDayEnding = true;
                    if (currentDay >= 3 && remainingMinutes < 120)
                        StartCoroutine(DefeatTransition());
                    else
                        StartCoroutine(Progress6Flow());
                }
            }
            if (value == 7)
                isProgress7TimerActive = false;

            if (!suppressVictoryCheck && value >= maxProgress && !victoryAchieved)
            {
                victoryAchieved = true;
                StartCoroutine(VictoryTransition());
            }
        }
    }
    public bool suppressVictoryCheck;
    public static bool IsInputLocked { get; set; }

    public bool shameActive;
    [Range(0f, 1f)] public float shameLevel;
    public int shameTimerMinutes { get; private set; }
    [Range(0f, 1f)] public float timedShameOpacity = 0.85f;
    [SerializeField] private int shameTimeMultiplier = 2;
    private int shameFlashCooldown;

    [SerializeField, Range(1, 5)] private int shameFadeOutMinutes = 3;
    private int shameFadeOutRemaining;
    private float shameFadeOutStartLevel;
    public bool IsShameFadingOut => shameFadeOutRemaining > 0;

    [SerializeField] private BuildingInterior[] allBuildings;
    [SerializeField] private TextMeshProUGUI timeLabel;
    [SerializeField] private TextMeshProUGUI moneyLabel;

    [SerializeField] private int remainingMinutes = 480;
    [SerializeField] private int currentMoney = 2000;

    [SerializeField] private TextMeshProUGUI dayLabel;
    [SerializeField] private GameObject endDayPanel;
    [SerializeField] private GameObject endDayPanelEarly;
    [SerializeField] private GameObject dayStartPanel;
    [SerializeField] private GameObject blackOverlay;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject shameIntroductionPanel;
    private bool hasShameIntroductionBeenShown;
    [SerializeField] private int maxProgress = 10;
    [SerializeField] private string[] dayNames = { "", "", "Jueves", "Viernes" };
    [SerializeField] private Waypoint houseWaypoint;
    [SerializeField] private Color feedbackColor = new Color(0.9f, 0.3f, 0.3f);

    private float timeAccumulator;
    private const float REAL_SECONDS_PER_GAME_MINUTE = 1f;
    private bool isProgress7TimerActive;
    private int progress6StartMinutes;
    private bool victoryAchieved;

    private const int DAY_MINUTES = 420;
    private int timeToSpend;
    private int currentDay = 2;
    private bool isDayEnding;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentBuildingName = "house";
        currentLocation = "house";
        UpdateTimeDisplay();
        UpdateMoneyDisplay();

        foreach (var building in allBuildings)
        {
            if (building.BuildingName == currentLocation)
                building.Show();
            else
                building.Hide();
        }
    }

    private void Update()
    {
        timeAccumulator += Time.deltaTime;
        if (timeAccumulator >= REAL_SECONDS_PER_GAME_MINUTE)
        {
            timeAccumulator -= REAL_SECONDS_PER_GAME_MINUTE;

            int minutesToDeduct = shameActive ? shameTimeMultiplier : 1;
            remainingMinutes -= minutesToDeduct;
            if (remainingMinutes < 0) remainingMinutes = 0;
            UpdateTimeDisplay();

            if (minutesToDeduct > 1)
            {
                shameFlashCooldown++;
                if (shameFlashCooldown >= 3)
                {
                    shameFlashCooldown = 0;
                    if (timeFeedback != null) timeFeedback.Flash();
                }
            }

            if (shameTimerMinutes > 0)
            {
                shameTimerMinutes--;
                if (shameTimerMinutes <= 0 && shameFadeOutRemaining <= 0)
                {
                    shameFadeOutRemaining = shameFadeOutMinutes;
                    shameFadeOutStartLevel = shameLevel;
                }
            }

            if (shameFadeOutRemaining > 0)
            {
                shameFadeOutRemaining--;
                float t = 1f - (float)shameFadeOutRemaining / shameFadeOutMinutes;
                shameLevel = Mathf.Lerp(shameFadeOutStartLevel, 0f, t);

                if (shameFadeOutRemaining <= 0)
                {
                    shameActive = false;
                    shameLevel = 0f;
                }
            }

            if (isProgress7TimerActive && progress6StartMinutes - remainingMinutes >= 120)
            {
                isProgress7TimerActive = false;
                questProgress = 7;
            }

            if (remainingMinutes <= 0 && !isDayEnding)
            {
                isDayEnding = true;
                if (currentDay <= 2)
                    StartCoroutine(EndDayTransition());
                else if (victoryAchieved)
                    StartCoroutine(VictoryTransition());
                else
                    StartCoroutine(DefeatTransition());
            }
        }

        if (timeToSpend > 0)
        {
            SpendTime(timeToSpend);
            timeToSpend = 0;
        }
    }

    private void UpdateTimeDisplay()
    {
        if (timeLabel != null)
        {
            int hours = remainingMinutes / 60;
            int minutes = remainingMinutes % 60;
            timeLabel.text = $"{hours:D2} : {minutes:D2} hs restantes";
        }
    }

    private void UpdateMoneyDisplay()
    {
        if (moneyLabel != null)
            moneyLabel.text = $"$ {currentMoney}";
    }

    public void SpendTime(int minutes)
    {
        remainingMinutes -= minutes;
        if (remainingMinutes < 0) remainingMinutes = 0;
        UpdateTimeDisplay();
        if (timeFeedback != null)
            timeFeedback.Flash();

        if (isProgress7TimerActive && progress6StartMinutes - remainingMinutes >= 120)
        {
            isProgress7TimerActive = false;
            questProgress = 7;
        }
    }

    public void ScheduleTime(int minutes)
    {
        timeToSpend += minutes;
    }

    public bool CanAfford(int amount)
    {
        return currentMoney >= amount;
    }

    public void SpendMoney(int amount)
    {
        currentMoney -= amount;
        if (currentMoney < 0) currentMoney = 0;
        UpdateMoneyDisplay();
        if (moneyFeedback != null)
            moneyFeedback.Flash();
    }

    public void UpdateLocation()
    {
        currentLocation = currentBuildingName;

        foreach (var building in allBuildings)
        {
            if (building == null) continue;
            if (building.BuildingName == currentLocation)
                building.Show();
            else
                building.Hide();
        }
    }

    public void SetShameTimed(float level, int durationMinutes)
    {
        shameActive = true;
        shameLevel = level;
        shameTimerMinutes = durationMinutes;
        shameFadeOutRemaining = 0;

        if (!hasShameIntroductionBeenShown)
        {
            hasShameIntroductionBeenShown = true;
            if (shameIntroductionPanel != null)
            {
                shameIntroductionPanel.SetActive(true);
                StartCoroutine(HideShameIntroductionPanel());
            }
        }
    }

    private IEnumerator HideShameIntroductionPanel()
    {
        yield return new WaitForSeconds(2f);
        if (shameIntroductionPanel != null)
            shameIntroductionPanel.SetActive(false);
    }

    public void TryHideShamePanel()
    {
        if (shameIntroductionPanel != null && shameIntroductionPanel.activeSelf)
        {
            StopCoroutine(nameof(HideShameIntroductionPanel));
            shameIntroductionPanel.SetActive(false);
        }
    }

    public void PushSubLocation(GameObject newLocation, GameObject backTarget = null)
    {
        subLocationStack.Push(backTarget != null ? backTarget : currentSubRoom);
        currentSubRoom = newLocation;

        if (mapButton != null) mapButton.SetActive(false);
        if (backButton != null) backButton.SetActive(true);

        foreach (var obj in entranceInteractiveObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    public void PopSubLocation()
    {
        if (IsInputLocked) return;
        if (subLocationStack.Count == 0) return;

        if (currentSubRoom != null)
            currentSubRoom.SetActive(false);

        currentSubRoom = subLocationStack.Pop();

        if (currentSubRoom != null)
            currentSubRoom.SetActive(true);

        if (subLocationStack.Count == 0)
        {
            if (backButton != null) backButton.SetActive(false);
            if (mapButton != null) mapButton.SetActive(true);

            foreach (var obj in entranceInteractiveObjects)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
    }

    private IEnumerator EndDayTransition(bool useEarlyPanel = false)
    {
        while (DialogueManager.Instance.IsDialogueActive || IsInputLocked || MinigameTrigger.ActiveCount > 0)
            yield return null;

        while (subLocationStack.Count > 0)
            PopSubLocation();

        yield return new WaitForSeconds(3f);

        GameObject panel = useEarlyPanel ? endDayPanelEarly : endDayPanel;
        if (panel != null) panel.SetActive(true);
        yield return new WaitForSeconds(3f);

        if (blackOverlay != null)
        {
            CanvasGroup cg = blackOverlay.GetComponent<CanvasGroup>();
            if (cg == null) cg = blackOverlay.AddComponent<CanvasGroup>();
            blackOverlay.SetActive(true);
            cg.alpha = 0f;
            float elapsed = 0f;
            while (elapsed < 4f)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, elapsed / 4f);
                yield return null;
            }
            cg.alpha = 1f;
        }

        if (panel != null) panel.SetActive(false);

        currentBuildingName = "house";
        UpdateLocation();

        PathDrawer pathDrawer = FindFirstObjectByType<PathDrawer>();
        Card[] cards = FindObjectsByType<Card>(FindObjectsSortMode.None);
        foreach (Card card in cards)
        {
            if (card.BuildingName == "house")
            {
                if (pathDrawer != null)
                    pathDrawer.SetCurrentWaypoint(card.DestinationWaypoint);
                currentDestinationWaypoint = card.DestinationWaypoint;
                break;
            }
        }

        remainingMinutes = DAY_MINUTES;
        timeAccumulator = 0;
        UpdateTimeDisplay();

        currentDay++;
        if (dayLabel != null && currentDay >= 0 && currentDay < dayNames.Length)
            dayLabel.text = dayNames[currentDay];

        if (blackOverlay != null)
        {
            CanvasGroup cg = blackOverlay.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0f;
            blackOverlay.SetActive(false);
        }

        if (dayStartPanel != null) dayStartPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (dayStartPanel != null) dayStartPanel.SetActive(false);

        if (questProgress == 6)
            questProgress = 7;

        isDayEnding = false;
    }

    private IEnumerator Progress6Flow()
    {
        yield return TwoHourFeedback();

        if (remainingMinutes < 120)
        {
            StartCoroutine(EndDayTransition(useEarlyPanel: true));
        }
        else
        {
            isProgress7TimerActive = true;
            progress6StartMinutes = remainingMinutes;
            ScheduleTime(120);
        }
    }

    private IEnumerator TwoHourFeedback()
    {
        int ticks = 8;
        int chunk = 120 / ticks;
        for (int i = 0; i < ticks; i++)
        {
            remainingMinutes -= chunk;
            if (remainingMinutes < 0) remainingMinutes = 0;
            UpdateTimeDisplay();
            if (timeFeedback != null) timeFeedback.Flash();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator VictoryTransition()
    {
        while (DialogueManager.Instance.IsDialogueActive || IsInputLocked || MinigameTrigger.ActiveCount > 0)
            yield return null;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            yield return new WaitForSeconds(4f);
        }

        // volver al menú principal
        UnityEngine.SceneManagement.SceneManager.LoadScene("main_menu");
    }

    private IEnumerator DefeatTransition()
    {
        while (DialogueManager.Instance.IsDialogueActive || IsInputLocked || MinigameTrigger.ActiveCount > 0)
            yield return null;

        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
            yield return new WaitForSeconds(4f);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("main_menu");
    }

    public void AnimateUIElement(GameObject target)
    {
        StartCoroutine(AnimateUIFeedback(target));
    }

    private IEnumerator AnimateUIFeedback(GameObject target)
    {
        if (target == null) yield break;

        RectTransform rect = target.GetComponent<RectTransform>();
        Image image = target.GetComponentInChildren<Image>(true);
        Color originalColor = image != null ? image.color : Color.white;
        Vector3 originalScale = rect != null ? rect.localScale : Vector3.one;
        float duration = 0.15f;
        float elapsed;

        elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            if (rect != null) rect.localScale = Vector3.Lerp(originalScale, originalScale * 1.3f, t);
            if (image != null) image.color = Color.Lerp(originalColor, feedbackColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            if (rect != null) rect.localScale = Vector3.Lerp(originalScale * 1.3f, originalScale, t);
            if (image != null) image.color = Color.Lerp(feedbackColor, originalColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (rect != null) rect.localScale = originalScale;
        if (image != null) image.color = originalColor;
    }
}
