using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Snake : MonoBehaviour
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

    [Header("Ataque")]
    [SerializeField] 
    private float cooldownAtaque = 2f;  // Aumentado a 2 segundos

    private Transform frogger;
    private Frogger scriptFrogger;
    private bool persiguiendo = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Listas para el seguimiento del cuerpo
    private List<Transform> segmentosCuerpo = new List<Transform>();
    private List<Vector2> historialPosiciones = new List<Vector2>();

    // Cooldown para evitar matar a Frogger múltiples veces
    private float ultimoAtaque = 0f;
    private bool froggerAtrapado = false;  // Nuevo: evita matarlo mientras está muriendo

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
            scriptFrogger = jugador.GetComponent<Frogger>();
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

        // Si Frogger está vivo nuevamente, resetear la bandera
        if (froggerAtrapado && scriptFrogger != null && scriptFrogger.enabled && !estaMuerto())
        {
            froggerAtrapado = false;
        }
    }

    void FixedUpdate()
    {
        if (!persiguiendo || frogger == null) return;

        // No perseguir si Frogger está muerto
        if (estaMuerto())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Calcular distancia a Frogger
        float distanciaAFrogger = Vector2.Distance(transform.position, frogger.position);

        // Si está dentro del rango de detección, perseguir
        if (distanciaAFrogger <= rangoDeteccion)
        {
            MoverseHaciaFrogger();
        }
        else
        {
            // Si está fuera del rango, detenerse
            rb.velocity = Vector2.zero;
        }
    }

    bool estaMuerto()
    {
        if (scriptFrogger == null) return true;

        // Comprobar si Frogger está muerto por su sprite o por estado
        SpriteRenderer froggerSprite = frogger.GetComponent<SpriteRenderer>();
        if (froggerSprite != null && froggerSprite.sprite == scriptFrogger.deadSprite)
        {
            return true;
        }

        return !scriptFrogger.enabled;
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
            AtraparFrogger(collision.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Comprobar si toca a Frogger (trigger)
        if (other.gameObject.name == "Frogger" || other.gameObject.CompareTag("Player"))
        {
            AtraparFrogger(other.gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Para mantener la colisión mientras esté encima
        if (collision.gameObject.name == "Frogger" || collision.gameObject.CompareTag("Player"))
        {
            if (!froggerAtrapado && Time.time - ultimoAtaque >= cooldownAtaque)
            {
                AtraparFrogger(collision.gameObject);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Para mantener el trigger mientras esté encima
        if (other.gameObject.name == "Frogger" || other.gameObject.CompareTag("Player"))
        {
            if (!froggerAtrapado && Time.time - ultimoAtaque >= cooldownAtaque)
            {
                AtraparFrogger(other.gameObject);
            }
        }
    }

    void AtraparFrogger(GameObject froggerObject)
    {
        // Verificar cooldown y si ya está atrapado
        if (Time.time - ultimoAtaque < cooldownAtaque) return;
        if (froggerAtrapado) return;
        if (estaMuerto()) return;  // No matar si ya está muerto

        Debug.Log("🐍 ¡LA SERPIENTE HA ATRAPADO A FROGGER! 🐸💀");

        // Marcar como atrapado para no volver a matarlo
        froggerAtrapado = true;

        // Llamar al método Death del script Frogger
        if (scriptFrogger != null)
        {
            scriptFrogger.Death();
        }
        else
        {
            // Intentar obtener el componente si no está guardado
            scriptFrogger = froggerObject.GetComponent<Frogger>();
            if (scriptFrogger != null)
            {
                scriptFrogger.Death();
            }
            else
            {
                Debug.LogWarning("No se encontró el script Frogger en el objeto");
                froggerAtrapado = false;  // Resetear si no hay script
            }
        }

        // Actualizar el tiempo del último ataque
        ultimoAtaque = Time.time;

        // Animación de ataque
        if (spriteRenderer != null)
        {
            StartCoroutine(AnimacionAtaque());
        }

        // Reproducir sonido si existe
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        // Programar el reset de la bandera después de que Frogger respawnee
        StartCoroutine(ResetearAtrapado());
    }

    IEnumerator ResetearAtrapado()
    {
        // Esperar el tiempo de respawn + tiempo de invulnerabilidad
        yield return new WaitForSeconds(2.5f);  // Ajusta según tu tiempo de respawn

        // Resetear la bandera para poder atraparlo de nuevo
        froggerAtrapado = false;
        Debug.Log("Serpiente: Puedo atrapar a Frogger nuevamente");
    }

    IEnumerator AnimacionAtaque()
    {
        // Cambiar color para indicar ataque
        Color colorOriginal = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        // Volver al color original
        spriteRenderer.color = colorOriginal;
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar el rango de detección en el editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}