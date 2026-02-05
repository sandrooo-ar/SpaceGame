using UnityEngine;

public class SkillSelectionPool : MonoBehaviour
{

    public GameObject skillPrefab;

    public Ability[] skills;

    public SkillSelectionManager skillSelectionManager;

    private void Start()
    {
        
        foreach (var skill in skills)
        {
            var skillObj = Instantiate(skillPrefab, transform);
            var skillComponent = skillObj.GetComponent<SkillSelectionButton>();
            if (skillComponent != null)
            {
                skillComponent.skillSelectionManager = skillSelectionManager;
                skillComponent.SetSkill(skill);
            }
        }

        if (skillSelectionManager != null && skillSelectionManager.selectedSkills.Count > 0)
        {
            skillSelectionManager.selectedSkills[0].OnSkillSelected(skillSelectionManager.selectedSkills[0].currentSkill);
        }

    }

}
