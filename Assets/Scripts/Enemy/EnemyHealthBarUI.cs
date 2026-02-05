using UnityEngine;

public class EnemyHealthBarUI : MonoBehaviour
{

    public Vector3 offset = new Vector3(0, 2f, 0);

    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(transform.parent.position +  offset);
    }
}
