using UnityEngine;

public class PreviewBallScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float timeAlive = 0.0f; //how long the preview ball has existed
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > 0.75f)
        {
            Destroy(gameObject);
        }
    }
}
