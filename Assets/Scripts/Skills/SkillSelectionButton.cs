using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionButton : MonoBehaviour
{

    public Ability currentSkill;

    public SkillSelectionManager skillSelectionManager;

    [SerializeField] private Image skillIcon;
    [SerializeField] private Image skillSelected;
    [SerializeField] private Button skillButton;

    // Manages the skill selection button in the skill selection pool
    public void SetSkill(Ability skill)
    {
        skillIcon.sprite = skill.icon;
        currentSkill = skill;

        // Add a listener to the button to handle skill selection
        if (skillButton != null)
        {
            skillButton.onClick.AddListener(() => OnSkillSelected(skill));
        }

        // Adds this skill to the available skills in the manager
        skillSelectionManager.selectedSkills.Add(this);
    }

    // Handles skill selection logic
    public void OnSkillSelected(Ability skill)
    {
        skillSelectionManager.ChangeSkill(skill);
        skillSelected.enabled = true;
    }

    // Deselects the skill in the UI
    public void Deselect()
    {
        skillSelected.enabled = false;
    }

}
