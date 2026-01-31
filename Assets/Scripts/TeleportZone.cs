using UnityEngine;
using System.Collections; // Avem nevoie pentru Coroutines (asteptare)

public class TeleportZone : MonoBehaviour
{
    [Header("Unde mergem?")]
    public Transform destinationPoint; // Trage aici obiectul gol destinatie

    [Header("Setari")]
    public bool keepRotation = false; // Daca vrei sa pastrezi rotatia jucatorului sau sa il rotesti cum e destinatia

    private void OnTriggerEnter(Collider other)
    {
        // 1. Verificam daca a intrat Jucatorul
        // Cautam tag-ul "Player" SAU daca obiectul are un CharacterController
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null || other.name.Contains("XR Origin"))
        {
            // Gasim radacina (XR Origin), nu doar mana sau capul
            Transform playerRoot = other.transform.root;

            // Pornim secventa de teleportare
            StartCoroutine(TeleportPlayer(playerRoot));
        }
    }

    IEnumerator TeleportPlayer(Transform player)
    {
        // 2. Daca avem CharacterController, IL OPRIM TEMPORAR
        // Acesta este pasul critic! Fara el, teleportarea nu merge.
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // 3. Mutam jucatorul
        player.position = destinationPoint.position;

        // (Optional) Rotim jucatorul sa fie cu fata unde vrem noi
        if (!keepRotation)
        {
            player.rotation = destinationPoint.rotation;
        }

        // Asteptam un cadru (o fractiune de secunda) ca Unity sa proceseze mutarea
        yield return null; 

        // 4. Pornim la loc CharacterController
        if (cc != null) cc.enabled = true;

        Debug.Log("Teleportare reusita la: " + destinationPoint.name);
    }
}