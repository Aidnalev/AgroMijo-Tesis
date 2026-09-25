using UnityEngine;

// Crea parcelas desde: clic derecho en Proyecto > Create > AgroMijo > ParcelaTemplate
// Cada asset representa una parcela posible del pool.
// Solo define características fijas — el estado en juego (cultivo, mejoras, etc.)
// se crea en runtime a partir de esta plantilla.
[CreateAssetMenu(fileName = "NuevaParcela", menuName = "AgroMijo/ParcelaTemplate")]
public class ParcelaTemplateSO : ScriptableObject
{
    [Header("Identificación")]
    public string nombreParcela = "Parcela";

    [Header("Características del terreno")]
    public TipoSuelo tipoSuelo = TipoSuelo.Franco;

    [Range(0f, 1f)]
    [Tooltip("Disponibilidad de agua base. La mejora de riego la sube permanentemente.")]
    public float aguaBase = 0.5f;

    [Range(0f, 1f)]
    [Tooltip("Calidad del acceso vial inicial. Afecta el impacto de eventos de infraestructura.")]
    public float accesoVial = 0.5f;
}
