using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string titreSceneGame;
    public string titreSceneCredit;
    public string titreSceneSettings;


    public void LoadSettingsScene()
    {
        SceneManager.LoadScene(titreSceneSettings);
    }
    public void LoadCreditsScene()
    {
        SceneManager.LoadScene(titreSceneCredit);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(titreSceneGame);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
