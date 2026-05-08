using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // win 
using static ShopManager;
[RequireComponent(typeof(SpriteRenderer))]
public class Frogger : MonoBehaviour
{
    // Las variables de estado actuales se mantienen
    public Sprite idleSprite;
    public Sprite leapSprite;
    public Sprite deadSprite;

    private SpriteRenderer spriteRenderer;
    private Vector3 spawnPosition;
    private float farthestRow;
    private bool cooldown;
    private bool hasWon = false;
    public bool victoryEnabled = true;

    // --- NUEVO SISTEMA DE ESTADOS ---
    [Header("Configuración de Skins")]
    public SkinData[] todasLasSkins; // Llena esto en el Inspector con tus 3 estados por skin

    void Start()
    {
        // 1. Obtener ID guardado
        int id = PlayerPrefs.GetInt("SkinEquipada", 0);

        // 2. Cargar los estados de la skin seleccionada
        if (todasLasSkins != null && todasLasSkins.Length > id)
        {
            // Asignamos los sprites de la skin elegida a las variables de estado del sapo
            idleSprite = todasLasSkins[id].idle;
            leapSprite = todasLasSkins[id].leap;
            deadSprite = todasLasSkins[id].dead;

            // Aplicamos la imagen inicial
            spriteRenderer.sprite = idleSprite;
        }
        else
        {
            Debug.LogError("El ID de skin no existe en el array todasLasSkins.");
        }
    }


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;
    }

    private void Update()
    {
        //if (hasWon) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Move(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            Move(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            Move(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            Move(Vector3.down);
        }
    }


    private void Move(Vector3 direction)
    {
        if (cooldown) return;

        Vector3 destination = transform.position + direction;

        // Comprobar si hay colisión en el destino
        Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
        Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));
        Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier"));

        // Impide cualquier movimiento si hay una barrera.
        if (barrier != null) {
            return;
        }

        // Conectar/desconectar Frogger de la plataforma
        if (platform != null) {
            transform.SetParent(platform.transform);
        } else {
            transform.SetParent(null);
        }

        // Frogger muere cuando choca contra un obstáculo.
        if (obstacle != null && platform == null)
        {
            transform.position = destination;
            Death();
        }
        // Las condiciones se cumplen, pasar al destino
        else
        {
            // Comprobar si hemos avanzado a una fila más lejana
            if (destination.y > farthestRow)
            {
                farthestRow = destination.y;
                GameManager.Instance.AdvancedRow();
            }

            // Iniciar animación de salto
            StopAllCoroutines();
            StartCoroutine(Leap(destination));
        }
    }

    private IEnumerator Leap(Vector3 destination)
    {
        Vector3 startPosition = transform.position;

        float elapsed = 0f;
        float duration = 0.125f;

        // Establecer estado inicial
        spriteRenderer.sprite = leapSprite;
        cooldown = true;

        while (elapsed < duration)
        {
            // Avanzar hacia el destino con el tiempo
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, destination, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Establecer estado final
        transform.position = destination;
        spriteRenderer.sprite = idleSprite;
        cooldown = false;
    }

    public void Respawn()
    {
        // Detener animaciones
        StopAllCoroutines();

        // Reiniciar la transformación para generar
        transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        farthestRow = spawnPosition.y;

        //Restablecer sprite
        spriteRenderer.sprite = idleSprite;

        //Habilitar control
        gameObject.SetActive(true);
        enabled = true;
        cooldown = false;
    }

    public void Death()
    {
        // Detener animaciones
        StopAllCoroutines();

        // Deshabilitar el control
        enabled = false;

        // Mostrar sprite de muerte
        transform.rotation = Quaternion.identity;
        spriteRenderer.sprite = deadSprite;

        //Actualiza el estado del juego
        GameManager.Instance.Died();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /* Solo detecta Home si victoryEnabled está ACTIVADO
        if (victoryEnabled && other.gameObject.name == "Home" && !hasWon)
        {
            hasWon = true;
            Win();
            return;
        }*/

        // Lógica original de muerte
        bool hitObstacle = other.gameObject.layer == LayerMask.NameToLayer("Obstacle");
        bool onPlatform = transform.parent != null;

        if (enabled && hitObstacle && !onPlatform)
        {
            Death();
        }
    }

    /*private void Win()
    {
        StopAllCoroutines();
        enabled = false;

        // Frogger desaparece
        spriteRenderer.enabled = false;

        // Opcional: desactivar collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Debug.Log("¡FROGGER GANÓ!");

       
    }*/
}