using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private MoneyFeedback moneyFeedback;
    [SerializeField] private TimeFeedback timeFeedback;

    [SerializeField] private GameObject mapButton;
    [SerializeField] private GameObject backButton;

    private readonly Stack<GameObject> subLocationStack = new Stack<GameObject>();
    private GameObject currentSubRoom;

    public string currentBuildingName;
    public Waypoint currentDestinationWaypoint;
    public string currentLocation;

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

    [SerializeField] private int remainingMinutes = 660;
    [SerializeField] private int currentMoney = 2000;

    private float timeAccumulator;
    private const float REAL_SECONDS_PER_GAME_MINUTE = 1f;
    private int timeToSpend;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
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

            if (remainingMinutes <= 0)
            {
                Debug.Log("Se acabó el tiempo del día");
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
    }

    public void PushSubLocation(GameObject newLocation, GameObject backTarget = null)
    {
        subLocationStack.Push(backTarget != null ? backTarget : currentSubRoom);
        currentSubRoom = newLocation;

        if (mapButton != null) mapButton.SetActive(false);
        if (backButton != null) backButton.SetActive(true);
    }

    public void PopSubLocation()
    {
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
        }
    }
}
