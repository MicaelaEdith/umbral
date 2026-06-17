using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FirstMapTutorial : MonoBehaviour
{
    [Header("First Map")]
    [SerializeField] private GameObject firstMap;
    [SerializeField] private Graphic houseIndicator;
    [SerializeField] private Button houseButton;

    [Header("Map Button Indicator")]
    [SerializeField] private Graphic mapBtnIndicator;
    [SerializeField] private Button mapButton;

    [Header("Settings")]
    [SerializeField] private float housePulseSpeed = 0.75f;
    [SerializeField] private float mapPulseSpeed = 1.5f;
    [SerializeField][Range(0f, 1f)] private float maxAlpha = 0.588f;
    [SerializeField] private int mapPulseCycles = 2;

    private Coroutine housePulse;
    private Coroutine mapPulse;
    private bool mapClicked;

    private void Start()
    {
        firstMap.SetActive(true);

        if (mapBtnIndicator != null)
            mapBtnIndicator.gameObject.SetActive(false);

        SetAlpha(houseIndicator, maxAlpha);
        housePulse = StartCoroutine(PulseRoutine(houseIndicator, housePulseSpeed, 0));

        houseButton.onClick.AddListener(OnHouseClicked);
    }

    private void OnHouseClicked()
    {
        if (housePulse != null)
            StopCoroutine(housePulse);

        firstMap.SetActive(false);

        GameManager.Instance.currentBuildingName = "house";
        GameManager.Instance.UpdateLocation();

        if (mapBtnIndicator != null)
        {
            mapBtnIndicator.gameObject.SetActive(true);
            SetAlpha(mapBtnIndicator, maxAlpha);
            mapPulse = StartCoroutine(PulseRoutine(mapBtnIndicator, mapPulseSpeed, mapPulseCycles));
        }

        if (mapButton != null)
        {
            mapButton.onClick.AddListener(OnMapClicked);
        }
    }

    private void OnMapClicked()
    {
        if (mapClicked) return;
        mapClicked = true;

        if (mapPulse != null)
            StopCoroutine(mapPulse);
        if (mapBtnIndicator != null)
            mapBtnIndicator.gameObject.SetActive(false);

        Destination.blockUnlessInList = true;
        Destination.allowedNames = new string[] { "House", "School" };
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
        graphic.gameObject.SetActive(false);
    }

    private void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic == null) return;
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }

    private void OnDestroy()
    {
        if (housePulse != null) StopCoroutine(housePulse);
        if (mapPulse != null) StopCoroutine(mapPulse);
        Destination.blockUnlessInList = false;
        Destination.allowedNames = null;
    }
}
