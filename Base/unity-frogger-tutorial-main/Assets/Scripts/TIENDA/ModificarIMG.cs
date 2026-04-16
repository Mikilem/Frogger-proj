using UnityEngine;
using UnityEngine.UI;

public class ImageChanger : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image displayImage;  // La imagen que se va a mostrar
    [SerializeField] private Sprite[] images;     // Array de imágenes a mostrar
    [SerializeField] private Button nextButton;   // Botón para siguiente imagen
    [SerializeField] private Button prevButton;   // Botón para imagen anterior

    private int currentIndex = 0;  // Índice de la imagen actual

    void Start()
    {
        // Asegurarse de que haya imágenes para mostrar
        if (images.Length > 0)
        {
            displayImage.sprite = images[currentIndex];  // Asignar la primera imagen al inicio
        }

        // Agregar listeners a los botones
        nextButton.onClick.AddListener(NextImage);
        prevButton.onClick.AddListener(PreviousImage);
    }

    // Método para cambiar a la siguiente imagen
    void NextImage()
    {
        if (currentIndex < images.Length - 1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;  // Vuelve a la primera imagen si se llega al final
        }
        UpdateImage();
    }

    // Método para mostrar la imagen anterior
    void PreviousImage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
        }
        else
        {
            currentIndex = images.Length - 1;  // Vuelve a la última imagen si se está en la primera
        }
        UpdateImage();
    }

    // Método para actualizar la imagen mostrada
    void UpdateImage()
    {
        displayImage.sprite = images[currentIndex];  // Actualizar la imagen mostrada
    }
}