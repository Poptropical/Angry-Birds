using Unity.VisualScripting;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    Rigidbody rb;
    // Inspector-assignable player GameObject; PlayerLaunchScript is obtained from it at runtime
    public GameObject player;
    public float breakVelocity = 2.0f; //velocity difference required to break block
    public int scoreValue = 50; //score value of block when broken
    public float slowdownMagnitude = 0.5f; //magnitude of velocity reduction on player collision
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // If player not assigned in inspector, try to auto-find it by tag or name.
        if (player == null)
        {
            GameObject found = GameObject.FindWithTag("Player");
            if (found == null)
            {
                found = GameObject.Find("Player");
            }
            if (found != null)
            {
                player = found;
                Debug.Log("BlockScript: auto-assigned player GameObject '" + player.name + "'", this);
            }
            else
            {
                Debug.LogWarning("BlockScript: no GameObject with tag 'Player' or name 'Player' found in scene. Please assign 'player' in Inspector.", this);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Ensure the method has the correct signature so Unity invokes it.
    void OnCollisionEnter(Collision collision)
    {
        // Safely get PlayerLaunchScript from the assigned player or from the colliding object
        PlayerLaunchScript targetPlayer = null;
        if (player != null)
        {
            targetPlayer = player.GetComponent<PlayerLaunchScript>();
        }
        if (targetPlayer == null)
        {
            targetPlayer = collision.gameObject.GetComponentInParent<PlayerLaunchScript>();
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Cube hit by player!");
            if (targetPlayer != null)
            {
                targetPlayer.PlayCollisionSFX();
            }
            else
            {
                Debug.LogWarning("BlockScript: PlayCollisionSFX target not found.", this);
            }

            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRb != null && rb != null)
            {
                if ((otherRb.linearVelocity - rb.linearVelocity).magnitude > breakVelocity)
                {
                    Destroy(gameObject);
                    otherRb.linearVelocity *= slowdownMagnitude;
                    if (targetPlayer != null)
                    {
                        targetPlayer.playerScore += scoreValue;
                    }
                    else
                    {
                        Debug.Log("BlockScript: no PlayerLaunchScript found to award score (Player branch).", this);
                    }
                }
            }
            else
            {
                Debug.LogWarning("BlockScript: missing Rigidbody on collision or block when evaluating break condition.", this);
            }
        }
        if (rb != null && rb.linearVelocity.magnitude > breakVelocity)
        {
            Destroy(gameObject);
            // Increase player score when block is destroyed
            if (targetPlayer != null)
            {
                targetPlayer.playerScore += scoreValue;
            }
            else
            {
                PlayerLaunchScript fallback = collision.gameObject.GetComponentInParent<PlayerLaunchScript>();
                if (fallback != null)
                {
                    fallback.playerScore += scoreValue;
                }
                else
                {
                    Debug.Log("BlockScript: no PlayerLaunchScript found to award score (Block-moving branch).", this);
                }
            }
        }
    }
}
