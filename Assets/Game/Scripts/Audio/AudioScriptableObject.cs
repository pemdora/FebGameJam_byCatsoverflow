using UnityEngine;

[CreateAssetMenu(fileName = "Properties", menuName = "ScriptableObjects/audioScriptableObject", order = 0)]
public class AudioScriptableObject : ScriptableObject
{
    public GenericDictionary<SoundEffectType, AudioClip> soundsEffectsBySoundType;
    public GenericDictionary<MusicType, AudioClip> musicsBySoundType;
}