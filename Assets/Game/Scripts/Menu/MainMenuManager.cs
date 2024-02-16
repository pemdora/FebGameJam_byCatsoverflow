using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject _mainMenuScreen;
    [SerializeField] private GameObject _creditScreen;
    [SerializeField] private GameObject _settingsScreen;
    [SerializeField] private GameObject _exitScreen;

    void OnEnable()
    {
        _mainMenuScreen.SetActive(true);
        _creditScreen.SetActive(false);
        _settingsScreen.SetActive(false);
        _exitScreen.SetActive(false);
    }

    #region CreditScreen

    public void ShowCreditScreen(bool visible)
    {
        _mainMenuScreen.SetActive(!visible);
        _creditScreen.SetActive(visible);
    }

    #endregion

    #region SettingsScreen

    public void ShowSettingsScreen(bool visible)
    {
        _mainMenuScreen.SetActive(!visible);
        _settingsScreen.SetActive(visible);
    }

    #endregion

    #region ExitScreen
    public void QuitGame()
    {
        _mainMenuScreen.SetActive(false);
        _exitScreen.SetActive(true);
    }

    public void OnExitConfirmation(bool confirmed)
    {
        switch (confirmed)
        {
            case true:
                Application.Quit();
                break;
            case false:
                _mainMenuScreen.SetActive(true);
                _exitScreen.SetActive(false);
                break;
        }
    }

    #endregion
}
