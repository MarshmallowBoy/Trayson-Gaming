using UnityEngine;

public class ActiveOnStart : MonoBehaviour
{
    public bool Enable = true;

    void Start()
    {
        gameObject.SetActive(Enable);
    }
}
