using UnityEngine;
using System.Collections;

public class TerminalMonitor : MonoBehaviour
{
    public ClickableButton[] buttons;
    public int[] correctSequence;
    public GameObject ticketOnComplete;

    private int currentStep;
    private bool isActive;

    public bool IsComplete { get; private set; }

    public event System.Action OnCompleted;

    public void Activate()
    {
        gameObject.SetActive(true);
        isActive = true;
        currentStep = 0;
        IsComplete = false;

        foreach (var btn in buttons)
            btn.ResetButton();
    }

    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    public void SetCorrectSequence(int[] sequence)
    {
        correctSequence = sequence;
    }

    public bool ProcessClick(ClickableButton button)
    {
        if (!isActive || IsComplete) return false;

        int index = System.Array.IndexOf(buttons, button);
        if (index == -1) return false;

        for (int i = 0; i < buttons.Length && i < 6; i++)
        {
            if (i != index)
                buttons[i].ResetButton();
        }

        if (index == correctSequence[currentStep])
        {
            currentStep++;
            if (currentStep >= correctSequence.Length)
            {
                IsComplete = true;
                OnCompleted?.Invoke();
            }
            return true;
        }

        currentStep = 0;

        if (index >= 6)
            button.ResetButton();

        return false;
    }

    public void ShowTicketOnComplete()
    {
        isActive = false;
        foreach (var btn in buttons)
            btn.gameObject.SetActive(false);
        if (ticketOnComplete != null)
            ticketOnComplete.SetActive(true);
    }

    public void ResetMonitor()
    {
        currentStep = 0;
        foreach (var btn in buttons)
            btn.ResetButton();
    }

    public IEnumerator AutoExecute(float minDelay, float maxDelay)
    {
        Activate();

        for (int i = 0; i < correctSequence.Length; i++)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            if (!isActive) yield break;
            buttons[correctSequence[i]].PressNoNotify();
            currentStep++;
        }

        IsComplete = true;
        OnCompleted?.Invoke();
        Deactivate();
    }
}
