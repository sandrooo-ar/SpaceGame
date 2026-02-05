using NUnit.Framework;
using UnityEngine;

public class AbilitiesData : MonoBehaviour
{

    public static AbilitiesData Instance;

    public Ability abilityOne;
    public Ability abilityTwo;
    public Ability abilityThree;
    public Ability abilityFour;

    private void Awake()
    {
        // Implementing Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist through scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    public void AssignAbilities()
    {
        GameObject player = GameObject.FindWithTag("Player");

        // Ensure the player GameObject and AbilityHolder components exist
        if (player != null)
        {
            AbilityHolder[] abilityHolders = player.GetComponents<AbilityHolder>();

            if (abilityHolders.Length >= 4)
            {
                abilityHolders[0].ability = abilityOne;
                abilityHolders[1].ability = abilityTwo;
                abilityHolders[2].ability = abilityThree;
                abilityHolders[3].ability = abilityFour;
            }
            else
            {
                Debug.LogError("Not enough AbilityHolder components found on the Player GameObject.");
            }

        }
        else
        {
            Debug.LogError("Player GameObject not found.");
        }
    }

}
