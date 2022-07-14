using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatMenuController : MonoBehaviour
{
    [SerializeField] private Button botonCura;
    private FightController controladorCombate;
    void Awake()
    {
        GameObject GOcontroladorCombate = GameObject.Find("ControladorFightScene");
        controladorCombate = GOcontroladorCombate.GetComponent<FightController>();
        if (controladorCombate.CuraUsada())
        {
            botonCura.interactable = false;
        }
    }

    public void CambioEscena(string escena)
    {
        switch (escena)
        {
            case "Ataque":
                SceneManager.LoadScene("CameraDetection");
            break;

            case "Cura":
                controladorCombate.SetAtaqueAliado("HEAL");
                SceneManager.LoadScene("FightScene");
            break;

            case "Escape":
                controladorCombate.AcabarCombate();
            break;
        }
    }
}
