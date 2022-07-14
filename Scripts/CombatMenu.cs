using UnityEngine;
using UnityEngine.UI;

public class CombatMenu : MonoBehaviour
{
    [SerializeField] private Button botonAnterior;
    [SerializeField] private Button botonSiguiente;
    private int itemActual;
    private int ultimoClick;

    private void SeleccionMenu(int _index)
    {
        if (_index == transform.childCount)
        {
            _index = 0;
            itemActual = 0;
        }
        if (_index == -1)
        {
            _index = transform.childCount - 1;
            itemActual = transform.childCount -1;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == _index);
        }
    }

    private void CambioItemMenu(int _change)
    {
        ultimoClick = _change;
        itemActual += _change;
        SeleccionMenu(itemActual);
    }

    public int GetClick()
    {
        return ultimoClick;
    }
}
