using System.IO;
using UnityEngine;

// Clase estática: no necesita estar en escena, se llama directamente.
// Cada perfil tiene su propio archivo: saves/{profileId}.json
public static class SaveManager
{
    private static string GetRuta(string profileId)
    {
        string carpeta = Path.Combine(Application.persistentDataPath, "saves");
        Directory.CreateDirectory(carpeta); // la crea si no existe
        return Path.Combine(carpeta, $"{profileId}.json");
    }

    public static void Guardar(GameSaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(GetRuta(data.profileId), json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al guardar la partida: {e.Message}");
        }
    }

    // Devuelve null si no existe guardado para ese perfil
    public static GameSaveData Cargar(string profileId)
    {
        string ruta = GetRuta(profileId);
        if (!File.Exists(ruta)) return null;

        try
        {
            string json = File.ReadAllText(ruta);
            return JsonUtility.FromJson<GameSaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar la partida: {e.Message}");
            return null;
        }
    }

    public static bool ExisteGuardado(string profileId)
    {
        return File.Exists(GetRuta(profileId));
    }

    // Llamar al terminar una partida para que el jugador empiece fresco la próxima vez
    public static void Borrar(string profileId)
    {
        string ruta = GetRuta(profileId);
        if (File.Exists(ruta)) File.Delete(ruta);
    }
}
