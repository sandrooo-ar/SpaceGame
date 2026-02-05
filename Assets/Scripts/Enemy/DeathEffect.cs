using UnityEngine;

public class DeathEffect : MonoBehaviour
{

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayDeathEffect()
    {

        gameObject.transform.SetParent(null);

        if (anim != null)
        {
            anim.Play("DeathAnimation");
        }

        Destroy(gameObject, 1f);

    }

}
