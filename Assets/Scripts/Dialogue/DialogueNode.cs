using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public int requiredProgress;
    [TextArea] public string playerLine;
    [TextArea] public string npcLine;
}

[CreateAssetMenu(menuName = "Umbral/Dialogue Node", fileName = "NewDialogueNode")]
public class DialogueNode : ScriptableObject
{
    [SerializeField]
    private string npcId;
    [SerializeField]
    private DialogueEntry[] entries;

    public string NpcId => npcId;
    public DialogueEntry[] Entries => entries;
}
