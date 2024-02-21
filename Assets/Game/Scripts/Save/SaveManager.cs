using System;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerSave
{
    public int playerHighscore;
    public float masterVolume;
    public float musicVolume;
    public float soundVolume;
}

public static class SaveManager
{
    private static readonly string applicationPath = Application.persistentDataPath;
    private static readonly string saveFile = "player.sav";

    /// <summary>
    /// Checks if there is a save file available or not.
    /// </summary>
    /// <returns></returns>
    private static bool SaveFileExists()
    {
        string savePath = Path.Combine(applicationPath, saveFile);
        return File.Exists(savePath);
    }

    /// <summary>
    /// Checks if there is the save file contain valid data.
    /// </summary>
    /// <returns></returns>
    private static bool IsSaveFileValid()
    {
        try
        {
            string savePath = Path.Combine(applicationPath, saveFile);
            var savedDataString = File.ReadAllText(savePath);
            JsonUtility.FromJson<PlayerSave>(savedDataString);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Creates a new PlayerSave.
    /// </summary>
    /// <returns></returns>
    private static PlayerSave CreateNewSave(int highscore = 0, float masterVolume = .25f, float musicVolume = .25f, float soundVolume = .25f)
    {
        return new PlayerSave
        {
            playerHighscore = highscore,
            masterVolume = masterVolume,
            musicVolume = musicVolume,
            soundVolume = soundVolume
        };
    }

    /// <summary>
    /// Save a PlayerSave into an actual file.
    /// </summary>
    /// <param name="playerSave">The PlayerSave to save.</param>
    public static void Save(PlayerSave playerSave)
    {
        string savePath = Path.Combine(applicationPath, saveFile);
        File.WriteAllText(savePath, JsonUtility.ToJson(playerSave));
    }

    /// <summary>
    /// Load a save file.
    /// </summary>
    /// <returns>A PlayerSave to use or read from.</returns>
    public static PlayerSave Load()
    {
        PlayerSave loadedSave;

        if (!SaveFileExists() || !IsSaveFileValid())
        {
            loadedSave = CreateNewSave();
            Save(loadedSave);
            Debug.LogWarning($"[SaveManager.Load] No save file available, created a new one.");
            return loadedSave;
        }

        string savePath = Path.Combine(applicationPath, saveFile);

        var savedDataString = File.ReadAllText(savePath);

        return JsonUtility.FromJson<PlayerSave>(savedDataString);
    }

    /// <summary>
    /// Prints the PlayerSave in JSON into the console for debugging purposes.
    /// </summary>
    /// <param name="playerSave">The PlayerSave to debug.</param>
    /// <param name="beautified">Beautify the JSON or not.</param>
    public static void DebugSave(PlayerSave playerSave, bool beautified)
    {
        Debug.Log(JsonUtility.ToJson(playerSave, beautified));
    }
}