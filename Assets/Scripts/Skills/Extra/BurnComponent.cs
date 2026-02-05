using UnityEngine;

public class BurnComponent : MonoBehaviour
{
    
    [Header("Burn Effect")]
    [SerializeField] private int burnDamage = 5;
    [SerializeField] private float burnTickInterval = 1f;
    [SerializeField] private float burnDuration = 5f;

    private void Start()
    {
        GetComponent<AbilityProjectile>().OnDamageDealt += ApplyBurnEffect;
    }

    private void ApplyBurnEffect(GameObject enemy)
    {
        BurnEffect burn = enemy.AddComponent<BurnEffect>();
        burn.ApplyBurn(burnDamage, burnTickInterval, burnDuration);
    }

}
