using UnityEngine;
using System.Collections;

public class NpcTerminal : MonoBehaviour
{
    [SerializeField] private TerminalMonitor[] monitors;
    [SerializeField] private float minDelay = 0.5f;
    [SerializeField] private float maxDelay = 2f;

    [Header("Monitor 0 — biased button")]
    [SerializeField] private int correctButtonIndex = 2;
    [SerializeField][Range(0f, 1f)] private float correctBias = 0.8f;

    private void Start()
    {
        StartCoroutine(CycleLoop());
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

                float pause = Random.Range(minDelay, maxDelay);
                yield return new WaitForSeconds(pause);
            }

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private int[] GenerateMonitor0Sequence()
    {
        int okIndex = monitors[0].buttons.Length - 1;
        int chosen;

        if (Random.value < correctBias)
        {
            chosen = correctButtonIndex;
        }
        else
        {
            do
            {
                chosen = Random.Range(0, monitors[0].buttons.Length - 1);
            } while (chosen == correctButtonIndex);
        }

        return new int[] { chosen, okIndex };
    }
}
