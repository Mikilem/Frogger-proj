using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MongoDB.Bson;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Home[] homes;
    [SerializeField] private Frogger frogger;
    [SerializeField] private GameObject gameOverMenu;

    [Header("UI")]
    [SerializeField] private Text timeText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text coinsText;

    [SerializeField] private LeaderboardManager leaderboard; // Añade esta línea

    public int lives { get; private set; } = 3;
    public int score { get; private set; } = 0;
    public int time { get; private set; } = 30;
    public int coins { get; private set; } = 0;

    private ConnectMongoDB mongoDbConnector; // Referencia al script ConnectMongoDB

    private void Awake()
    {
        // 1. Patrón Singleton Robusto
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Opcional: Si quieres que el GameManager no se destruya al cambiar de escena (ej: ir del Menú al Juego)
        // transform.SetParent(null); // Asegura que sea objeto raíz
        // DontDestroyOnLoad(gameObject);

        // 2. Carga de datos segura
        coins = PlayerPrefs.GetInt("Coins", 0);

        // 3. Búsqueda optimizada de componentes
        if (mongoDbConnector == null)
        {
            mongoDbConnector = FindObjectOfType<ConnectMongoDB>();

            if (mongoDbConnector == null)
            {
                Debug.LogWarning("ConnectMongoDB no está en la escena. El guardado online estará desactivado.");
            }
        }
    }

    private void Start()
    {
        NewGame();
        UpdateCoinsUI();
    }

    private void NewGame()
    {
        gameOverMenu.SetActive(false);
        SetScore(0);
        SetLives(3);
        UpdateCoinsUI();
        NewLevel();
    }

    private void NewLevel()
    {
        for (int i = 0; i < homes.Length; i++)
            homes[i].enabled = false;
        Respawn();
    }

    private void Respawn()
    {
        frogger.Respawn();
        StopAllCoroutines();
        StartCoroutine(Timer(30));
    }

    private IEnumerator Timer(int duration)
    {
        time = duration;
        timeText.text = time.ToString();

        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
            timeText.text = time.ToString();
        }

        frogger.Death();
    }

    public void Died()
    {
        SetLives(lives - 1);

        if (lives > 0)
            Invoke(nameof(Respawn), 1f);
        else
            Invoke(nameof(GameOver), 1f);
    }

    private void GameOver()
    {
        frogger.gameObject.SetActive(false);
        gameOverMenu.SetActive(true);


        SaveGameToJSONAndSendToMongoDB();
        // En lugar de llamar a la función de guardado normal, usamos la nueva secuencia
        StartCoroutine(GuardarYActualizarTabla());

        StopAllCoroutines();
        StartCoroutine(CheckForPlayAgain());

        LeaderboardManager tabla = FindObjectOfType<LeaderboardManager>();
        if (tabla != null)
        {
            tabla.MostrarRanking();
        }

        StopAllCoroutines();
        StartCoroutine(CheckForPlayAgain());
    
}

    private IEnumerator GuardarYActualizarTabla()
    {
        // 1. Preparar los datos
        PlayerData data = new PlayerData
        {
            coins = this.coins,
            score = this.score,
            currentSkin = FindObjectOfType<ImageChanger>()?.currentIndex ?? 0,
            lastSave = System.DateTime.Now.ToString("HH:mm dd/MM")
        };

        // 2. Enviar a MongoDB y ESPERAR a que la tarea termine
        if (mongoDbConnector != null)
        {
            var task = mongoDbConnector.EnviarDatosAMongoDB(data);
            // Esta línea hace que Unity espere hasta que el servidor de MongoDB responda "OK"
            yield return new WaitUntil(() => task.IsCompleted);
        }

        // 3. Ahora que el dato está en la base de datos, pedimos la tabla
        LeaderboardManager tabla = FindObjectOfType<LeaderboardManager>();
        if (tabla != null)
        {
            tabla.ActualizarYMostrarTabla();
        }
    }

    private IEnumerator CheckForPlayAgain()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                NewGame();
                yield break;
            }

            yield return null;
        }
    }

    public void AdvancedRow()
    {
        SetScore(score + 10);
    }

    public void HomeOccupied()
    {
        frogger.gameObject.SetActive(false);

        int bonusPoints = time * 20;
        SetScore(score + bonusPoints);

        AddCoins(10);

        if (Cleared())
        {
            SetLives(lives + 1);
            SetScore(score + 1000);
            AddCoins(50);

            Invoke(nameof(NewLevel), 1f);
        }
        else
        {
            Invoke(nameof(Respawn), 1f);
        }
    }

    private bool Cleared()
    {
        for (int i = 0; i < homes.Length; i++)
            if (!homes[i].enabled)
                return false;

        return true;
    }

    private void SetScore(int value)
    {
        score = value;
        scoreText.text = score.ToString();
    }

    private void SetLives(int value)
    {
        lives = value;
        livesText.text = lives.ToString();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinsUI();
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save(); // IMPORTANTE
    }

    private void UpdateCoinsUI()
    {
        if (coinsText != null)
            coinsText.text = coins.ToString();
    }

    // Guardar los datos y enviarlos a MongoDB
    public async void SaveGameToJSONAndSendToMongoDB()
    {
        // Crear el objeto PlayerData
        PlayerData data = new PlayerData
        {
            coins = this.coins,
            score = this.score,
            currentSkin = FindObjectOfType<ImageChanger>()?.currentIndex ?? 0,
            lastSave = System.DateTime.Now.ToString()
        };

        // Convertir los datos a formato JSON
        string json = JsonUtility.ToJson(data, true);

        // Guardar el JSON en un archivo
        string path = Path.Combine(Application.persistentDataPath, "player_save.json");
        File.WriteAllText(path, json);
        Debug.Log("¡Archivo JSON creado en: " + path);

        // Enviar los datos a MongoDB
        if (mongoDbConnector != null)
        {
            await mongoDbConnector.EnviarDatosAMongoDB(data); // Enviar directamente el objeto playerData
        }
        else
        {
            Debug.LogError("El componente ConnectMongoDB no está asignado correctamente.");
        }
    }
}