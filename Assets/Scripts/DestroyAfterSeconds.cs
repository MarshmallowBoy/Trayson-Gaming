using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float Seconds;

    private void Start()
    {
        Invoke("DestroyThisObject", Seconds);
    }

    void DestroyThisObject()
    {
        Destroy(gameObject);
    }
}
