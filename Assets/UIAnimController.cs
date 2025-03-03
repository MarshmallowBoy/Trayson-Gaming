using UnityEngine;

public class UIAnimController : MonoBehaviour
{
    public GameObject CharacterMenu;
    public AudioSource cMenuAS;
    public Animator animr;
    private float animTimer;

    public void ToggleCharacterMenu()
    {
        CharacterMenu.SetActive(!CharacterMenu.activeInHierarchy);
        if(cMenuAS ==  null) { cMenuAS = GameObject.Find("CharacterMenu").GetComponent<AudioSource>(); }
        if(CharacterMenu.activeInHierarchy == true) { cMenuAS.Play(); } else { cMenuAS.Stop(); }
    }

    private void Update()
    {
        animTimer += Time.deltaTime;
        if(CharacterMenu.activeInHierarchy == false) { return; }
        else if(CharacterMenu.activeInHierarchy == true)
        {
            if(animr ==  null) { animr = GameObject.Find("AnimationAndFades").GetComponent<Animator>(); }
            if(Input.GetKeyDown(KeyCode.Escape) && animTimer >= 5f) 
            {
                animTimer = 0;
                animr.SetTrigger("Exit");
            }  
        }
    }
}