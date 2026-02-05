using UnityEngine;

[CreateAssetMenu(fileName = "BounceBehavior", menuName = "Abilities/SpecialUpgrades/BounceBehaviour")]
public class BounceBehavior : UpgradeBehavior
{

    public int bounceCount = 1;

    public GameObject effect;

    public override void Apply(Ability ability)
    {
        if (ability is AimshotAbility aimshot)
        {

            GameObject projectile = ObjectPooler.Instance.GetPrefabByTag("BaseProjectile");
            
            projectile.GetComponent<AbilityProjectile>().canBounce = true;
            projectile.GetComponent<AbilityProjectile>().remainingBounces = bounceCount;

            if (effect != null)
            {
                Instantiate(effect, projectile.transform);
                effect.transform.localPosition = Vector3.zero;
            }
        }
    }
}
