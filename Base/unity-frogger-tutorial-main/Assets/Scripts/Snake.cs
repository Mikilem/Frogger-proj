using UnityEngine;
using System.Collections.Generic;

public class Serpiente : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] 
    private float velocidadMovimiento = 3f;
    [SerializeField] 
    private float rangoDeteccion = 10f;

    [Header("Cuerpo de la serpiente")]
    [SerializeField] 
    private GameObject prefabSegmentoCuerpo;
    [SerializeField]
    private int segmentosIniciales = 3;
    [SerializeField]
    private float separacionSegmentos = 0.5f;

    private Transform frogger;
    private bool persiguiendo = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Listas para el seguimiento del cuerpo
    private List<Transform> segmentosCuerpo = new List<Transform>();
    private List<Vector2> historialPosiciones = new List<Vector2>();

    void Start()
    {
        // Buscar a Frogger por nombre o tag
        GameObject jugador = GameObject.Find("Frogger");
        if (jugador == null)
        {
            jugador = GameObject.FindGameObjectWithTag("Player");
        }

        if (jugador != null)
        {
            frogger = jugador.transform;
            persiguiendo = true;
            Debug.Log("Serpiente: ¡Frogger encontrado! Comenzando persecución...");
        }
        else
        {
            Debug.LogError("Serpiente: ¡No se ha encontrado a Frogger!");
            persiguiendo = false;
        }

        // Componentes
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Inicializar el cuerpo de la serpiente
        InicializarCuerpo();
    }

    void Update()
    {
        if (!persiguiendo || frogger == null) return;

        // Actualizar la posición del cuerpo
        ActualizarPosicionesCuerpo();
    }

    void FixedUpdate()
    {
        if (!persiguiendo || frogger == null) return;

        // Calcular distancia a Frogger
        float distanciaAFrogger = Vector2.Distance(transform.position, frogger.position);

        // Si está dentro del rango de detección, perseguir
        if (distanciaAFrogger <= rangoDeteccion)
        {
            MoverseHaciaFrogger();
        }
    }

    void MoverseHaciaFrogger()
    {
        // Calcular dirección hacia Frogger
        Vector2 direccion = (frogger.position - transform.position).normalized;

        // Mover la serpiente
        rb.velocity = direccion * velocidadMovimiento;

        // Girar la serpiente hacia la dirección del movimiento
        if (direccion != Vector2.zero)
        {
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            rb.rotation = angulo;
        }
    }

    void InicializarCuerpo()
    {
        segmentosCuerpo.Clear();
        historialPosiciones.Clear();

        // Añadir la cabeza (este transform)
        segmentosCuerpo.Add(transform);
        historialPosiciones.Add(transform.position);

        // Crear segmentos del cuerpo
        for (int i = 0; i < segmentosIniciales; i++)
        {
            GameObject segmento = Instantiate(prefabSegmentoCuerpo, transform.position, Quaternion.identity);
            segmento.transform.parent = transform.parent;
            segmentosCuerpo.Add(segmento.transform);
            historialPosiciones.Add(segmento.transform.position);
        }
    }

    void ActualizarPosicionesCuerpo()
    {
        // Guardar la posición actual de la cabeza
        historialPosiciones.Insert(0, transform.position);

        // Limitar el historial
        int maxHistorial = segmentosCuerpo.Count * Mathf.CeilToInt(separacionSegmentos * 10);
        if (historialPosiciones.Count > maxHistorial)
        {
            historialPosiciones.RemoveAt(historialPosiciones.Count - 1);
        }

        // Actualizar cada segmento del cuerpo
        for (int i = 1; i < segmentosCuerpo.Count; i++)
        {
            int indiceHistorial = Mathf.FloorToInt(i * separacionSegmentos * 10);

            if (indiceHistorial < historialPosiciones.Count)
            {
                Vector2 posicionObjetivo = historialPosiciones[indiceHistorial];
                segmentosCuerpo[i].position = Vector2.Lerp(segmentosCuerpo[i].position, posicionObjetivo, Time.deltaTime * 10f);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Comprobar si toca a Frogger
        if (collision.gameObject.name == "Frogger" || collision.gameObject.CompareTag("Player"))
        {
            MatarFrogger(collision.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Comprobar si toca a Frogger (trigger)
        if (other.gameObject.name == "Frogger" || other.gameObject.CompareTag("Player"))
        {
            MatarFrogger(other.gameObject);
        }
    }

    void MatarFrogger(GameObject froggerObject)
    {
        Debug.Log("?? ¡LA SERPIENTE HA MATADO A FROGGER! ????");

        // Opción 1: Destruir a Frogger
        Destroy(froggerObject);

        // Opción 2: Desactivarlo (comenta la línea de arriba y descomenta estas si prefieres)
        // froggerObject.SetActive(false);

        // Detener la persecución
       // persiguiendo = false;

        // Opcional: La serpiente hace una animación de victoria
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; // Cambia de color
        }

        // Detener el movimiento
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar el rango de detección en el editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f); // Rango de contacto
    }
}