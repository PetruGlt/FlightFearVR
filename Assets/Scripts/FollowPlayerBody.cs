using UnityEngine;

public class FollowPlayerBody : MonoBehaviour
{
    [Header("Referinte")]
    [SerializeField] private Transform cameraTransform; // Trage aici Main Camera din XR Origin

    [Header("Setari Pozitionare")]
    [SerializeField] private float distance = 0.6f;      // Distanta in fata (metri)
    [SerializeField] private float heightOffset = -0.4f; // Cat de jos sa fie fata de ochi
    [SerializeField] private float followSpeed = 5.0f;   // Viteza de urmarire (smoothness)

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // 1. Calculam pozitia tinta (in fata camerei, dar la o inaltime fixa)
        Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * distance);
        targetPosition.y = cameraTransform.position.y + heightOffset;

        // 2. Aplicam miscare fluida (Lerp) pentru a evita tremuratul
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // 3. Rotim toolbar-ul sa priveasca mereu spre jucator
        // Calculam directia catre camera, dar ignoram axa Y pentru a nu se inclina
        Vector3 lookDirection = cameraTransform.position - transform.position;
        lookDirection.y = 0; 
        
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);
        }
    }
}