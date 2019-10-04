using UnityEngine;


[CreateAssetMenu(fileName = "New Saver", menuName = "Saver")]
public class Saver : ScriptableObject
{
    // @todo #281: 30min Add metod to check if the one key is used more then ones
    public static void Save(KeysEnum saveKey, string data)
    {
        PlayerPrefs.SetString(saveKey.ToString(), data);
    }
    public static void Save(KeysEnum saveKey, int data)
    {
        PlayerPrefs.SetInt(saveKey.ToString(), data);
    }
    public static void Save(KeysEnum saveKey, float data)
    {
        PlayerPrefs.SetFloat(saveKey.ToString(), data);
    }

    public static string LoadString(KeysEnum saveKey)
    {
        return PlayerPrefs.GetString(saveKey.ToString());
    }
    public static int LoadInt(KeysEnum saveKey)
    {
        return PlayerPrefs.GetInt(saveKey.ToString());
    }
    public static float LoadFloat(KeysEnum saveKey)
    {
        return PlayerPrefs.GetFloat(saveKey.ToString());
    }
}
