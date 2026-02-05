using UnityEngine;

[CreateAssetMenu(fileName = "MultipleExecute", menuName = "Abilities/SpecialUpgrades/MultipleExecute")]
public class MultipleExecute : UpgradeBehavior
{

    public int executeCount = 2;

    public override void Apply(Ability ability)
    {
        if (ability is AreaDamageAbility aimshot)
        {
            aimshot.executionCount = executeCount;
        }
    }
}
