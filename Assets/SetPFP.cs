using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SetPFP : MonoBehaviour
{
    public RawImage tex;
    void Start()
    {
        var avatarHandle = SteamFriends.GetMediumFriendAvatar(SteamUser.GetSteamID());

        uint width, height;

        SteamUtils.GetImageSize(avatarHandle, out width, out height);
        byte[] imageData = new byte[width * height * 4];
        SteamUtils.GetImageRGBA(avatarHandle, imageData, imageData.Length);
        Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
        texture.LoadRawTextureData(imageData);
        texture.Apply();
        tex.texture = texture;
    }
}
