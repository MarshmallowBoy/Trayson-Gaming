using UnityEngine;

public class UIAnimController : MonoBehaviour
{
    public GameObject CharacterMenu;
    public AudioSource cMenuAS;
    public Animator animr;

    public void EnableCharacterMenu()
    {
        CharacterMenu.SetActive(true);
        if(cMenuAS ==  null) { GameObject.Find("CharacterMenu").GetComponent<AudioSource>(); cMenuAS.Play(); }
        else { cMenuAS.Play(); }
    }

    public void DisableCharacterMenu()
    {
        if (cMenuAS == null) { GameObject.Find("CharacterMenu").GetComponent<AudioSource>(); cMenuAS.Stop(); }
        else { cMenuAS.Stop(); }
        CharacterMenu.SetActive(false);
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