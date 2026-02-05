using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillSelectionManager : MonoBehaviour
{

    public List<SkillSelectionButton> selectedSkills = new List<SkillSelectionButton>();

    [SerializeField] private Image skillImage;
    [SerializeField] private SkillSlot skillSlot;   

    public void DeselectSkills()
    {
        foreach (var skillButton in selectedSkills)
        {
            skillButton.Deselect();
        }
    }

    public void ChangeSkill(Ability ability)
    {
        DeselectSkills();

        // Update the ability in the AbilitiesData singleton
        switch (skillSlot)
        {
            case SkillSlot.SkillOne:
                AbilitiesData.Instance.abilityOne = ability;
                break;
            case SkillSlot.SkillTwo:
                AbilitiesData.Instance.abilityTwo = ability;
                break;
            case SkillSlot.SkillThree:
                AbilitiesData.Instance.abilityThree = ability;
                break;
            case SkillSlot.SkillFour:
                AbilitiesData.Instance.abilityFour = ability;
                break;
        }

        if (skillImage != null)
            skillImage.sprite = ability.icon;
    }

}

public enum SkillSlot
{
    SkillOne,
    SkillTwo,
    SkillThree,
    SkillFour
}