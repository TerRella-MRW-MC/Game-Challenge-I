using UnityEngine;

public class PlayerMouseLook : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask groundMask;

    private Camera playerCam;

    void Start()
    {
        playerCam = Camera.main; // Used to cache the main camera for optimization. No cache = search for camera every update.
    }

    // Update is called once per frame
    void Update()
    {

        Ray cameraRay = playerCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cameraRay, out RaycastHit hit, 100f, groundMask)) {
            Vector3 target = hit.point;
            Vector3 direction = target - transform.position; // 
            direction.y = 0f; // removes the vertical (Y-axis) difference between the player and the mouse target position to avoid player looking up and down.

            if (direction.sqrMagnitude > 0.01f) {
                Quaternion playerRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, playerRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
