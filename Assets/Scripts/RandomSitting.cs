using UnityEngine;

public class RandomSitting : MonoBehaviour
{
    [Header("Configurare")]
    public int numberOfAnimations = 4;
    public string parameterName = "SitStyle"; 

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        int randomStyle = Random.Range(0, numberOfAnimations);

        if (anim != null)
        {
            anim.SetInteger(parameterName, randomStyle);
            
            anim.Play(0, -1, Random.value); 
        }
    }
}