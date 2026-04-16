using UnityEngine;
using UnityEngine.UI;  // Necesario para trabajar con UI

public class ShopManager : MonoBehaviour
{
    public int coins = 100;           // Monedas del jugador
    public Text coinsText;            // Referencia al UI de texto que muestra las monedas
    public Button buyButton;          // El botón de compra
    public int itemPrice = 10;        // Precio del ítem que el jugador comprará

    void Start()
    {
        // Asegúrate de que el botón esté configurado
        buyButton.onClick.AddListener(BuyItem);  // Al hacer clic en el botón, se ejecutará la función BuyItem()
        UpdateUI();  // Actualiza el UI al comenzar
    }

    public void BuyItem()
    {
        // Verifica si el jugador tiene suficiente dinero para comprar el ítem
        if (coins >= itemPrice)
        {
            coins -= itemPrice;  // Resta el precio del ítem de las monedas del jugador
            UpdateUI();           // Actualiza el UI para reflejar las monedas restantes
            Debug.Log("¡Compra exitosa!"); // Mensaje de depuración, puedes agregar más lógica aquí para la compra
        }
        else
        {
            Debug.Log("No tienes suficientes monedas para comprar este ítem.");
        }
    }

    void UpdateUI()
    {
        // Actualiza el texto de monedas en la UI
        coinsText.text = "" + coins;
    }
}