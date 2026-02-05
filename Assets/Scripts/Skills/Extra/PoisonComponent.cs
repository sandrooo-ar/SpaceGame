using UnityEngine;

public class PoisonComponent : MonoBehaviour
{

    [Header("Poison Effect")]
    [SerializeField] private int poisonDamage = 5;
    [SerializeField] private int poisonDecay = 1;
    [SerializeField] private float poisonTickInterval = 1f;
 

    public void SetPoisonDamage(int damage)
    {
        poisonDamage = damage;

        if (poisonDamage < 1)
            poisonDamage = 1;
    }

    public int GetPoisonDamage()
    {
        return poisonDamage;
    }

    public void SetPoisonDecay(int decay)
    {
        poisonDecay = decay;
        if (poisonDecay < 1)
            poisonDecay = 1;
    }

    public int GetPoisonDecay()
    {
        return poisonDecay;
    }

    public void SetPoisonTickInterval(float interval)
    {
        poisonTickInterval = interval;
        if (poisonTickInterval < 0.2f)
            poisonTickInterval = 0.2f;
    }

    public float GetPoisonTickInterval()
    {
        return poisonTickInterval;
    }

    private void Start()
    {
        GetComponent<AbilityProjectile>().OnDamageDealt += ApplyPoisonEffect;
    }

    private void ApplyPoisonEffect(GameObject enemy)
    {
        PoisonEffect poison = enemy.AddComponent<PoisonEffect>();
        poison.ApplyPoison(poisonDamage, poisonDecay, poisonTickInterval);
    }

}
