using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
    private static readonly string path = Application.persistentDataPath;
    private static readonly string filename = "player.sav";

    /// <summary>
    /// Checks if there is a save file available or not.
    /// </summary>
    /// <returns></returns>
    public static bool SaveFileExists()
    {
        return File.Exists($"{path}/{filename}");
    }

    /// <summary>
    /// Creates a new PlayerSave.
    /// </summary>
    /// <returns></returns>
    public static PlayerSave CreateNew(int highscore = 0, float masterVolume = .25f, float musicVolume = .25f, float soundVolume = .25f)
    {
        return new PlayerSave()
        {
            playerHighscore = highscore,
            masterVolume = masterVolume,
            musicVolume = musicVolume,
            soundVolume = soundVolume,
        };
    }

    /// <summary>
    /// Save a PlayerSave into an actual file.
    /// </summary>
    /// <param name="playerSave">The PlayerSave to save.</param>
    public static void Save(PlayerSave playerSave)
    {
        BinaryFormatter binaryFormatter = new();
        FileStream fileStream = new($"{path}/{filename}", FileMode.Create);
        binaryFormatter.Serialize(fileStream, playerSave);
        fileStream.Close();
    }

    /// <summary>
    /// Load a save file.
    /// </summary>
    /// <returns>A PlayerSave to use or read from.</returns>
    public static PlayerSave Load()
    {
        PlayerSave loadedSave;

        if (!SaveFileExists())
        {
            loadedSave = CreateNew();
            Save(loadedSave);
            Debug.LogWarning($"[SaveManager.Load] No save file available, created a new one.");
            return loadedSave;
        }
        else
        {
            BinaryFormatter binaryFormatter = new();
            FileStream fileStream = new($"{path}/{filename}", FileMode.Open);
            loadedSave = binaryFormatter.Deserialize(fileStream) as PlayerSave;
            fileStream.Close();
            return loadedSave;
        }
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
