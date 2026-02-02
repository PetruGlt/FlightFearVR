using UnityEngine;
using UnityEngine.Events;

public class SimpleTrigger : MonoBehaviour
{
    // functii in Inspector
    public UnityEvent<Collider> onEnter;

    void OnTriggerEnter(Collider other)
    {
        // Ignoram coliziunile care nu ne intereseaza
        if (other.isTrigger) return; 
        
        onEnter.Invoke(other);
    }
}