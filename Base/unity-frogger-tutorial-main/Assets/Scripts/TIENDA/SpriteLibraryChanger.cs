using UnityEngine;
using UnityEngine.U2D;  // Necesario para SpriteLibrary
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class SpriteLibraryChanger : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image displayImage;  // La imagen que se va a mostrar
    [SerializeField] private SpriteLibraryAsset spriteLibraryAsset;  // Referencia a la SpriteLibraryAsset
    [SerializeField] private Button nextButton;   // Botón para siguiente imagen
    [SerializeField] private Button prevButton;   // Botón para imagen anterior

    private int currentIndex = 0;  // Índice de la imagen actual
    private string category = "SKIN";  // Nombre de la categoría en SpriteLibrary
    private string[] spriteNames = { "Frogger", "Frogger1", "Frogger2" };  // Los nombres de los sprites

    void Start()
    {
        if (spriteLibraryAsset != null)
        {
            UpdateImage();  // Actualiza la imagen mostrada al inicio

            nextButton.onClick.AddListener(NextImage);
            prevButton.onClick.AddListener(PreviousImage);
        }
        else
        {
            Debug.LogError("Sprite Library Asset no está asignada.");
        }
    }

    void NextImage()
    {
        if (currentIndex < spriteNames.Length - 1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;  // Si llegamos al final, volvemos a la primera imagen
        }
        UpdateImage();
    }

    void PreviousImage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
        }
        else
        {
            currentIndex = spriteNames.Length - 1;  // Si estamos en la primera imagen, vamos a la última
        }
        UpdateImage();
    }

    void UpdateImage()
    {
        // Usamos el nombre del sprite de acuerdo al índice
        string spriteName = spriteNames[currentIndex];
        Sprite newSprite = spriteLibraryAsset.GetSprite(category, spriteName);  // Obtiene el sprite por nombre
        displayImage.sprite = newSprite;  // Actualiza la imagen mostrada en la UI
    }
}