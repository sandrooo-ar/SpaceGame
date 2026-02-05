using UnityEngine;
using UnityEngine.UI;
using TMPro;    

public class UIAbilityCooldown : MonoBehaviour
{

    [SerializeField] private Image cooldownImage;
    [SerializeField] private Image backgroundCooldownImage;
    [SerializeField] private TextMeshProUGUI cooldownText;

    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private GameObject noMana;

    public void SetManaIcon(bool hasNoMana)
    {
        if (noMana != null)
        {
            noMana.SetActive(hasNoMana);
        }
    }

    public void SetManaCost(int manaCost)
    {
        if (manaText != null)
        {
            if (manaCost > 0)
            {
                manaText.gameObject.SetActive(true);
                manaText.text = manaCost.ToString();
            }
            else
            {
                manaText.gameObject.SetActive(false);
            }
        }
    }

    public void SetAbilityImage(Sprite background)
    {
        if (backgroundCooldownImage != null)
        {
            backgroundCooldownImage.sprite = background;
        }

        if (cooldownImage != null)
        {
            cooldownImage.sprite = background;
        }
    }

    public void SetTransparency(float alpha)
    {
        if (cooldownImage != null)
        {
            Color color = cooldownImage.color;
            color.a = Mathf.Clamp01(alpha);
            cooldownImage.color = color;
        }
    }

    public void SetCooldown(float timeLeft, float totalTime, bool isFilling)
    {

        if (cooldownText != null)
        {
            if (timeLeft > 0f)
                cooldownText.gameObject.SetActive(true);
            else
                cooldownText.gameObject.SetActive(false);

            cooldownText.text = Mathf.Ceil(timeLeft).ToString();
        } 

        if (cooldownImage != null)
        {
            if (isFilling)
            {
                cooldownImage.fillAmount = 1f - (timeLeft / totalTime); // Va de 0 a 1
            }
            else
            {
                cooldownImage.fillAmount = timeLeft / totalTime; // Va de 1 a 0
            }
        }
    }


}
