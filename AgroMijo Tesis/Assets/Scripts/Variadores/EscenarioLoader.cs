using System.IO;
using UnityEngine;

public static class EscenarioLoader
{
    private const string FILE_NAME = "escenario.json";

    public static EscenarioConfig Cargar()
    {
        string ruta = Path.Combine(
            Application.streamingAssetsPath,
            FILE_NAME
        );

        if (!File.Exists(ruta))
        {
            Debug.LogWarning(
                $"No se encontró {FILE_NAME} en StreamingAssets."
            );

            return null;
        }

        try
        {
            string json = File.ReadAllText(ruta);

            EscenarioConfig config =
                JsonUtility.FromJson<EscenarioConfig>(json);

            if (config == null)
            {
                Debug.LogError(
                    "No se pudo interpretar escenario.json."
                );

                return null;
            }

            Debug.Log("Escenario personalizado cargado correctamente.");

            return config;
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                $"Error al cargar escenario.json: {e.Message}"
            );

            return null;
        }
    }
}