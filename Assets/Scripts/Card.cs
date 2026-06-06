using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Card : MonoBehaviour
{
    [SerializeField]
    private string buildingName;

    [SerializeField]
    private Waypoint destinationWaypoint;

    [SerializeField]
    private TextMeshProUGUI walkTimeLabel;

    [SerializeField]
    private TextMeshProUGUI walkCostLabel;

    [SerializeField]
    private TextMeshProUGUI busTimeLabel;

    [SerializeField]
    private TextMeshProUGUI busCostLabel;

    private void Awake()
    {
        SetupButton("btn_walk");
        SetupButton("btn_bus");
    }

    private void SetupButton(string buttonName)
    {
        Transform btnTransform = transform.Find(buttonName);
        if (btnTransform == null) return;

        Image img = btnTransform.GetComponent<Image>();
        if (img == null) return;

        img.color = new Color(1f, 1f, 1f, 0f);

        EventTrigger trigger = btnTransform.GetComponent<EventTrigger>();
        if (trigger == null) trigger = btnTransform.gameObject.AddComponent<EventTrigger>();

        AddEvent(trigger, EventTriggerType.PointerEnter, _ =>
        {
            img.color = new Color(1f, 1f, 1f, 0.18f);
        });

        AddEvent(trigger, EventTriggerType.PointerExit, _ =>
        {
            img.color = new Color(1f, 1f, 1f, 0f);
        });

        AddEvent(trigger, EventTriggerType.PointerDown, _ =>
        {
            img.color = new Color(22f / 255f, 22f / 255f, 22f / 255f, 0.12f);
        });

        AddEvent(trigger, EventTriggerType.PointerUp, _ =>
        {
            img.color = new Color(1f, 1f, 1f, 0.25f);
        });
    }

    private void AddEvent(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    private void OnEnable()
    {
        UpdateTravelInfo();
    }

    private void UpdateTravelInfo()
    {
        PathDrawer drawer = FindFirstObjectByType<PathDrawer>();
        if (drawer == null) return;

        drawer.CalculateTravelOptions(drawer.TotalDistance, out int walkTime, out int busTime, out int busCost);

        if (walkTimeLabel != null)
            walkTimeLabel.text = $"{walkTime} min";
        if (walkCostLabel != null)
            walkCostLabel.text = "Gratis";
        if (busTimeLabel != null)
            busTimeLabel.text = $"{busTime} min";
        if (busCostLabel != null)
            busCostLabel.text = $"$ {busCost}";
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnWalk()
    {
        PathDrawer drawer = FindFirstObjectByType<PathDrawer>();
        if (drawer != null)
        {
            drawer.CalculateTravelOptions(drawer.TotalDistance, out int walkTime, out int _, out int _);
            GameManager.Instance.ScheduleTime(walkTime);
            drawer.SetCurrentWaypoint(destinationWaypoint);
        }

        GameManager.Instance.currentBuildingName = buildingName;
        GameManager.Instance.currentDestinationWaypoint = destinationWaypoint;
        GameManager.Instance.UpdateLocation();

        CloseCardAndMap();
    }

    public void OnBus()
    {
        PathDrawer drawer = FindFirstObjectByType<PathDrawer>();
        if (drawer != null)
        {
            drawer.CalculateTravelOptions(drawer.TotalDistance, out int _, out int busTime, out int busCost);

            if (!GameManager.Instance.CanAfford(busCost))
                return;

            GameManager.Instance.SpendMoney(busCost);
            GameManager.Instance.ScheduleTime(busTime);
            drawer.SetCurrentWaypoint(destinationWaypoint);
        }

        GameManager.Instance.currentBuildingName = buildingName;
        GameManager.Instance.currentDestinationWaypoint = destinationWaypoint;
        GameManager.Instance.UpdateLocation();

        CloseCardAndMap();
    }

    private void CloseCardAndMap()
    {
        gameObject.SetActive(false);

        MapClose mapClose = FindFirstObjectByType<MapClose>();
        if (mapClose != null)
            mapClose.Close();
    }
}
