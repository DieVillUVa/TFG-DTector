using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickableMenuController : MonoBehaviour
{

    private MainMenuController controladorMenu;

    private void Awake()
    {
        GameObject GOcontroladorMenu = GameObject.Find("ControladorMainMenu");
        controladorMenu = GOcontroladorMenu.GetComponent<MainMenuController>();
    }
    public void SeleccionNivel(string nivel)
    {
        controladorMenu.CambioEscena(nivel);
    }
}
