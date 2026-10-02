using System.IO;
using UnityEngine;

public static class ApiConfig
{
    private const string FILE_NAME = "config.json";

    private static string apiUrl;

    public static string API_URL
    {
        get
        {
            if (string.IsNullOrEmpty(apiUrl))
            {
                CargarConfiguracion();
            }

            return apiUrl;
        }
    }

    private static void CargarConfiguracion()
    {
        string ruta = Path.Combine(
            Application.streamingAssetsPath,
            FILE_NAME
        );

        if (!File.Exists(ruta))
        {
            apiUrl = "https://agromijo-api.onrender.com";

            Debug.LogWarning(
                $"No se encontró {FILE_NAME} en StreamingAssets. " +
                $"Se utilizará la URL predeterminada: {apiUrl}"
            );

            return;
        }

        try
        {
            string json = File.ReadAllText(ruta);
            ConfigData config = JsonUtility.FromJson<ConfigData>(json);

            if (config == null || string.IsNullOrWhiteSpace(config.apiUrl))
            {
                throw new System.Exception(
                    "El archivo config.json no contiene una URL válida."
                );
            }

            apiUrl = config.apiUrl.Trim().TrimEnd('/');

            Debug.Log($"API configurada: {apiUrl}");
        }
        catch (System.Exception e)
        {
            apiUrl = "https://agromijo-api.onrender.com";

            Debug.LogError(
                $"No se pudo leer {FILE_NAME}: {e.Message}\n" +
                $"Se utilizará la URL predeterminada: {apiUrl}"
            );
        }
    }

    [System.Serializable]
    private class ConfigData
    {
        public string apiUrl;
    }
}