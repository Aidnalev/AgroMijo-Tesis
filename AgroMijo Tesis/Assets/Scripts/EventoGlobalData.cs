using UnityEngine;

public enum CategoriaEvento
{
    Mercado,         // alza/baja de precios de un cultivo específico
    Infraestructura, // vía dañada, afecta capacidad de comercializar
    Social,          // escuela, salud, afectan productividad a largo plazo
    Climatico        // sequía, lluvia excesiva, afecta todos los cultivos
}

// Crea eventos desde: clic derecho en Proyecto > Create > AgroMijo > EventoGlobal
[CreateAssetMenu(fileName = "NuevoEvento", menuName = "AgroMijo/EventoGlobal")]
public class EventoGlobalData : ScriptableObject
{
    [Header("Identificación")]
    public string nombreEvento;
    [TextArea] public string descripcionAlOcurrir;
    public CategoriaEvento categoria;

    [Header("Probabilidad")]
    [Range(0f, 1f)] public float probabilidadPorCiclo = 0.15f;

    [Header("Persistencia")]
    public bool esPersistente = false;
    [Tooltip("Ciclos hasta que se resuelve solo. 0 = nunca se resuelve solo.")]
    public int ciclosHastaAutoResolver = 0;

    [Header("Resolución opcional por inversión")]
    public bool esResolvible = false;
    public float costoResolucion = 0f;
    [TextArea] public string descripcionResolucion;

    [Header("Efectos sobre la cosecha")]
    [Tooltip("Cultivo específico afectado. Dejar vacío = afecta todos.")]
    public CultivoData cultivoAfectado;

    [Range(-1f, 1f)]
    [Tooltip("Modifica el precio de cosecha del cultivo afectado. Ej: 0.2 = +20%, -0.3 = -30%")]
    public float modificadorPrecio = 0f;

    [Range(-1f, 1f)]
    [Tooltip("Modifica el rendimiento global de todos los cultivos este ciclo.")]
    public float modificadorRendimientoGlobal = 0f;
}
