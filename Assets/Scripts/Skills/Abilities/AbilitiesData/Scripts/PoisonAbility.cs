using UnityEngine;

[CreateAssetMenu(fileName = "PoisonAbility", menuName = "Abilities/PoisonAbility")]
public class PoisonAbility : Ability
{

    public int poisonDamageImprovement = 2;
    public int poisonDecayImprovement = 1;
    public float poisonTickIntervalImprovement = 0.2f;

    public PoisonComponent poisonComponentProjectile;

    private void Activate()
    {
        ImprovePoisons();
    }

    public void ImprovePoisons()
    {
        GameObject baseProjectile = ObjectPooler.Instance.GetPrefabByTag("BaseProjectile");

        if (baseProjectile != null)
        {
            if (baseProjectile.GetComponent<PoisonComponent>() != null)
            {
                // Improve existing poison components
                foreach (PoisonComponent existingPoison in baseProjectile.GetComponents<PoisonComponent>())
                {
                    existingPoison.SetPoisonDamage(existingPoison.GetPoisonDamage() + poisonDamageImprovement);
                    existingPoison.SetPoisonDecay(existingPoison.GetPoisonDecay() - poisonDecayImprovement);
                    existingPoison.SetPoisonTickInterval(existingPoison.GetPoisonTickInterval() - poisonTickIntervalImprovement);
                }

            }

            // Add new poison component if none exist
            else
            {
                PoisonComponent poisonComp = baseProjectile.AddComponent<PoisonComponent>();
                poisonComp.SetPoisonDamage(poisonDamageImprovement);
                poisonComp.SetPoisonDecay(poisonDecayImprovement);
                poisonComp.SetPoisonTickInterval(1);
            }
                
        }

    }

}
