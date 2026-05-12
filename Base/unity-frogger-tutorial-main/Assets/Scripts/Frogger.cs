using System.Collections;
using UnityEngine;
<<<<<<< HEAD
using UnityEngine.SceneManagement; // win 
using static ShopManager;
[RequireComponent(typeof(SpriteRenderer))]
public class Frogger : MonoBehaviour
{
    // Las variables de estado actuales se mantienen
=======

[RequireComponent(typeof(SpriteRenderer))]
public class SinFrogger : MonoBehaviour
{
>>>>>>> origin/Monedas
    public Sprite idleSprite;
    public Sprite leapSprite;
    public Sprite deadSprite;

    private SpriteRenderer spriteRenderer;
    private Vector3 spawnPosition;
    private float farthestRow;
    private bool cooldown;
<<<<<<< HEAD
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

=======
>>>>>>> origin/Monedas

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;
    }

    private void Update()
    {
<<<<<<< HEAD
        //if (hasWon) return;

=======
>>>>>>> origin/Monedas
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

<<<<<<< HEAD

=======
>>>>>>> origin/Monedas
    private void Move(Vector3 direction)
    {
        if (cooldown) return;

        Vector3 destination = transform.position + direction;

<<<<<<< HEAD
        // Comprobar si hay colisión en el destino
=======
        // Check for collision at the destination
>>>>>>> origin/Monedas
        Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
        Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));
        Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier"));

<<<<<<< HEAD
        // Impide cualquier movimiento si hay una barrera.
=======
        // Prevent any movement if there is a barrier
>>>>>>> origin/Monedas
        if (barrier != null) {
            return;
        }

<<<<<<< HEAD
        // Conectar/desconectar Frogger de la plataforma
=======
        // Attach/detach frogger from the platform
>>>>>>> origin/Monedas
        if (platform != null) {
            transform.SetParent(platform.transform);
        } else {
            transform.SetParent(null);
        }

<<<<<<< HEAD
        // Frogger muere cuando choca contra un obstáculo.
=======
        // Frogger dies when it hits an obstacle
>>>>>>> origin/Monedas
        if (obstacle != null && platform == null)
        {
            transform.position = destination;
            Death();
        }
<<<<<<< HEAD
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
=======
        // Conditions pass, move to the destination
        else
        {
            // Check if we have advanced to a farther row
            if (destination.y > farthestRow)
            {
                farthestRow = destination.y;
                SinGameManager.Instance.AdvancedRow();
            }

            // Start leap animation
>>>>>>> origin/Monedas
            StopAllCoroutines();
            StartCoroutine(Leap(destination));
        }
    }

    private IEnumerator Leap(Vector3 destination)
    {
        Vector3 startPosition = transform.position;

        float elapsed = 0f;
        float duration = 0.125f;

<<<<<<< HEAD
        // Establecer estado inicial
=======
        // Set initial state
>>>>>>> origin/Monedas
        spriteRenderer.sprite = leapSprite;
        cooldown = true;

        while (elapsed < duration)
        {
<<<<<<< HEAD
            // Avanzar hacia el destino con el tiempo
=======
            // Move towards the destination over time
>>>>>>> origin/Monedas
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, destination, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

<<<<<<< HEAD
        // Establecer estado final
=======
        // Set final state
>>>>>>> origin/Monedas
        transform.position = destination;
        spriteRenderer.sprite = idleSprite;
        cooldown = false;
    }

    public void Respawn()
    {
<<<<<<< HEAD
        // Detener animaciones
        StopAllCoroutines();

        // Reiniciar la transformación para generar
        transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        farthestRow = spawnPosition.y;

        //Restablecer sprite
        spriteRenderer.sprite = idleSprite;

        //Habilitar control
=======
        // Stop animations
        StopAllCoroutines();

        // Reset transform to spawn
        transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        farthestRow = spawnPosition.y;

        // Reset sprite
        spriteRenderer.sprite = idleSprite;

        // Enable control
>>>>>>> origin/Monedas
        gameObject.SetActive(true);
        enabled = true;
        cooldown = false;
    }

    public void Death()
    {
<<<<<<< HEAD
        // Detener animaciones
        StopAllCoroutines();

        // Deshabilitar el control
        enabled = false;

        // Mostrar sprite de muerte
        transform.rotation = Quaternion.identity;
        spriteRenderer.sprite = deadSprite;

        //Actualiza el estado del juego
        GameManager.Instance.Died();
=======
        // Stop animations
        StopAllCoroutines();

        // Disable control
        enabled = false;

        // Display death sprite
        transform.rotation = Quaternion.identity;
        spriteRenderer.sprite = deadSprite;

        // Update game state
        SinGameManager.Instance.Died();
>>>>>>> origin/Monedas
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
<<<<<<< HEAD
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
=======
        bool hitObstacle = other.gameObject.layer == LayerMask.NameToLayer("Obstacle");
        bool onPlatform = transform.parent != null;

        if (enabled && hitObstacle && !onPlatform) {
>>>>>>> origin/Monedas
            Death();
        }
    }

<<<<<<< HEAD
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
=======
}
>>>>>>> origin/Monedas
