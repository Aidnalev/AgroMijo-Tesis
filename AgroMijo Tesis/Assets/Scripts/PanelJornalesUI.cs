using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Se abre automáticamente cuando los jornales familiares no alcanzan para el ciclo.
// Estructura del panel:
//   - Txt_Descripcion   → cuántos faltan y por qué
//   - Slider_Jornales   → de 0 al déficit
//   - Txt_Seleccionados → "X jornales"
//   - Txt_Costo         → "Costo: $X"
//   - Btn_Confirmar     → OnClickConfirmar()
//   - Btn_SinJornales   → OnClickSinJornales() (avanzar sin contratar)
public class PanelJornalesUI : MonoBehaviour
{
    public static PanelJornalesUI Instancia { get; private set; }

    [Header("Estructura")]
    public GameObject panelPrincipal;
    public TMP_Text textoDescripcion;
    public Slider sliderJornales;
    public TMP_Text textoSeleccionados;
    public TMP_Text textoCosto;

    private int deficitJornales;
    private Action<int> alConfirmar;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
        sliderJornales.onValueChanged.AddListener(OnSliderCambiado);
    }

    public void Mostrar(int deficit, Action<int> callback)
    {
        deficitJornales = deficit;
        alConfirmar = callback;

        sliderJornales.minValue = 0;
        sliderJornales.maxValue = deficit;
        sliderJornales.wholeNumbers = true;
        sliderJornales.value = 0;

        textoDescripcion.text = $"Te faltan {deficit} jornal(es) este ciclo.\n" +
                                $"Los jornales familiares ({GameManager.Instancia.jornalesFamiliares}) " +
                                $"no alcanzan.\n¿Cuántos quieres contratar?";

        ActualizarTextos(0);
        panelPrincipal.SetActive(true);
    }

    private void OnSliderCambiado(float valor)
    {
        ActualizarTextos(Mathf.RoundToInt(valor));
    }

    private void ActualizarTextos(int jornales)
    {
        float costo = jornales * GameManager.Instancia.costoJornalContratado;
        textoSeleccionados.text = $"Jornales a contratar: {jornales}";
        textoCosto.text = jornales > 0
            ? $"Costo: ${costo:N0}"
            : "Sin costo adicional";
    }

    // Conecta al botón "Confirmar"
    public void OnClickConfirmar()
    {
        int seleccionados = Mathf.RoundToInt(sliderJornales.value);
        panelPrincipal.SetActive(false);
        alConfirmar?.Invoke(seleccionados);
    }

    // Conecta al botón "Volver"
    // El jugador puede replantear sus decisiones y volver a intentar avanzar
    public void OnClickVolver()
    {
        panelPrincipal.SetActive(false);
    }
}