using UnityEngine;

public class SignboardDirector : MonoBehaviour
{
    [SerializeField]
    private GameObject leftRoom;
    [SerializeField]
    private GameObject rightRoom;
    [SerializeField]
    private GameObject previousRoom;

    public void SelectOption(int index)
    {
        bool isRight = index == 0 || index == 2;

        GameObject selectedRoom = isRight ? rightRoom : leftRoom;

        if (leftRoom != null)
            leftRoom.SetActive(isRight == false);

        if (rightRoom != null)
            rightRoom.SetActive(isRight);

        if (selectedRoom != null && GameManager.Instance != null)
            GameManager.Instance.PushSubLocation(selectedRoom, previousRoom);
    }
}
