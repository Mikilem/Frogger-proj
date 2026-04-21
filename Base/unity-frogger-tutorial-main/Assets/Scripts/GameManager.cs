using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }



    [SerializeField] private Home[] homes;
    [SerializeField] private Frogger frogger;
    [SerializeField] private GameObject gameOverMenu;



    [Header("MongoDB Config")]
    [SerializeField] private string mongoUrl = "mongodb+srv://a23sersansan_db_user:<db_password>@cluster0.qp4mqk8.mongodb.net/?appName=Cluster0";
    [SerializeField] private string apiKey = "al-5YpeV67mL_wvk-kaa-yoASlZbv-D-s6MeyWfrYJYSE-";
    [SerializeField] private string clusterName = "Cluster0";
    [SerializeField] private string databaseName = "GameDB";
    [SerializeField] private string collectionName = "Players";



    [Header("UI")]
    [SerializeField] private Text timeText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text coinsText;

    public int lives { get; private set; } = 3;
    public int score { get; private set; } = 0;
    public int time { get; private set; } = 30;
    public int coins { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // cargar monedas
        coins = PlayerPrefs.GetInt("Coins", 0);
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

        UpdateCoinsUI(); // importante

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

    // LLAMADA IMPORTANTE:
    SaveGameToJSON(); 

    StopAllCoroutines();
    StartCoroutine(CheckForPlayAgain());
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

        AddCoins(10); //  monedas por llegar a casa

        if (Cleared())
        {
            SetLives(lives + 1);
            SetScore(score + 1000);

            AddCoins(50); //  bonus nivel

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

    // SISTEMA DE MONEDAS FIX
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





    public void SaveGameToJSON()
    {
        // 1. Creamos el contenedor de datos
        PlayerData data = new PlayerData();
        data.coins = this.coins;
        data.score = this.score;

        // 2. BUSQUEDA SEGURA: Intentamos buscar el ImageChanger
        ImageChanger scriptImagen = FindObjectOfType<ImageChanger>();

        if (scriptImagen != null)
        {
            // Si lo encuentra, guardamos su índice
            data.currentSkin = scriptImagen.currentIndex;
        }
        else
        {
            // Si NO lo encuentra (porque el objeto está desactivado o no existe),
            // guardamos un valor por defecto (0) y avisamos en consola
            data.currentSkin = 0;
            Debug.LogWarning("No se encontró ImageChanger en la escena. Se guardó skin 0 por defecto.");
        }

        data.lastSave = System.DateTime.Now.ToString();

        // 3. Convertir y guardar
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "player_save.json");

        File.WriteAllText(path, json);

        Debug.Log("¡Éxito! Archivo JSON creado en: " + path);
    }


    public void EnviarArchivoJSONaMongoDB()
    {
        string path = Path.Combine(Application.persistentDataPath, "player_save.json");

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            StartCoroutine(PostToMongoDB(jsonContent));
        }
    }

    IEnumerator PostToMongoDB(string jsonDocument)
    {
        string combinedJson = $"{{\"collection\":\"{collectionName}\",\"database\":\"{databaseName}\",\"dataSource\":\"{clusterName}\",\"document\":{jsonDocument}}}";

        UnityWebRequest request = new UnityWebRequest(mongoUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(combinedJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("¡Datos enviados a MongoDB!");
        else
            Debug.LogError("Error MongoDB: " + request.downloadHandler.text);
    }




    private void Update()
    {
        // Si presionas la tecla S en el teclado, se guardará el juego
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGameToJSON();
        }
    }
}