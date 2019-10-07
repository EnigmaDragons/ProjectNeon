using UnityEngine;


[CreateAssetMenu(fileName = "New Saver", menuName = "Saver")]
public class Settings : ScriptableObject
{
    public void SetSound(int soundVolume)
    {
        Save(KeysEnum.SoundVolume, soundVolume);
    }
    public void SetMusic(int musicVolume)
    {
        Save(KeysEnum.MusicVolume, musicVolume);
    }
    public int GetSound()
    {
        return LoadInt(KeysEnum.SoundVolume);
    }
    public int GetMusic()
    {
        return LoadInt(KeysEnum.MusicVolume);
    }
    private static void Save(KeysEnum saveKey, int data)
    {
        PlayerPrefs.SetInt(saveKey.ToString(), data);
    }
    private static int LoadInt(KeysEnum saveKey)
    {
        return PlayerPrefs.GetInt(saveKey.ToString());
    }
}
