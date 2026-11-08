using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject BotonJ;
    public GameObject BotonS;
    public GameObject BotonM;

    private void Start()
    { 


    }

    public void StartGame()
    {

        Time.timeScale = 1f;
        SceneManager.LoadScene("Nivel");
    }

    public void ExitGame()
    {

        Application.Quit();
    }

    public void IrMenu()
    {

        SceneManager.LoadScene("Menu");
    }
}

