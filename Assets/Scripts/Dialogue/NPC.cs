using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private string npcId;
    [SerializeField] private string npcName;
    [SerializeField] private float hoverDarkenAmount = 0.85f;

    private SpriteRenderer sr;
    private Color defaultColor;
    private Animator animator;

    public string NpcId => npcId;
    public string NpcName => npcName;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        defaultColor = sr.color;
    }

    private void OnMouseEnter()
    {
        Color c = defaultColor;
        c.r *= hoverDarkenAmount;
        c.g *= hoverDarkenAmount;
        c.b *= hoverDarkenAmount;
        sr.color = c;
    }

    private void OnMouseExit()
    {
        sr.color = defaultColor;
    }

    private void OnMouseDown()
    {
        DialogueManager.Instance.StartDialogue(this);
    }

    public void PlayTalkAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Talk");
    }

    public void PlayIdleAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Idle");
    }
}
