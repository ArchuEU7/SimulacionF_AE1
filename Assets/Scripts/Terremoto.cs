using System.Collections;
using UnityEngine;

public class Terremoto : MonoBehaviour
{
    public SimulacionCeldas simulacion;
    public Camera camara;
    public AudioSource fuente;
    public AudioClip sonido;
    public GameObject efectoVisual;
    public UIEventos uiEventos;

    public float duracion = 2f;
    public float intensidad = 0.5f;

    void Start()
    {
        if (camara == null) camara = Camera.main;
        if (fuente == null) fuente = gameObject.AddComponent<AudioSource>();
        if (efectoVisual != null) efectoVisual.SetActive(false);
    }

    // Llamado por EventosAleatorios
    public void Activar()
    {
        if (simulacion == null || simulacion.Terminado) return;

        simulacion.MatarTodas();

        // Mostramos el aviso en pantalla durante la sacudida
        if (uiEventos != null) uiEventos.Mostrar("¡Terremoto!", duracion);

        if (sonido != null) fuente.PlayOneShot(sonido);

        StartCoroutine(Sacudir());
    }

    IEnumerator Sacudir()
    {
        if (efectoVisual != null) efectoVisual.SetActive(true);

        Vector3 origen = camara.transform.position;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float fuerza = intensidad * (1f - tiempo / duracion);
            Vector3 desplazamiento = new Vector3(Random.Range(-fuerza, fuerza), Random.Range(-fuerza, fuerza), 0f);
            camara.transform.position = origen + desplazamiento;
            yield return null;
        }

        camara.transform.position = origen;

        if (efectoVisual != null) efectoVisual.SetActive(false);
    }
}