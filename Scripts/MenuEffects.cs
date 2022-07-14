using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEffects : MonoBehaviour
{
    [SerializeField] private Vector2 posicionFinal;
    private GameObject canvas;
    private Vector2 posicionDerecha = new Vector2(-2000, 0);
    private Vector2 posicionIzquierda = new Vector2(2000, 0);

    private void Update()
    {
        transform.position = Vector2.Lerp(transform.position, posicionFinal, 0.5f);
    }

    private void OnDisable()
    {
        transform.position = posicionFinal;
    }

    private void OnEnable()
    {
        canvas = GameObject.Find("Main Camera").transform.Find("Canvas").gameObject;
        int click = canvas.GetComponent<CombatMenu>().GetClick();
        if (click == 1)
        {
            transform.position = posicionDerecha;
        }
        if (click == -1)
        {
            transform.position = posicionIzquierda;
        }
    }
}
