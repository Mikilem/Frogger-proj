using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameManager gameManager;


    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}