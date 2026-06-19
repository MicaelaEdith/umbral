using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FirstMapTutorial : MonoBehaviour
{
    [Header("First Map")]
    [SerializeField] private GameObject firstMap;
    [SerializeField] private Graphic houseIndicator;
    [SerializeField] private Button houseButton;

    [Header("Settings")]
    [SerializeField] private float housePulseSpeed = 0.75f;
    [SerializeField][Range(0f, 1f)] private float maxAlpha = 0.588f;

    private Coroutine housePulse;
    private Day1Tutorial day1Tutorial;

    private void Start()
    {
        firstMap.SetActive(true);

        SetAlpha(houseIndicator, maxAlpha);
        housePulse = StartCoroutine(PulseRoutine(houseIndicator, housePulseSpeed, 0));

        houseButton.onClick.AddListener(OnHouseClicked);

        day1Tutorial = FindFirstObjectByType<Day1Tutorial>();
    }

    private void OnHouseClicked()
    {
        if (housePulse != null)
            StopCoroutine(housePulse);

        firstMap.SetActive(false);

        GameManager.Instance.currentBuildingName = "house";
        GameManager.Instance.UpdateLocation();

        if (day1Tutorial != null)
            day1Tutorial.StartThoughtBubbleSequence();
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
    }
}
