using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Guarda los registros de partidas terminadas en:
// persistentDataPath/registros/{profileId}_registros.json
// Los registros son permanentes — no se borran al terminar una partida.
public static class RegistroPartidasManager
{
    private static string GetRuta(string profileId)
    {
        string carpeta = Path.Combine(Application.persistentDataPath, "registros");
        Directory.CreateDirectory(carpeta);
        return Path.Combine(carpeta, $"{profileId}_registros.json");
    }

    public static void GuardarRegistro(GameOverData data)
    {
        try
        {
            List<GameOverData> existentes = CargarRegistros(data.profileId);
            existentes.Add(data);

            string json = JsonUtility.ToJson(
                new PerfilRegistros { registros = existentes }, prettyPrint: true);
            File.WriteAllText(GetRuta(data.profileId), json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al guardar registro: {e.Message}");
        }
    }

    public static List<GameOverData> CargarRegistros(string profileId)
    {
        string ruta = GetRuta(profileId);
        if (!File.Exists(ruta)) return new List<GameOverData>();

        try
        {
            string json = File.ReadAllText(ruta);
            PerfilRegistros wrapper = JsonUtility.FromJson<PerfilRegistros>(json);
            return wrapper?.registros ?? new List<GameOverData>();
        }
        catch
        {
            return new List<GameOverData>();
        }
    }
}
