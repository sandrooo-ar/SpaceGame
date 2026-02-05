using UnityEngine;

[CreateAssetMenu(menuName = "Combo/DamageBuff")]
public class DamageImprovement : ComboImprovement
{
    public float extraMultiplier = 0.2f; // 20% increase

    public override void Activate()
    {
        PlayerStats.Instance.DamageMultiplier += extraMultiplier;
    }

    public override void Deactivate()
    {
        PlayerStats.Instance.DamageMultiplier -= extraMultiplier;
    }
}
