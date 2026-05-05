using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    public Text textoDeLaTabla;
    public ConnectMongoDB mongoScript;

    // Esta función debe llamarse así para que GameManager no de error
    public async void ActualizarYMostrarTabla()
    {
        if (textoDeLaTabla == null || mongoScript == null) return;

        textoDeLaTabla.text = "CARGANDO TOP 10...";
        // Llamamos a la función que acabamos de crear en ConnectMongoDB
        string datos = await mongoScript.ObtenerTop10Arcade();
        textoDeLaTabla.text = datos;
    }

    // Añadimos esta como "alias" para que no falle si usas otro nombre
    public void MostrarRanking()
    {
      
        ActualizarYMostrarTabla();
    }
}