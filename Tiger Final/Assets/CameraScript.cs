using UnityEditor.Callbacks;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player; //Assigned in inspector as the player
    Rigidbody rb;
    public float camDisplacement = 5.0f;
    float tempDisplacement = 0.0f;
    //public float horizontalCameraDisplacement; //horizontal displacement left from player
    //public float verticalCameraDisplacement; //vertical displacement left from player

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = player.GetComponent<Rigidbody>(); //rb is the player's rigidbody
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) {
            //incorporate raycasts to zoom in if wall behind player later
            tempDisplacement = camDisplacement;
            if (Physics.Raycast(rb.position,-rb.linearVelocity, out RaycastHit hitInfo))
            {
                if (hitInfo.distance < camDisplacement)
                {
                    tempDisplacement = hitInfo.distance * 0.9f;
                }
            }

            // Position camera relative to player, then look toward the player's velocity direction.
            // Vector3 vel = rb.linearVelocity;
            // transform.position = rb.position - tempDisplacement * (vel / vel.magnitude);
            // transform.LookAt(rb.transform);
        }
    }
}
