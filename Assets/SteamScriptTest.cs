using UnityEngine;
using Steamworks;
using TMPro;
public class SteamScriptTest : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
    protected Callback<UserStatsReceived_t> m_UserStatsReceived;
    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.RequestCurrentStats();
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
            m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(GiveStartAchievement);
        }
    }

    private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
    {
        if (pCallback.m_bActive != 0)
        {
            Debug.Log("Steam Overlay has been activated");
        }
        else
        {
            Debug.Log("Steam Overlay has been closed");
        }
    }

    public void GiveStartAchievement(UserStatsReceived_t pCallback)
    {
        if (SteamUserStats.SetAchievement("starting_achievement"))
        {
            SteamUserStats.StoreStats();
        }
    }

    public void GrantTestItems()
    {
        SteamItemDef_t[] newItems = new SteamItemDef_t[1];
        uint[] quantities = new uint[1];

        newItems[0] = (SteamItemDef_t)1;
        quantities[0] = 1;

        SteamInventoryResult_t resultHandle;

        if (SteamInventory.GenerateItems(out resultHandle, newItems, quantities, (uint)newItems.Length))
        {
            Debug.Log("Items generated successfully.");
            // Do something with resultHandle if needed, like fetching the item details.
            SteamInventory.DestroyResult(resultHandle); // Clean up after you're done.
        }
        else
        {
            Debug.LogError("Failed to generate items.");
        }
    }


    void Start()
    {
        if (SteamManager.Initialized)
        {
            textMeshProUGUI.text = SteamFriends.GetPersonaName();
        }
    }
}
