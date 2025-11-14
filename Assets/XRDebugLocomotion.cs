using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class XRDebugLocomotion : MonoBehaviour
{
    public float moveSpeed = 2f;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // WASD / Arrow keys
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0f, v);

        if (input.sqrMagnitude > 0.0001f)
        {
            // move relative to where the rig is facing
            Vector3 move = transform.TransformDirection(input);
            controller.Move(move * moveSpeed * Time.deltaTime);
        }
    }
}
