using UnityEngine;
using UnityEngine.UI;

public class SkillLevelUI : MonoBehaviour
{

    public Color activeColor;

    public Image[] levels;

    public void SetLevel(int level)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (i < level)
            {
                levels[i].color = activeColor;
            }
            else
            {
                levels[i].color = Color.gray;
            }
        }
    }

}
