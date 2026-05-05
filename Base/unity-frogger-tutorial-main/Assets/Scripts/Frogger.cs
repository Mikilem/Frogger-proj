using System.Collections;
using UnityEngine;
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

        // Check for collision at the destination
        Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
        Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));
        Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier"));

        // Prevent any movement if there is a barrier
        if (barrier != null) {
            return;
        }

        // Attach/detach frogger from the platform
        if (platform != null) {
            transform.SetParent(platform.transform);
        } else {
            transform.SetParent(null);
        }

        // Frogger dies when it hits an obstacle
        if (obstacle != null && platform == null)
        {
            transform.position = destination;
            Death();
        }
        // Conditions pass, move to the destination
        else
        {
            // Check if we have advanced to a farther row
            if (destination.y > farthestRow)
            {
                farthestRow = destination.y;
                GameManager.Instance.AdvancedRow();
            }

            // Start leap animation
            StopAllCoroutines();
            StartCoroutine(Leap(destination));
        }
    }

    private IEnumerator Leap(Vector3 destination)
    {
        Vector3 startPosition = transform.position;

        float elapsed = 0f;
        float duration = 0.125f;

        // Set initial state
        spriteRenderer.sprite = leapSprite;
        cooldown = true;

        while (elapsed < duration)
        {
            // Move towards the destination over time
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, destination, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set final state
        transform.position = destination;
        spriteRenderer.sprite = idleSprite;
        cooldown = false;
    }

    public void Respawn()
    {
        // Stop animations
        StopAllCoroutines();

        // Reset transform to spawn
        transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        farthestRow = spawnPosition.y;

        // Reset sprite
        spriteRenderer.sprite = idleSprite;

        // Enable control
        gameObject.SetActive(true);
        enabled = true;
        cooldown = false;
    }

    public void Death()
    {
        // Stop animations
        StopAllCoroutines();

        // Disable control
        enabled = false;

        // Display death sprite
        transform.rotation = Quaternion.identity;
        spriteRenderer.sprite = deadSprite;

        // Update game state
        GameManager.Instance.Died();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool hitObstacle = other.gameObject.layer == LayerMask.NameToLayer("Obstacle");
        bool onPlatform = transform.parent != null;

        if (enabled && hitObstacle && !onPlatform) {
            Death();
        }
    }

}
