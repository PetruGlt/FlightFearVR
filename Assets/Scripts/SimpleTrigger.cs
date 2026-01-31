using UnityEngine;
using UnityEngine.Events;

public class SimpleTrigger : MonoBehaviour
{
    // Aici poti trage functii in Inspector, exact ca la un Buton UI
    public UnityEvent<Collider> onEnter;

    void OnTriggerEnter(Collider other)
    {
        // Ignoram coliziunile care nu ne intereseaza
        if (other.isTrigger) return; 
        
        onEnter.Invoke(other);
    }
}