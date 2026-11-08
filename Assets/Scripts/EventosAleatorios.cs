using UnityEngine;

public class EventosAleatorios : MonoBehaviour
{
    public SimulacionCeldas simulacion;
    public Terremoto terremoto;
    public Lluvia lluvia;

    public float esperaMin = 10f;
    public float esperaMax = 20f;

    // Cuánto baja la probabilidad del evento que acaba de salir (0.5 = a la mitad)
    public float factorReduccion = 0.5f;

    float temporizador;
    float esperaActual;
    float pesoTerremoto = 1f;
    float pesoLluvia = 1f;

    void Start()
    {
        esperaActual = Random.Range(esperaMin, esperaMax);
    }

    void Update()
    {
        if (simulacion == null || simulacion.Terminado) return;

        temporizador += Time.deltaTime;

        if (temporizador >= esperaActual)
        {
            temporizador = 0f;
            esperaActual = Random.Range(esperaMin, esperaMax);
            LanzarEventoAleatorio();
        }
    }

    void LanzarEventoAleatorio()
    {
        float total = pesoTerremoto + pesoLluvia;
        float tirada = Random.value * total;

        if (tirada < pesoTerremoto)
        {
            // Sale terremoto baja su probabilidad y restablece la de la lluvia
            if (terremoto != null) terremoto.Activar();
            pesoTerremoto *= factorReduccion;
            pesoLluvia = 1f;
        }
        else
        {
            // Sale lluvia baja su probabilidad y restablece la del terremoto
            if (lluvia != null) lluvia.Activar();
            pesoLluvia *= factorReduccion;
            pesoTerremoto = 1f;
        }
    }
}