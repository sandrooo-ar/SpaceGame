using UnityEngine;

[CreateAssetMenu(menuName = "Combo/AttackSpeedBuff")]
public class AttackSpeedImprovement : ComboImprovement
{
    public float extraMultiplier = 0.2f; // 20% increase

    public override void Activate()
    {
        PlayerStats.Instance.AttackSpeedMultiplier += extraMultiplier;
    }

    public override void Deactivate()
    {
        PlayerStats.Instance.AttackSpeedMultiplier -= extraMultiplier;
    }
}