using System.Collections;
using TMPro;
using UnityEngine;

public class UIEventos : MonoBehaviour
{
    public TextMeshProUGUI textoEvento;

    Coroutine rutinaActual;

    void Start()
    {
        if (textoEvento != null) textoEvento.gameObject.SetActive(false);
    }

    public void Mostrar(string mensaje, float segundos)
    {
        if (textoEvento == null) return;

        
        if (rutinaActual != null) StopCoroutine(rutinaActual);
        rutinaActual = StartCoroutine(MostrarCoroutine(mensaje, segundos));
    }

    IEnumerator MostrarCoroutine(string mensaje, float segundos)
    {
        textoEvento.text = mensaje;
        textoEvento.gameObject.SetActive(true);

        yield return new WaitForSeconds(segundos);

        textoEvento.gameObject.SetActive(false);
        rutinaActual = null;
    }
}