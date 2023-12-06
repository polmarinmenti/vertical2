using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayLvl1()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
