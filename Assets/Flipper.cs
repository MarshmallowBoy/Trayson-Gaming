using UnityEngine;

public class Flipper : MonoBehaviour
{
    public Animator anim;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("Slap");
        }
    }
}
