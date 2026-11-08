using System.Collections;
using UnityEngine;

public class Lluvia : MonoBehaviour
{
    public SimulacionCeldas simulacion;
    public AudioSource fuente;
    public AudioClip sonido;
    public GameObject efectoVisual;
    public UIEventos uiEventos;

    public float duracion = 8f;

    public bool Activo { get; private set; }

    void Start()
    {
        if (fuente == null) fuente = gameObject.AddComponent<AudioSource>();
        if (efectoVisual != null) efectoVisual.SetActive(false);
    }

   
    public void Activar()
    {
        if (Activo || simulacion == null || simulacion.Terminado) return;

        StartCoroutine(Llover());
    }

    IEnumerator Llover()
    {
        Activo = true;
        simulacion.lloviendo = true;

        //  aviso mientras dura la lluvia
        if (uiEventos != null) uiEventos.Mostrar("¡Lluvia!", duracion);

        if (efectoVisual != null) efectoVisual.SetActive(true);

        if (sonido != null)
        {
            fuente.clip = sonido;
            fuente.loop = true;
            fuente.Play();
        }

        yield return new WaitForSeconds(duracion);

        simulacion.lloviendo = false;
        fuente.Stop();

        if (efectoVisual != null) efectoVisual.SetActive(false);

        Activo = false;
    }
}