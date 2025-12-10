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
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Cube hit by player!");
            if ((collision.gameObject.GetComponent<Rigidbody>().linearVelocity - rb.linearVelocity).magnitude > breakVelocity)
            {
                Destroy(gameObject);
                collision.gameObject.GetComponent<Rigidbody>().linearVelocity *= slowdownMagnitude;
                // Increase player score when block is destroyed
                PlayerLaunchScript targetPlayer = null;
                if (player != null)
                {
                    targetPlayer = player.GetComponent<PlayerLaunchScript>();
                }
                if (targetPlayer == null)
                {
                    targetPlayer = collision.gameObject.GetComponentInParent<PlayerLaunchScript>();
                }
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
        if (rb.linearVelocity.magnitude > breakVelocity)
        {
            Destroy(gameObject);
            // Increase player score when block is destroyed
            PlayerLaunchScript targetPlayer = null;
            if (player != null)
            {
                targetPlayer = player.GetComponent<PlayerLaunchScript>();
            }
            if (targetPlayer == null)
            {
                targetPlayer = collision.gameObject.GetComponentInParent<PlayerLaunchScript>();
            }
            if (targetPlayer != null)
            {
                targetPlayer.playerScore += scoreValue;
            }
            else
            {
                Debug.Log("BlockScript: no PlayerLaunchScript found to award score (Block-moving branch).", this);
            }
        }
    }
}
