using UnityEngine;
using System.Collections;

public class NpcTerminal : MonoBehaviour
{
    [SerializeField]
    private TerminalMonitor[] monitors;
    [SerializeField]
    private float minDelay = 0.3f;
    [SerializeField]
    private float maxDelay = 1.5f;
    [SerializeField]
    private float monitorPauseMin = 0.5f;
    [SerializeField]
    private float monitorPauseMax = 2f;
    [SerializeField]
    private float cyclePauseMin = 1f;
    [SerializeField]
    private float cyclePauseMax = 3f;

    [Header("Monitor 0 — correct button")]
    [SerializeField]
    private int correctButtonIndex = 2;

    private bool started;

    private void Start()
    {
        started = true;
        StartCoroutine(CycleLoop());
    }

    private void OnEnable()
    {
        if (started)
            StartCoroutine(CycleLoop());
    }

    public void ResetTerminal()
    {
        StopAllCoroutines();
        foreach (var m in monitors)
            m.Activate();
    }

    private IEnumerator CycleLoop()
    {
        while (true)
        {
            for (int i = 0; i < monitors.Length; i++)
            {
                if (i == 0)
                    monitors[i].SetCorrectSequence(GenerateMonitor0Sequence());

                yield return StartCoroutine(monitors[i].AutoExecute(minDelay, maxDelay));
            }

            float cyclePause = Random.Range(cyclePauseMin, cyclePauseMax);
            yield return new WaitForSeconds(cyclePause);
        }
    }

    private int[] GenerateMonitor0Sequence()
    {
        int okIndex = monitors[0].buttons.Length - 1;
        return new int[] { correctButtonIndex, okIndex };
    }
}
