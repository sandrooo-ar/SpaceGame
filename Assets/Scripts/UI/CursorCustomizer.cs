using UnityEngine;

public class CursorCustomizer : MonoBehaviour
{

    public Texture2D cursorTexture;
    public Vector2 hotspot = Vector2.zero; // Default hotspot at the top-left corner
    void Start()
    {
        if (cursorTexture != null)
        {
            // Center the hotspot so it matches where you're aiming
            hotspot = new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f);
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }
        else
        {
            Debug.LogWarning("Cursor texture is not set.");
        }
    }
    void OnDestroy()
    {
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.ForceSoftware);
    }

}
