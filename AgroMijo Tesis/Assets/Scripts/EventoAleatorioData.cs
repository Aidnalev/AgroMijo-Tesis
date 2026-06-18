using UnityEngine;

public enum TipoEfectoEvento
{
    Positivo,
    Negativo
}

// Representa un evento imprevisto (plaga, daño en vía, cambio de precio, etc.).
// Crea uno por cada evento desde: Create > AgroMijo > EventoAleatorio
[CreateAssetMenu(fileName = "NuevoEvento", menuName = "AgroMijo/EventoAleatorio")]
public class EventoAleatorioData : ScriptableObject
{
    [Header("Identificación")]
    public string nombreEvento = "Nombre del evento";

    [TextArea]
    public string descripcion = "Descripción del evento para mostrar al jugador";

    [Header("Probabilidad")]
    [Range(0f, 1f)]
    [Tooltip("Probabilidad de que ocurra este evento en un ciclo cualquiera")]
    public float probabilidadPorCiclo = 0.1f;

    [Header("Efecto")]
    public TipoEfectoEvento tipoEfecto = TipoEfectoEvento.Negativo;

    [Tooltip("Impacto económico directo. Negativo = costo, positivo = ganancia")]
    public float impactoEconomico = -50000f;

    [Tooltip("Multiplicador sobre el rendimiento de una cosecha afectada. 1 = sin efecto")]
    public float impactoRendimiento = 1f;

    [Header("Reparación (opcional)")]
    [Tooltip("Si es true, el jugador puede pagar para repararlo y evitar el impacto continuo")]
    public bool requiereReparacion = false;
    public float costoReparacion = 0f;
}
