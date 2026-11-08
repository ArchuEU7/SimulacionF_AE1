using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class MarcadorUI : MonoBehaviour
{
    public TextMeshProUGUI textoResumen;
    public TextMeshProUGUI textoVictorias;
    public TextMeshProUGUI textoConquista;

    void Start()
    {
        if (textoConquista != null) textoConquista.gameObject.SetActive(false);
    }

    public void ActualizarResumen(int ciudadanos, int tribus, int imperios)
    {
        if (textoResumen == null) return;

        textoResumen.text = "Ciudadanos: " + ciudadanos
            + "   Tribus: " + tribus
            + "   Imperios: " + imperios;
    }

    public void ActualizarVictorias(List<int> victoriasImperios, int objetivo)
    {
        if (textoVictorias == null) return;

        StringBuilder texto = new StringBuilder();
        for (int i = 0; i < victoriasImperios.Count; i++)
        {
            texto.Append("Imperio " + (i + 1) + ": " + victoriasImperios[i] + "/" + objetivo);
            if (i < victoriasImperios.Count - 1) texto.Append("\n");
        }

        textoVictorias.text = texto.ToString();
    }

    public void MostrarConquista()
    {
        if (textoConquista != null) textoConquista.gameObject.SetActive(true);
    }
}