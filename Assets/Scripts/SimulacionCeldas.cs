using System.Collections.Generic;
using UnityEngine;

public enum TipoCelda { Ciudadano, Tribu, Imperio }

public class Celda
{
    public TipoCelda tipo;
    public Vector2 posicion;
    public Vector2 direccion;
    public int victorias;
    public bool viva = true;
    public GameObject objeto;
}

public class SimulacionCeldas : MonoBehaviour
{
    public Camera camara;
    public GameObject fondo;
    public MarcadorUI marcador;

    public float intervaloSpawn = 0.2f;
    public int maxCiudadanos = 300;
    public float radioVecindad = 5f;
    public float intervaloRegla = 0.5f;
    public int victoriasParaConquista = 3;
    public float velocidadCiudadano = 0.8f;
    public float velocidadTribu = 1.5f;
    public float velocidadImperio = 1f;
    public float ruidoGiro = 90f;

    public GameObject prefabCiudadano;
    public GameObject prefabTribu;
    public GameObject prefabImperio;

    // Lo activa el evento de lluvia
    public bool lloviendo;

    List<Celda> celdas = new List<Celda>();
    float temporizadorSpawn;
    float temporizadorRegla;
    bool terminado;

    public bool Terminado => terminado;

    void Start()
    {
        // Camara centrada
        if (camara == null) camara = Camera.main;

        if (fondo != null)
        {
            Vector2 mitadFondo = MitadVista();
            fondo.transform.localScale = new Vector3(mitadFondo.x * 2f, mitadFondo.y * 2f, 1f);
            fondo.transform.position = new Vector3(0f, 0f, 1f);
        }

        for (int i = 0; i < 20; i++)
        {
            CrearCelda(PuntoSpawnAleatorio(), TipoCelda.Ciudadano);
        }

        ActualizarUI();
    }

    void Update()
    {
        if (terminado) return;

        float dt = Time.deltaTime;

        temporizadorSpawn += dt;
        if (temporizadorSpawn >= intervaloSpawn)
        {
            temporizadorSpawn = 0f;
            IntentarSpawnCiudadano();
        }

        temporizadorRegla += dt;
        if (temporizadorRegla >= intervaloRegla)
        {
            temporizadorRegla = 0f;
            AplicarReglas();
        }

        Mover(dt);
        ActualizarUI();
    }

    //  Lo usa el terremoto
    public void MatarTodas()
    {
        foreach (Celda c in celdas)
        {
            if (c.viva) Matar(c);
        }
        celdas.RemoveAll(c => !c.viva);
    }

    Vector2 MitadVista()
    {
        float alto = camara.orthographicSize;
        float ancho = alto * camara.aspect;
        return new Vector2(ancho, alto);
    }

    void IntentarSpawnCiudadano()
    {
        int ciudadanosVivos = 0;
        foreach (Celda c in celdas)
        {
            if (c.viva && c.tipo == TipoCelda.Ciudadano) ciudadanosVivos++;
        }

        if (ciudadanosVivos < maxCiudadanos)
        {
            CrearCelda(PuntoSpawnAleatorio(), TipoCelda.Ciudadano);
        }
    }

    Vector2 PuntoSpawnAleatorio()
    {
        Vector2 mitad = MitadVista();
        return new Vector2(Random.Range(-mitad.x, mitad.x), Random.Range(-mitad.y, mitad.y));
    }

    void CrearCelda(Vector2 posicion, TipoCelda tipo)
    {
        float angulo = Random.Range(0f, Mathf.PI * 2f);

        Celda celda = new Celda
        {
            tipo = tipo,
            posicion = posicion,
            direccion = new Vector2(Mathf.Cos(angulo), Mathf.Sin(angulo))
        };

        celdas.Add(celda);
        ActualizarVisual(celda);
    }

    void ActualizarVisual(Celda celda)
    {
        // Al cambiar de tipo se destruye el objeto anterior
        if (celda.objeto != null) Destroy(celda.objeto);

        GameObject prefab = celda.tipo == TipoCelda.Ciudadano ? prefabCiudadano
                          : celda.tipo == TipoCelda.Tribu ? prefabTribu
                          : prefabImperio;

        celda.objeto = Instantiate(prefab);
        celda.objeto.transform.position = new Vector3(celda.posicion.x, celda.posicion.y, 0f);
    }

    void Matar(Celda celda)
    {
        celda.viva = false;
        if (celda.objeto != null) Destroy(celda.objeto);
    }

    void AplicarReglas()
    {
        // Evita que una celda actúe dos veces en el mismo turno
        HashSet<Celda> yaActuaron = new HashSet<Celda>();
        List<Celda> copia = new List<Celda>(celdas);

        foreach (Celda celda in copia)
        {
            if (!celda.viva || yaActuaron.Contains(celda)) continue;

            yaActuaron.Add(celda);

            switch (celda.tipo)
            {
                case TipoCelda.Ciudadano:
                    ReglaCiudadano(celda);
                    break;

                case TipoCelda.Tribu:
                    ReglaTribu(celda, yaActuaron);
                    break;

                case TipoCelda.Imperio:
                    if (ReglaImperio(celda, yaActuaron)) return;
                    break;
            }
        }

        celdas.RemoveAll(c => !c.viva);
    }

    void ReglaCiudadano(Celda ciudadano)
    {
        if (Vecinos(ciudadano, TipoCelda.Tribu).Count > 0)
        {
            Matar(ciudadano);
            return;
        }

        List<Celda> ciudadanosCercanos = Vecinos(ciudadano, TipoCelda.Ciudadano);
        if (ciudadanosCercanos.Count >= 2)
        {
            Matar(ciudadanosCercanos[0]);
            Matar(ciudadanosCercanos[1]);

            ciudadano.tipo = TipoCelda.Tribu;
            ActualizarVisual(ciudadano);
        }
    }

    void ReglaTribu(Celda tribu, HashSet<Celda> yaActuaron)
    {
        List<Celda> tribusCercanas = Vecinos(tribu, TipoCelda.Tribu);
        if (tribusCercanas.Count == 0) return;

        Celda rival = tribusCercanas[0];
        yaActuaron.Add(rival);

        bool ganaTribu = Random.value < 0.5f;
        Celda ganador = ganaTribu ? tribu : rival;
        Celda perdedor = ganaTribu ? rival : tribu;

        Matar(perdedor);
        ganador.tipo = TipoCelda.Imperio;
        ActualizarVisual(ganador);
    }

    // Devuelve true si el imperio conquista el mapa
    bool ReglaImperio(Celda imperio, HashSet<Celda> yaActuaron)
    {
        List<Celda> imperiosCercanos = Vecinos(imperio, TipoCelda.Imperio);
        if (imperiosCercanos.Count == 0) return false;

        Celda rival = imperiosCercanos[0];
        yaActuaron.Add(rival);

        bool ganaImperio = Random.value < 0.5f;
        Celda ganador = ganaImperio ? imperio : rival;
        Celda perdedor = ganaImperio ? rival : imperio;

        ganador.victorias++;

        Vector2 posicionPerdedor = perdedor.posicion;
        Matar(perdedor);

        // El perdedor deja una tribu con un 50% de probabilidad
        if (Random.value < 0.5f)
        {
            CrearCelda(posicionPerdedor, TipoCelda.Tribu);
        }

        if (ganador.victorias >= victoriasParaConquista)
        {
            Conquistar(ganador);
            return true;
        }

        return false;
    }

    void Conquistar(Celda ganador)
    {
        terminado = true;

        foreach (Celda c in celdas)
        {
            if (c != ganador && c.viva) Matar(c);
        }
        celdas.RemoveAll(c => c != ganador);

        Vector2 mitadConquista = MitadVista();
        ganador.objeto.transform.position = Vector3.zero;
        ganador.objeto.transform.localScale = new Vector3(mitadConquista.x * 2f, mitadConquista.y * 2f, 1f);

        ActualizarUI();
        if (marcador != null) marcador.MostrarConquista();
    }

    List<Celda> Vecinos(Celda celda, TipoCelda tipo)
    {
        List<Celda> resultado = new List<Celda>();
        float radioCuadrado = radioVecindad * radioVecindad;

        foreach (Celda otra in celdas)
        {
            if (otra == celda || !otra.viva || otra.tipo != tipo) continue;

            if ((celda.posicion - otra.posicion).sqrMagnitude <= radioCuadrado)
            {
                resultado.Add(otra);
            }
        }

        return resultado;
    }

    void Mover(float dt)
    {
        Vector2 mitad = MitadVista();

        foreach (Celda celda in celdas)
        {
            if (!celda.viva) continue;

            bool esCiudadano = celda.tipo == TipoCelda.Ciudadano;

            // Con lluvia se mueven los ciudadanos. Sin lluvia, tribus e imperios
            if (lloviendo && !esCiudadano) continue;
            if (!lloviendo && esCiudadano) continue;

            float velocidad = esCiudadano ? velocidadCiudadano
                            : celda.tipo == TipoCelda.Tribu ? velocidadTribu
                            : velocidadImperio;

            float giro = Random.Range(-ruidoGiro, ruidoGiro) * dt;
            celda.direccion = RotarVector(celda.direccion, giro);

            celda.posicion += celda.direccion * velocidad * dt;

            // Rebote en los bordes de la vista
            if (Mathf.Abs(celda.posicion.x) > mitad.x)
            {
                celda.direccion.x = -celda.direccion.x;
                celda.posicion.x = Mathf.Clamp(celda.posicion.x, -mitad.x, mitad.x);
            }
            if (Mathf.Abs(celda.posicion.y) > mitad.y)
            {
                celda.direccion.y = -celda.direccion.y;
                celda.posicion.y = Mathf.Clamp(celda.posicion.y, -mitad.y, mitad.y);
            }

            celda.objeto.transform.position = new Vector3(celda.posicion.x, celda.posicion.y, 0f);
        }
    }

    Vector2 RotarVector(Vector2 v, float grados)
    {
        float rad = grados * Mathf.Deg2Rad;
        float seno = Mathf.Sin(rad);
        float coseno = Mathf.Cos(rad);
        return new Vector2(v.x * coseno - v.y * seno, v.x * seno + v.y * coseno);
    }

    void ActualizarUI()
    {
        if (marcador == null) return;

        int ciudadanos = 0;
        int tribus = 0;
        int imperios = 0;
        List<int> victoriasImperios = new List<int>();

        foreach (Celda c in celdas)
        {
            if (!c.viva) continue;

            if (c.tipo == TipoCelda.Ciudadano) ciudadanos++;
            else if (c.tipo == TipoCelda.Tribu) tribus++;
            else
            {
                imperios++;
                victoriasImperios.Add(c.victorias);
            }
        }

        marcador.ActualizarResumen(ciudadanos, tribus, imperios);
        marcador.ActualizarVictorias(victoriasImperios, victoriasParaConquista);
    }
}