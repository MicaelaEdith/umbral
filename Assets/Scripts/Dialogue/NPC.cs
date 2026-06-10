using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private string npcId;
    [SerializeField] private string npcName;
    [SerializeField] private Color hoverColor = new Color(0.85f, 0.85f, 0.85f, 1f);

    private SpriteRenderer sr;
    private Color defaultColor;
    private Animator animator;

    public string NpcId => npcId;
    public string NpcName => npcName;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        defaultColor = sr.color;
    }

    private void OnMouseEnter()
    {
        sr.color = hoverColor;
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
