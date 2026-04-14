using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
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
    [SerializeField] private Text coinsText; // 🪙 NUEVO

    public int lives { get; private set; } = 3;
    public int score { get; private set; } = 0;
    public int time { get; private set; } = 30;

    public int coins { get; private set; } = 0; // 🪙 NUEVO

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }

        // 🪙 Cargar monedas guardadas (opcional)
        coins = PlayerPrefs.GetInt("Coins", 0);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
        UpdateCoinsUI(); // 🪙 actualizar UI al iniciar
    }

    private void NewGame()
    {
        gameOverMenu.SetActive(false);

        SetScore(0);
        SetLives(3);
        NewLevel();
    }

    private void NewLevel()
    {
        for (int i = 0; i < homes.Length; i++)
        {
            homes[i].enabled = false;
        }

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
        {
            Invoke(nameof(Respawn), 1f);
        }
        else
        {
            Invoke(nameof(GameOver), 1f);
        }
    }

    private void GameOver()
    {
        frogger.gameObject.SetActive(false);
        gameOverMenu.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(CheckForPlayAgain());
    }

    private IEnumerator CheckForPlayAgain()
    {
        bool playAgain = false;

        while (!playAgain)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                playAgain = true;
            }

            yield return null;
        }

        NewGame();
    }

    public void AdvancedRow()
    {
        SetScore(score + 10);
    }

    public void HomeOccupied()
    {
        frogger.gameObject.SetActive(false);

        int bonusPoints = time * 20;
        SetScore(score + bonusPoints + 0);

        AddCoins(10); // 🪙 monedas por llegar a casa

        if (Cleared())
        {
            SetLives(lives + 1);
            SetScore(score + 1000);

            AddCoins(50); // 🪙 bonus por completar nivel

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
        {
            if (!homes[i].enabled)
            {
                return false;
            }
        }

        return true;
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = lives.ToString();
    }

    // 🪙 SISTEMA DE MONEDAS
    private void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinsUI();

        // guardar monedas
        PlayerPrefs.SetInt("Coins", coins);
    }

    private void UpdateCoinsUI()
    {
        if (coinsText != null)
        {
            coinsText.text = coins.ToString();
        }
    }
}