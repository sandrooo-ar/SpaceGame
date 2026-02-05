using UnityEngine;

[CreateAssetMenu(fileName = "PierceBehaviour", menuName = "Abilities/SpecialUpgrades/PierceBehaviour")]
public class PierceBehaviour : UpgradeBehavior
{

    public GameObject effect;

    public override void Apply(Ability ability)
    {
        if (ability is AimshotAbility aimshot)
        {
            GameObject projectile = ObjectPooler.Instance.GetPrefabByTag("BaseProjectile");

            projectile.GetComponent<AbilityProjectile>().passEnemies = true;

            if (effect != null)
            {
                Instantiate(effect, projectile.transform);
                effect.transform.localPosition = Vector3.zero;
            }

        }
    }
}
