using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    public void Jugar ()
    {
        SceneManager.LoadScene("1_Aimar");
    }

    public void Salir ()
    {
        Debug.Log("Boooomboclatt");
        Application.Quit();
    }
}