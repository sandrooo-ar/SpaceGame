using UnityEngine;

public class Lightning : MonoBehaviour
{

    [SerializeField] private AreaDamage areaDamage;

    public void TriggerAreaDamage()
    {
        if (areaDamage != null)
        {
            areaDamage.ApplyDamage();
        }
        else
        {
            Debug.LogWarning("AreaDamage component is not assigned.");
        }
    }

}
