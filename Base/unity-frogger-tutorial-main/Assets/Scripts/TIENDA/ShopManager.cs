using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public int coins = 100;
    public Text coinsText;

    public void BuyItem(int price)
    {
        if (coins >= price)
        {
            coins -= price;
            UpdateUI();
            // Lógica para añadir el ítem al inventario
        }
        else
        {
            Debug.Log("No tienes suficiente dinero");
        }
    }

    void UpdateUI()
    {
        coinsText.text = "Monedas: " + coins;
    }
}
