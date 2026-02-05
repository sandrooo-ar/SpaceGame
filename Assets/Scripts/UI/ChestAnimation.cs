using UnityEngine;

public class ChestAnimation : MonoBehaviour
{

    public Chest chest;

    public void EmitHoverSound()
    {
        chest.StartHover();
    }

    public void StopHoverSound()
    {
        chest.EndHover();
    }

}
