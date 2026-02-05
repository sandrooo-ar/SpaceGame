using UnityEngine;

[CreateAssetMenu(menuName = "Combo/GoldBuff")]
public class GoldImprovement : ComboImprovement
{

    public int extraCoinGain = 1;

    public override void Activate()
    {
        CoinManager.Instance.extraCoinGain += extraCoinGain;
    }

    public override void Deactivate()
    {
        CoinManager.Instance.extraCoinGain -= extraCoinGain;
    }

}
