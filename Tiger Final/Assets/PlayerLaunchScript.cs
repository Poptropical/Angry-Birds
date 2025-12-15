using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerLaunchScript : MonoBehaviour
{
    //Velocity player is launched with
    float launchVel = 0; //baseline velocity that increases the longer space is held
    float launchForce = 25.0f; //how much launch velocity increases by a second as space is held down
    float maxLaunchVel = 30.0f; //maximum launch velocity
    float shotPreviewDelay = 0.005f;
    bool canLaunch = true; //whether the player can launch or not
    public Vector3 launchDirection; //vector toward which the player initially flies
    public GameObject ball; //the actual ball with the collider
    public Slider powerSlider; //Slider on left of screen that shows launch power
    Rigidbody rb; //assigned in Start() as the rigidbody for the player
    public GameObject cam; //the main camera
    public GameObject shotPreview; //the shot preview object
    public GameObject canvas; //the UI canvas
    public TMP_Text scoreText; //the UI text that shows the player's score
    public float playerScore = 0; //the player's score based on blocks hit
    float rotationSpeed = 50f; // degrees per second
    public float scoreNeeded = 500f; //score needed to win
    public int shotCount = 3; //number of shots left
    public GameObject plane; //the plane that follows the ball around
    public GameObject sfx1; //audio player for launch SFX
    public GameObject sfx2; //audio player for collision SFX
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = GameObject.Find("Plane");
        sfx1 = GameObject.Find("SFX1");
        sfx2 = GameObject.Find("SFX2");
        //Sets rb to the player's rigidbody
        if (ball != null)
        {
            rb = ball.GetComponent<Rigidbody>();
            scoreText = canvas.GetComponentInChildren<TMP_Text>();
            launchDirection.Normalize();
            transform.rotation = Quaternion.LookRotation(launchDirection);
            cam.transform.position = transform.position + launchDirection * -13 + new Vector3(0, 7, 0);
            cam.transform.LookAt(transform);
        }
    }



    // Update is called once per frame
    void Update()
    {
        Debug.Log("Running");
        // Reload active scene when player presses R
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Orients player toward its velocity (use position + velocity as a target).
        // LookAt expects a world-space position; passing the velocity vector alone
        // makes the object look at that point in world-space (often behind the player).
        if (rb != null)
        {
            Vector3 vel = rb.linearVelocity;
            if (vel.sqrMagnitude > 0.000001f)
            {
                rb.transform.LookAt(rb.position + vel);
            }
        }
        Debug.Log("Running2");
        //If space is held, the launch becomes stronger based on launchforce
        if (Input.GetKey(KeyCode.Space) && canLaunch && shotCount > 0 && playerScore < scoreNeeded)
        {
            launchVel += launchForce * Time.deltaTime;
        }
        Debug.Log("Running3");
        //If space is released, velocity is targeted toward the launch direction with the final launch force
        if (Input.GetKeyUp(KeyCode.Space))
        {
            PlayLaunchSFX();
            if (launchDirection != null && canLaunch && shotCount > 0)
            {
                canLaunch = false; //prevents midair launches
                if (launchVel > maxLaunchVel)
                {
                    launchVel = maxLaunchVel;
                }
                rb.linearVelocity = launchDirection * launchVel;
                canLaunch = false;
                shotCount -= 1;
            }
            //Resets launch velocity to 0 bc next shot needs to charge up from 0 velocity
            launchVel = 0;
        }
        Debug.Log(launchDirection.magnitude);
        if (Input.GetKey(KeyCode.A) && launchDirection != null && canLaunch && playerScore < scoreNeeded)
        {
            // Rotates launchDirection to the left, proportional to time held
            launchDirection = Quaternion.Euler(0, -rotationSpeed * Time.deltaTime, 0) * launchDirection;
            launchDirection.Normalize();
        }
        Debug.Log("Running5");
        if (Input.GetKey(KeyCode.D) && launchDirection != null && canLaunch && playerScore < scoreNeeded)
        {
            // Rotates launchDirection to the right, proportional to time held
            launchDirection = Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0) * launchDirection;
            launchDirection.Normalize();
        }


        Debug.Log("Running6");
        //Updates power slider to reflect current launch velocity
        if (powerSlider != null)
        {
            powerSlider.value = launchVel / maxLaunchVel;
        }
        Debug.Log("Running7");
        rb.transform.rotation = Quaternion.Euler(launchDirection);

        if (cam != null)
        {
            //do things to camera based on velocity
            transform.rotation = Quaternion.LookRotation(launchDirection);
            cam.transform.position = transform.position + launchDirection * -13 + new Vector3(0, 7, 0);
            cam.transform.LookAt(transform);
        }
        Debug.Log("Running8");
        shotPreviewDelay -= Time.deltaTime;
        if (shotPreviewDelay <= 0 && rb.linearVelocity.magnitude < 0.1f && launchDirection != null)
        {
            GameObject tempshot = Instantiate(shotPreview, rb.position, Quaternion.identity);
            if (launchVel > 0.01f) 
            {
                tempshot.GetComponent<Rigidbody>().linearVelocity = launchDirection * maxLaunchVel / 15f;
            }
            shotPreviewDelay = 0.001f;
        }

        Debug.Log("Running9");
        if (scoreText != null)
        {
            scoreText.text = "Score: " + playerScore + "\nScore Needed: " + scoreNeeded + "\nShots Left: " + shotCount;
        }
        Debug.Log("running10");

        //Resets player when it comes to a stop
        if (!canLaunch && rb.linearVelocity.magnitude < 0.1f)
        {
            launchDirection = new Vector3(100, 75, 0);
            canLaunch = true;
            rb.transform.position = Vector3.zero - transform.up * 0.5f; //slightly lowered to keep ball from bouncing on respawn
            rb.linearVelocity = Vector3.zero;
            
            launchDirection.Normalize();
            transform.rotation = Quaternion.LookRotation(launchDirection);
            cam.transform.position = transform.position + launchDirection * -13 + new Vector3(0, 7, 0);
            cam.transform.LookAt(transform);
        }

        //sets plane sprite pos and rotation
        plane.transform.position = new Vector3(rb.position.x, rb.position.y - 0.5f, rb.position.z);
        //sets rotation to look in launch direction but 20 degrees downward
        plane.transform.rotation = Quaternion.LookRotation(launchDirection) * Quaternion.Euler(30, 0, 0);


        Debug.Log("running11");
        if (playerScore >= scoreNeeded)
        {
            //You win
            if (scoreText != null)
            {
                scoreText.text = "You Win!\nFinal Score: " + playerScore;
            }
        }

        Debug.Log("running12");
        if (shotCount <= 0 && rb.linearVelocity.magnitude < 0.1f && playerScore < scoreNeeded)
        {
            //You lose
            if (scoreText != null)
            {
                scoreText.text = "You Lose!\nFinal Score: " + playerScore;
            }
        }

        Debug.Log("running13");
        // Reload active scene when player presses R
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void PlayLaunchSFX()
    {
        if (sfx1 != null)
        {
            AudioSource audioSource = sfx1.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }

    public void PlayCollisionSFX()
    {
        if (sfx2 != null)
        {
            AudioSource audioSource = sfx2.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}
