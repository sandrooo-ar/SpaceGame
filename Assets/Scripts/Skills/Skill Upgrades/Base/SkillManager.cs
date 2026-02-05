using UnityEngine;
using UnityEngine.InputSystem;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    public AudioSource skillPointSound;

    public int skillPoints = 0;
    public AbilityHolder[] abilityHolders;

    public GameObject[] upgradeButtons;
    
    public SkillLevelUI[] skillLevelUIs;

    private Ability[] abilities;

    private bool canLevelUpOne = false;
    private bool canLevelUpTwo = false;
    private bool canLevelUpThree = false;
    private bool canLevelUpFour = false;

    public InputActionReference levelUpOne;
    public InputActionReference levelUpTwo;
    public InputActionReference levelUpThree;
    public InputActionReference levelUpFour;

    private void OnEnable()
    {

        if (levelUpOne != null)
        {
            levelUpOne.action.Enable();
            levelUpOne.action.performed += ctx =>
            {
                if (canLevelUpOne)
                {
                    UpgradeAbility(0);
                }
            };
        }

        if (levelUpTwo != null)
        {
            levelUpTwo.action.Enable();
            levelUpTwo.action.performed += ctx =>
            {
                if (canLevelUpTwo)
                {
                    UpgradeAbility(1);
                }
            };
        }

        if (levelUpThree != null)
        {
            levelUpThree.action.Enable();
            levelUpThree.action.performed += ctx =>
            {
                if (canLevelUpThree)
                {
                    UpgradeAbility(2);
                }
            };

        }

        if (levelUpFour != null)
        {
            levelUpFour.action.Enable();
            levelUpFour.action.performed += ctx =>
            {
                if (canLevelUpFour)
                {
                    UpgradeAbility(3);
                }
            };
        }

    }

    private void OnDisable()
    {

        if (levelUpOne != null)
            levelUpOne.action.Disable();
        if (levelUpTwo != null)
            levelUpTwo.action.Disable();
        if (levelUpThree != null)
            levelUpThree.action.Disable();
        if (levelUpFour != null)
            levelUpFour.action.Disable();

    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (abilityHolders != null && abilityHolders.Length > 0)
        {
            abilities = new Ability[abilityHolders.Length];
            for (int i = 0; i < abilityHolders.Length; i++)
            {
                if (abilityHolders[i] != null)
                    abilities[i] = abilityHolders[i].ability;
            }
        }
    }

    private void EnsureInitialized()
    {
        if (abilities == null || abilities.Length == 0)
        {
            if (abilityHolders != null && abilityHolders.Length > 0)
            {
                abilities = new Ability[abilityHolders.Length];
                for (int i = 0; i < abilityHolders.Length; i++)
                {
                    if (abilityHolders[i] != null)
                        abilities[i] = abilityHolders[i].ability;
                }
            }
        }
    }

    public void ShowLevelUp()
    {
        // Automatically upgrade first ability on level 1
        if (LevelManager.Instance.currentLevel == 1)
        {
            UpgradeAbility(0);
            return;
        }

        // Automatically upgrade ultimate at levels 6, 12, and 18
        if (LevelManager.Instance.currentLevel == 6 ||
            LevelManager.Instance.currentLevel == 12 ||
            LevelManager.Instance.currentLevel == 18)
        {
            UpgradeAbility(3); // Ultimate index
            return; // Skip showing other UI since this auto-level consumes level-up
        }

        // Show upgrade buttons for other abilities only
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            // Skip showing the ultimate (index 3) manually
            if (i == 3)
            {
                upgradeButtons[i].SetActive(false);
                canLevelUpFour = false;
                continue;
            }

            if (abilities[i] != null && abilities[i].currentLevel < abilities[i].maxLevel)
            {
                upgradeButtons[i].SetActive(true);

                switch (i)
                {
                    case 0: canLevelUpOne = true; break;
                    case 1: canLevelUpTwo = true; break;
                    case 2: canLevelUpThree = true; break;
                }
            }
            else
            {
                upgradeButtons[i].SetActive(false);

                switch (i)
                {
                    case 0: canLevelUpOne = false; break;
                    case 1: canLevelUpTwo = false; break;
                    case 2: canLevelUpThree = false; break;
                }
            }
        }
    }


    public void AddSkillPoint()
    {
        skillPoints++;
    }

    public void UpgradeAbility(int index)
    {
        EnsureInitialized();

        Ability ability = abilities[index];

        if (ability.currentLevel < ability.maxLevel)
        {

            if (skillPointSound != null)
                skillPointSound.Play();


            ability.LevelUp();
            skillPoints--;
            skillLevelUIs[index].SetLevel(ability.currentLevel);
            HideLevelUp();
            LevelManager.Instance.NextLevel();
        }
    }

    public void HideLevelUp()
    {
        foreach (var button in upgradeButtons)
        {
            button.SetActive(false);

            canLevelUpOne = false;
            canLevelUpTwo = false;
            canLevelUpThree = false;
            canLevelUpFour = false;
        }
    }
}
