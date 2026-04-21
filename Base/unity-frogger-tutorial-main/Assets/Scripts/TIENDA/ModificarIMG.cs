using UnityEngine;
using UnityEngine.UI;

public class ImageChanger : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image displayImage;  // La imagen que se va a mostrar
    [SerializeField] private Sprite[] images;     // Array de imágenes a mostrar
    [SerializeField] private Button nextButton;   // Botón para siguiente imagen
    [SerializeField] private Button prevButton;   // Botón para imagen anterior

    public int currentIndex = 0;  // Índice de la imagen actual

    // Esto permite que otros lean el valor, pero solo este script lo cambie
    // public int CurrentIndex => currentIndex;

    void Start()
    {
        // Validar referencias
        if (displayImage == null || images == null || images.Length == 0 || nextButton == null || prevButton == null)
        {
            Debug.LogError("Faltan referencias en ImageChanger. Revisa el Inspector.");
            return;
        }

        // Asignar la primera imagen
        displayImage.sprite = images[currentIndex];

        //// Limpiar listeners previos y agregar los nuevos
        //nextButton.onClick.RemoveAllListeners();
        //prevButton.onClick.RemoveAllListeners();

    }

    private void OnEnable()
    {
        nextButton.onClick.AddListener(NextImage);
        prevButton.onClick.AddListener(PreviousImage);
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(NextImage);
        prevButton.onClick.RemoveListener(PreviousImage);
    }

    // Método para cambiar a la siguiente imagen
    void NextImage()
    {
        currentIndex = (currentIndex + 1) % images.Length;  // Ciclo circular
        UpdateImage();
    }

    // Método para mostrar la imagen anterior
    void PreviousImage()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = images.Length - 1;  // Ir al último si se pasa del inicio
        UpdateImage();
    }

    // Método para actualizar la imagen mostrada
    void UpdateImage()
    {
        displayImage.sprite = images[currentIndex];
    }
}