using System.Collections.Generic;
using UnityEngine;

// Coloca este script en un GameObject vacío en la escena del tutorial.
// En la escena normal NO existe, así que Instancia es null
// y las llamadas .NotificarAccion() no hacen nada.
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instancia { get; private set; }

    [System.Serializable]
    public class TutorialStep
    {
        [TextArea(2, 5)]
        public string mensaje;

        [Tooltip("Elemento a destacar con el spotlight. Null = overlay completo.")]
        public RectTransform objetivo;

        public TutorialCondicion condicion;
    }

    public List<TutorialStep> pasos = new List<TutorialStep>();

    private int pasoActual = -1;

    private void Awake()
    {
        Instancia = this;
    }

    private void Start()
    {
        AvanzarPaso();
    }

    public void AvanzarPaso()
    {
        pasoActual++;

        if (pasoActual >= pasos.Count)
        {
            TerminarTutorial();
            return;
        }

        TutorialStep paso = pasos[pasoActual];
        bool esManual = paso.condicion == TutorialCondicion.Manual;
        TutorialOverlayUI.Instancia.MostrarPaso(paso.mensaje, paso.objetivo, esManual);
    }

    // Llamado desde los scripts del juego cuando ocurre una acción.
    // Si la condición del paso actual coincide, avanza automáticamente.
    public void NotificarAccion(TutorialCondicion condicion)
    {
        if (pasoActual < 0 || pasoActual >= pasos.Count) return;
        if (pasos[pasoActual].condicion == condicion)
            AvanzarPaso();
    }

    private void TerminarTutorial()
    {
        TutorialOverlayUI.Instancia.Ocultar();
        SceneLoader.Instance.LoadMainMenu();
    }
}
