using UnityEngine;

public class UIAnimController : MonoBehaviour
{
    public GameObject CharacterMenu;
    public AudioSource cMenuAS;
    public Animator animr;

    public void ToggleCharacterMenu()
    {
        if(cMenuAS ==  null) { cMenuAS = GameObject.Find("CharacterMenu").GetComponent<AudioSource>(); }
        CharacterMenu.SetActive(!CharacterMenu.activeInHierarchy);
        if(CharacterMenu.activeInHierarchy == true) { cMenuAS.Play(); } else { cMenuAS.Stop(); }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CharacterMenu.activeInHierarchy == true)
        {
            if(animr ==  null) { animr = GameObject.Find("AnimationAndFades").GetComponent<Animator>(); }
            animr.SetTrigger("Exit");
        }
        else { return; }
    }
}