using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButtonScript : MonoBehaviour
{
    public int index;

    public void LoadScene()
    {
        if (index == 0)
        {
            SceneManager.LoadScene("911 Scene");
        }
        else if (index == 1)
        {
            SceneManager.LoadScene("updated");
        }
    }
}
