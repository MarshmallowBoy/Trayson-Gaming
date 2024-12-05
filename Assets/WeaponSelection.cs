using UnityEngine;

public class WeaponSelection : MonoBehaviour
{
    public GameObject[] Weapons;

    int _ActiveWeapon;
    public int ActiveWeapon {
        get {
            return _ActiveWeapon;
        }
        set
        {
            ActiveWeaponUpdated(_ActiveWeapon, value);
            _ActiveWeapon = value;
        } 
    }
    void Update()
    {
        UpdateActiveWeapon();
    }
    void ActiveWeaponUpdated(int oldValue, int newValue)
    {
        if(newValue < 0 || newValue > Weapons.Length - 1)
        {
            return;
        }
        foreach (GameObject weapon in Weapons) {
            weapon.SetActive(false);
        }
        Weapons[newValue].SetActive(true);
        Debug.Log(oldValue);
        Debug.Log(newValue);
    }

    void UpdateActiveWeapon()
    {
        if (ActiveWeapon + Mathf.RoundToInt(Input.mouseScrollDelta.y) < 0 || ActiveWeapon + Mathf.RoundToInt(Input.mouseScrollDelta.y) > Weapons.Length - 1 || Mathf.RoundToInt(Input.mouseScrollDelta.y) == 0)
        {
            return;
        }
        ActiveWeapon += Mathf.RoundToInt(Input.mouseScrollDelta.y);

        if (ActiveWeapon <= 0)
        {
            ActiveWeapon = 0;
        }
        if (ActiveWeapon >= Weapons.Length)
        {
            ActiveWeapon = Weapons.Length - 1;
        }
    }

}
