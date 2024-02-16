using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    private PlayerSave _playerSave;

    [Header("Screens")]
    [SerializeField] private GameObject _mainMenuScreen;
    [SerializeField] private GameObject _creditScreen;
    [SerializeField] private GameObject _settingsScreen;
    [SerializeField] private GameObject _exitScreen;

    [Header("Volume")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private TMP_Text _masterVolumeText;
    [SerializeField] private TMP_Text _musicVolumeText;
    [SerializeField] private TMP_Text _soundVolumeText;


    void OnEnable()
    {
        _mainMenuScreen.SetActive(true);
        _creditScreen.SetActive(false);
        _settingsScreen.SetActive(false);
        _exitScreen.SetActive(false);

        _playerSave = SaveManager.Load();
        SaveManager.DebugSave(_playerSave, true);

        UpdateSliderDisplay(_masterVolumeSlider, _masterVolumeText, _playerSave.masterVolume);
        UpdateSliderDisplay(_musicVolumeSlider, _musicVolumeText, _playerSave.musicVolume);
        UpdateSliderDisplay(_soundVolumeSlider, _soundVolumeText, _playerSave.soundVolume);
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
        if (!visible)
        {
            SaveManager.Save(_playerSave);
            SaveManager.DebugSave(_playerSave, true);
        }
        _mainMenuScreen.SetActive(!visible);
        _settingsScreen.SetActive(visible);
    }

    public void ChangeMasterVolume()
    {
        float newVolume = _masterVolumeSlider.value * .01f;
        AudioManager.Instance.SetMasterVolume(Mathf.Clamp(_masterVolumeSlider.value * .01f, 0f, 1f));
        _playerSave.masterVolume = newVolume;

        UpdateSliderDisplay(_masterVolumeSlider, _masterVolumeText, newVolume);
    }
    public void ChangeMusicVolume()
    {
        float newVolume = _musicVolumeSlider.value * .01f;
        AudioManager.Instance.SetMusicVolume(Mathf.Clamp(_musicVolumeSlider.value * .01f, 0f, 1f));
        _playerSave.musicVolume = newVolume;
        UpdateSliderDisplay(_musicVolumeSlider, _musicVolumeText, newVolume);
    }
    public void ChangeSoundVolume()
    {
        float newVolume = _soundVolumeSlider.value * .01f;
        AudioManager.Instance.SetSoundVolume(Mathf.Clamp(_soundVolumeSlider.value * .01f, 0f, 1f));
        _playerSave.soundVolume = newVolume;
        UpdateSliderDisplay(_soundVolumeSlider, _soundVolumeText, newVolume);
    }

    private void UpdateSliderDisplay(Slider slider, TMP_Text text, float value)
    {
        float sliderValue = value * 100f;
        slider.value = sliderValue;
        text.SetText($"{sliderValue.ToString("F0")}%");

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