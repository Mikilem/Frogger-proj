using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class SkinItem
    {
        public string nombre;
        public Sprite spriteVisual;
        public int precio;
        public bool esComprada; // Para saber si ya la tiene
    }

    public SkinItem[] listaDeSkins; // Llénalo en el Inspector
    public Image displayImage;      // El componente Image de la tienda
    public Text precioText;         // Texto para el precio
    public Text nombreText;         // Texto para el nombre
    public Button buyButton;
    public Text coinsText;

    private int currentIndex = 0;

    [System.Serializable]
    public class SkinData
    {
        public string nombre;
        public int precio;
        public Sprite idle;
        public Sprite leap;
        public Sprite dead;
    }
    void Start()
    {
        // Cargar cuál skin tenía puesta la última vez
        currentIndex = PlayerPrefs.GetInt("SkinEquipada", 0);
        UpdateUI();

        buyButton.onClick.AddListener(ConfirmarSeleccion);
    }

    public void NextSkin()
    {
        currentIndex = (currentIndex + 1) % listaDeSkins.Length;
        UpdateUI();
    }

    public void PrevSkin()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = listaDeSkins.Length - 1;
        UpdateUI();
    }

    void UpdateUI()
    {
        SkinItem skin = listaDeSkins[currentIndex];
        displayImage.sprite = skin.spriteVisual;
        nombreText.text = skin.nombre;

        // Si el precio es 0, es la skin por defecto o ya comprada
        precioText.text = skin.precio <= 0 ? "EQUIPAR" : "PRECIO: " + skin.precio;

        if (GameManager.Instance != null)
            coinsText.text = GameManager.Instance.coins.ToString();
    }

    public void ConfirmarSeleccion()
    {
        SkinItem skin = listaDeSkins[currentIndex];

        // Lógica de compra
        if (GameManager.Instance.coins >= skin.precio)
        {
            if (skin.precio > 0)
            {
                GameManager.Instance.AddCoins(-skin.precio);
                skin.precio = 0; // Marcamos como comprada esta sesión
            }

            // GUARDAR SELECCIÓN
            PlayerPrefs.SetInt("SkinEquipada", currentIndex);
            PlayerPrefs.Save();
            UpdateUI();
            Debug.Log("Skin " + skin.nombre + " equipada!");
        }
        else
        {
            Debug.Log("No tienes dinero suficiente");
        }
    }
}