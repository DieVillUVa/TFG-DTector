using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int vidaMax;
    public int fuerza;
    public int velocidad;
    public int tecnica;
    [SerializeField] private int energyRate;
    [SerializeField] private int rushRate;
    [SerializeField] private int abilityRate;

    private int vida;

    void Start()
    {
        vida = vidaMax;
    }

    public void SetVida(int ataque)
    {
        if (ataque > vida)
        {
            vida = 0;
        }
        else
        {
            vida = vida - ataque;
            if (vida > vidaMax)
            {
                vida = vidaMax;
            }
        }
    }

    public TipoAtaque ataqueEnemigo(int numero)
    {
        if (numero < energyRate)
        {
            return TipoAtaque.ENERGY;
        }
        else
        {
            int rushLimit = energyRate + rushRate;
            if (numero >= energyRate && numero < rushLimit)
            {
                return TipoAtaque.RUSH;
            }
            else
            {
                return TipoAtaque.ABILITY;
            }
        }
    }

    public int GetVida()
    {
        return vida;
    }
}
