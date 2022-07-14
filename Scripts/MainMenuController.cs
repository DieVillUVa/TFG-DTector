using UnityEngine;
using UnityEngine.SceneManagement;

public enum NivelSeleccionado { FACIL, MEDIO, DIFICIL };
public class MainMenuController : MonoBehaviour
{

    static MainMenuController controlador;

    private NivelSeleccionado nivel;

    private void Awake()
    {
        if (controlador == null)
        {
            controlador = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (controlador != this)
            Destroy(gameObject);
    }

    public void CambioEscena(string seleccion)
    {
        switch (seleccion)
        {
            case "FACIL":
                nivel = NivelSeleccionado.FACIL;
                break;

            case "MEDIO":
                nivel = NivelSeleccionado.MEDIO;
                break;

            case "DIFICIL":
                nivel = NivelSeleccionado.DIFICIL;
                break;
        }
        SceneManager.LoadScene("FightScene");
    }

    public NivelSeleccionado GetNivelSeleccionado()
    {
        return nivel;
    }
}
