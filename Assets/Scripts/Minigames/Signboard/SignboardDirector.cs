using UnityEngine;

public class SignboardDirector : MonoBehaviour
{
    [SerializeField] private GameObject leftRoom;
    [SerializeField] private GameObject rightRoom;

    public void SelectOption(int index)
    {
        bool isRight = index == 0 || index == 2;

        if (leftRoom != null)
            leftRoom.SetActive(isRight == false);

        if (rightRoom != null)
            rightRoom.SetActive(isRight);
    }
}
