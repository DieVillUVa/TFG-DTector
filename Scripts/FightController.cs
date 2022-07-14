using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum EstadoCombate { INICIO, INPUT, COMBATE, VICTORIA, DERROTA};
public enum TipoAtaque { ENERGY, RUSH, ABILITY, HEAL };
public class FightController : MonoBehaviour
{
    static FightController controlador;

    private TipoAtaque ataqueAliado;
    private TipoAtaque ataqueEnemigo;
    private EstadoCombate estado;
    public GameObject aliado;
    private GameObject enemigo;
    public GameObject enemigo1;
    public GameObject enemigo2;
    public GameObject enemigo3;
    public GameObject camara;
    public GameObject interfazVida;

    private Vector3 POSICION_ALIADO = new Vector3(400, 0, -10);
    private Vector3 POSICION_ENEMIGO = new Vector3(-400, 0, -10);
    private Vector3 POSICION_INICIAL = new Vector3(0, 0, -10);
    private Vector3 POSICION_ALIADO_SPRITE;
    private Vector3 POSICION_ENEMIGO_SPRITE;
    private Vector3 POSICION_ALIADO_PROYECTIL;
    private Vector3 POSICION_ENEMIGO_PROYECTIL;
    private CharacterStats estadisticasAliado;
    private CharacterStats estadisticasEnemigo;
    private Animator animacionAliado;
    private Animator animacionEnemigo;
    private GameObject proyectilAliado = null;
    private GameObject proyectilEnemigo = null;
    private int golpe;
    private bool curaUsada = false;
    private bool animacionCompletada;

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
    void Start()
    {
        estado = EstadoCombate.INICIO;
        CargarEnemigo();
        GameObject GOAliado = Instantiate(aliado);
        aliado = GOAliado;
        DontDestroyOnLoad(aliado);
        DontDestroyOnLoad(enemigo);
        POSICION_ALIADO_SPRITE = aliado.transform.position;
        POSICION_ENEMIGO_SPRITE = enemigo.transform.position;
        InstancializarVariables();
        animacionCompletada = false;
        StartCoroutine("ComienzoCombate");
    }

    private void CargarEnemigo()
    {
        GameObject GOcontroladorMainMenu = GameObject.Find("ControladorMainMenu");
        MainMenuController controladorMainMenu = GOcontroladorMainMenu.GetComponent<MainMenuController>();
        switch (controladorMainMenu.GetNivelSeleccionado())
        {
            case NivelSeleccionado.FACIL:
                enemigo = Instantiate(enemigo1);
                break;

            case NivelSeleccionado.MEDIO:
                enemigo = Instantiate(enemigo2);
                break;

            case NivelSeleccionado.DIFICIL:
                enemigo = Instantiate(enemigo3);
                break;
        }
    }

    private void InstancializarVariables()
    {
        estadisticasAliado = aliado.GetComponent<CharacterStats>();
        animacionAliado = aliado.transform.GetChild(0).GetComponent<Animator>();
        
        estadisticasEnemigo = enemigo.GetComponent<CharacterStats>();
        animacionEnemigo = enemigo.transform.GetChild(0).GetComponent<Animator>();
    }
    private IEnumerator ComienzoCombate()
    {
        camara.transform.position = POSICION_ENEMIGO;
        yield return new WaitForSecondsRealtime(0.5f);
        for (int i = 0; i < 3; i++)
        {
            animacionEnemigo.SetBool("AtaqueRush", true);
            yield return new WaitForSecondsRealtime(0.5f);
            animacionEnemigo.SetBool("AtaqueRush", false);
            yield return new WaitForSecondsRealtime(0.5f);
        }
        camara.transform.position = POSICION_ALIADO;
        for (int i = 0; i < 3; i++)
        {
            animacionAliado.SetBool("AtaqueRush", true);
            yield return new WaitForSecondsRealtime(0.5f);
            animacionAliado.SetBool("AtaqueRush", false);
            yield return new WaitForSecondsRealtime(0.5f);
        }
        estado = EstadoCombate.INPUT;
        yield return animacionCompletada = true;
    }

    public void SetAtaqueAliado(string tipo)
    {
        switch (tipo)
        {
            case "HEAL":
                ataqueAliado = TipoAtaque.HEAL;
                break;

            case "ENERGY":
                ataqueAliado = TipoAtaque.ENERGY;
                break;

            case "RUSH":
                ataqueAliado = TipoAtaque.RUSH;
                break;

            case "ABILITY":
                ataqueAliado = TipoAtaque.ABILITY;
                break;

            case "RANDOM":
                ataqueAliado = TipoAtaque.ABILITY;
                int ataque = UnityEngine.Random.Range(0, 3);
                if (ataque == 0)
                {
                    ataqueAliado = TipoAtaque.ENERGY;
                }
                if (ataque == 1)
                {
                    ataqueAliado = TipoAtaque.RUSH;
                }
                if (ataque == 2)
                {
                    ataqueAliado = TipoAtaque.ABILITY;
                }
                break;
        }
    }

    private void RestablecerAnimaciones()
    {
        aliado.transform.position = POSICION_ALIADO_SPRITE;
        enemigo.transform.position = POSICION_ENEMIGO_SPRITE;
        animacionAliado.SetBool("AtaqueRush", false);
        animacionAliado.SetBool("AtaqueEnergy", false);
        animacionEnemigo.SetBool("AtaqueRush", false);
        animacionEnemigo.SetBool("AtaqueEnergy", false);
        RestablecerProyectiles(proyectilAliado, proyectilEnemigo);
    }

    private void RestablecerProyectiles(GameObject proyectilA, GameObject proyectilE)
    {
        if (proyectilA != null)
        {
            proyectilA.transform.position = POSICION_ALIADO_PROYECTIL;
            proyectilA.SetActive(false);
        }
        if (proyectilE != null)
        {
            proyectilE.transform.position = POSICION_ENEMIGO_PROYECTIL;
            proyectilE.SetActive(false);
        }
    }

    public void SetAtaqueEnemigo()
    {
        int ataque = UnityEngine.Random.Range(0, 99);
        ataqueEnemigo = enemigo.GetComponent<CharacterStats>().ataqueEnemigo(ataque);
    }

    private IEnumerator ActualizarVida()
    {
        Text texto = interfazVida.transform.GetChild(1).GetComponent<Text>();
        if (camara.transform.position == POSICION_ALIADO)
        {
            texto.text = estadisticasAliado.GetVida().ToString();
            interfazVida.SetActive(true);
            yield return new WaitForSecondsRealtime(1.5f);
            estadisticasAliado.SetVida(golpe);
            texto.text = estadisticasAliado.GetVida().ToString();
            yield return new WaitForSecondsRealtime(1.5f);
            interfazVida.SetActive(false);
        }
        if (camara.transform.position == POSICION_ENEMIGO)
        {
            texto.text = estadisticasEnemigo.GetVida().ToString();
            interfazVida.SetActive(true);
            yield return new WaitForSecondsRealtime(1.5f);
            estadisticasEnemigo.SetVida(golpe);
            texto.text = estadisticasEnemigo.GetVida().ToString();
            yield return new WaitForSecondsRealtime(1.5f);
            interfazVida.SetActive(false);
        }
        if(estadisticasAliado.GetVida() == 0)
        {
            estado = EstadoCombate.DERROTA;
            yield break;
        }
        if (estadisticasEnemigo.GetVida() == 0)
        {
            estado = EstadoCombate.VICTORIA;
            yield break;
        }
        estado = EstadoCombate.INPUT;
        yield return null;
    }

    private IEnumerator ColisionDeDigimon()
    {
        camara.transform.position = POSICION_INICIAL;
        yield return new WaitForSecondsRealtime(1.5f);
        switch (ataqueAliado, ataqueEnemigo)
        {
            case (TipoAtaque.ENERGY, TipoAtaque.RUSH):
                //Generar proyectil y desplazar digimon
                proyectilAliado = aliado.transform.GetChild(2).GetChild(0).gameObject;
                POSICION_ALIADO_PROYECTIL = proyectilAliado.transform.position;
                proyectilAliado.SetActive(true);
                yield return StartCoroutine(Desplazar(proyectilAliado, -10, enemigo, 10, 0.7));
                //Explotar proyectil aliado
                proyectilAliado.SetActive(false);
                yield return StartCoroutine(Desplazar(enemigo, 10, 0.7));
                golpe = estadisticasEnemigo.velocidad;
                RestablecerAnimaciones();
                camara.transform.position = POSICION_ALIADO;
                yield return StartCoroutine("ActualizarVida");
                break;

            case (TipoAtaque.RUSH, TipoAtaque.ENERGY):
                //Generar proyectil y desplazar digimon
                proyectilEnemigo = enemigo.transform.GetChild(2).GetChild(0).gameObject;
                POSICION_ENEMIGO_PROYECTIL = proyectilEnemigo.transform.position;
                proyectilEnemigo.SetActive(true);
                yield return StartCoroutine(Desplazar(aliado, -10, proyectilEnemigo, 10, 0.7));
                //Explotar proyectil enemigo
                proyectilEnemigo.SetActive(false);
                yield return StartCoroutine(Desplazar(aliado, -10, 0.7));
                golpe = estadisticasAliado.velocidad;
                RestablecerAnimaciones();
                camara.transform.position = POSICION_ENEMIGO;
                yield return StartCoroutine("ActualizarVida");
                break;

            case (TipoAtaque.RUSH, TipoAtaque.RUSH):
                //Desplazar digimons
                yield return StartCoroutine(Desplazar(aliado, -10, enemigo, 10, 0.75));
                if (estadisticasAliado.velocidad > estadisticasEnemigo.velocidad)
                {
                    //Desplazar enemigo hacia atras
                    yield return StartCoroutine(Desplazar(aliado, -10, enemigo, -10, 0.65));
                    golpe = estadisticasAliado.velocidad;
                    RestablecerAnimaciones();
                    camara.transform.position = POSICION_ENEMIGO;
                    yield return StartCoroutine("ActualizarVida");
                }
                if (estadisticasAliado.velocidad < estadisticasEnemigo.velocidad)
                {
                    //Desplazar aliado hacia atras
                    yield return StartCoroutine(Desplazar(aliado, 10, enemigo, 10, 0.65));
                    golpe = estadisticasEnemigo.velocidad;
                    RestablecerAnimaciones();
                    camara.transform.position = POSICION_ALIADO;
                    yield return StartCoroutine("ActualizarVida");
                }
                if (estadisticasAliado.velocidad == estadisticasEnemigo.velocidad)
                {
                    //Desplazar ambos hacia atras
                    yield return StartCoroutine(Desplazar(aliado, 10, enemigo, -10, 0.65));
                    RestablecerAnimaciones();
                    yield return estado = EstadoCombate.INPUT;
                }
                break;

            case (TipoAtaque.RUSH, TipoAtaque.ABILITY):
                //Generar proyectil y desplazar digimon
                proyectilEnemigo = enemigo.transform.GetChild(1).GetChild(0).gameObject;
                POSICION_ENEMIGO_PROYECTIL = proyectilEnemigo.transform.position;
                proyectilEnemigo.SetActive(true);
                yield return StartCoroutine(Desplazar(aliado, -10, proyectilEnemigo, 10, 0.7));
                //Solapar habilidad sobre digimon
                yield return StartCoroutine(Desplazar(proyectilEnemigo, 10, 0.65));
                golpe = estadisticasEnemigo.tecnica;
                RestablecerAnimaciones();
                camara.transform.position = POSICION_ALIADO;
                yield return StartCoroutine("ActualizarVida");
                break;

            case (TipoAtaque.ABILITY, TipoAtaque.RUSH):
                //Generar proyectil y desplazar digimon
                proyectilAliado = aliado.transform.GetChild(1).GetChild(0).gameObject;
                POSICION_ALIADO_PROYECTIL = proyectilAliado.transform.position;
                proyectilAliado.SetActive(true);
                yield return StartCoroutine(Desplazar(proyectilAliado, -10, enemigo, 10, 0.7));
                //Solapar habilidad sobre digimon
                yield return StartCoroutine(Desplazar(proyectilAliado, -10, 0.65));
                golpe = estadisticasAliado.tecnica;
                RestablecerAnimaciones();
                camara.transform.position = POSICION_ENEMIGO;
                yield return StartCoroutine("ActualizarVida");
                break;
        }
        yield return null;
    }

    private IEnumerator ColisionDeProyectiles()
    {
        camara.transform.position = POSICION_INICIAL;
        yield return new WaitForSecondsRealtime(1.5f);
        switch (ataqueAliado, ataqueEnemigo)
        {
            case (TipoAtaque.ENERGY, TipoAtaque.ENERGY):
                //Generar proyectiles
                proyectilAliado = aliado.transform.GetChild(2).GetChild(0).gameObject;
                proyectilEnemigo = enemigo.transform.GetChild(2).GetChild(0).gameObject;
                POSICION_ALIADO_PROYECTIL = proyectilAliado.transform.position;
                POSICION_ENEMIGO_PROYECTIL = proyectilEnemigo.transform.position;
                proyectilAliado.SetActive(true);
                proyectilEnemigo.SetActive(true);
                yield return StartCoroutine(Desplazar(proyectilAliado, -10, proyectilEnemigo, 10, 0.8));
                if (estadisticasAliado.fuerza > estadisticasEnemigo.fuerza)
                {
                    //Explotar proyectil enemigo
                    proyectilEnemigo.SetActive(false);
                    yield return StartCoroutine(Desplazar(proyectilAliado, -10, 0.8));
                    golpe = estadisticasAliado.fuerza;
                    RestablecerAnimaciones();
                    camara.transform.position = POSICION_ENEMIGO;
                    yield return StartCoroutine("ActualizarVida");
                }
                if (estadisticasAliado.fuerza < estadisticasEnemigo.fuerza)
                {
                    //Explotar proyectil aliado
                    proyectilAliado.SetActive(false);
                    yield return StartCoroutine(Desplazar(proyectilEnemigo, 10, 0.8));
                    golpe = estadisticasEnemigo.fuerza;
                    RestablecerAnimaciones();
                    camara.transform.position = POSICION_ALIADO;
                    yield return StartCoroutine("ActualizarVida");
                }
                if (estadisticasAliado.fuerza == estadisticasEnemigo.fuerza)
                {
                    //Explotar ambos proyectiles
                    proyectilAliado.SetActive(false);
                    proyectilEnemigo.SetActive(false);
                    RestablecerAnimaciones();
                    yield return estado = EstadoCombate.INPUT;
                }
                break;

            case (TipoAtaque.ENERGY, TipoAtaque.ABILITY):
                //Generar proyectiles
                proyectilAliado = aliado.transform.GetChild(2).GetChild(0).gameObject;
                proyectilEnemigo = enemigo.transform.GetChild(1).GetChild(0).gameObject;
                POSICION_ALIADO_PROYECTIL = proyectilAliado.transform.position;
                POSICION_ENEMIGO_PROYECTIL = proyectilEnemigo.transform.position;
                proyectilAliado.SetActive(true);
                proyectilEnemigo.SetActive(true);
                yield return StartCoroutine(Desplazar(proyectilAliado, -10, proyectilEnemigo, 10, 0.8));
                //Explotar proyectil enemigo
                proyectilEnemigo.SetActive(false);
                yield return StartCoroutine(Desplazar(proyectilAliado, -10, 0.8));
                golpe = estadisticasAliado.fuerza;
                RestablecerAnimaciones();
                camara.transform.position = POSICION_ENEMIGO;
                yield return StartCoroutine("ActualizarVida");
                break;

            case (TipoAtaque.ABILITY, TipoAtaque.ENERGY):
                //Generar proyectiles
                proyectilAliado = aliado.transform.GetChild(1).GetChild(0).gameObject;
                proyectilEnemigo = enemigo.transform.GetChild(2).GetChild(0).gameObject;
                POSICION_ALIADO_PROYECTIL = proyectilAliado.transform.position;
                POSICION_ENEMIGO_PROYECTIL = proyectilEnemigo.transform.position;
                proyectilAliado.SetActive(true);
                proyectilEnemigo.SetActive(true);
                yield return StartCoroutine(Desplazar(proyectilAliado, -10, proyectilEnemigo, 10, 0.8));
                //Explotar proyectil aliado
                proyectilAliado.SetActive(false);
                yield return StartCoroutine(Desplazar(proyectilEnemigo, 10, 0.8));
                golpe = estadisticasEnemigo.fuerza;
                RestablecerAnimaciones();
                camara.transform.position = POSICION_ALIADO;
                yield return StartCoroutine("ActualizarVida");
                break;

            case (TipoAtaque.ABILITY, TipoAtaque.ABILITY):
                //Generar proyectiles
                proyectilAliado = aliado.transform.GetChild(1).GetChild(0).gameObject;
                proyectilEnemigo = enemigo.transform.GetChild(1).GetChild(0).gameObject;
                POSICION_ALIADO_PROYECTIL = proyectilAliado.transform.position;
                POSICION_ENEMIGO_PROYECTIL = proyectilEnemigo.transform.position;
                proyectilAliado.SetActive(true);
                proyectilEnemigo.SetActive(true);
                yield return StartCoroutine(Desplazar(proyectilAliado, -10, proyectilEnemigo, 10, 0.8));
                if (estadisticasAliado.tecnica > estadisticasEnemigo.tecnica)
                {
                    //Explotar proyectil enemigo
                    proyectilEnemigo.SetActive(false);
                    yield return StartCoroutine(Desplazar(proyectilAliado, -10, 0.8));
                    golpe = estadisticasAliado.tecnica;
                    RestablecerAnimaciones();
                    camara.transform.position = POSICION_ENEMIGO;
                    yield return StartCoroutine("ActualizarVida");
                }
                if (estadisticasAliado.tecnica < estadisticasEnemigo.tecnica)
                {
                    //Explotar proyectil aliado
                    proyectilAliado.SetActive(false);
                    yield return StartCoroutine(Desplazar(proyectilEnemigo, 10, 0.8));
                    golpe = estadisticasAliado.tecnica;
                    RestablecerAnimaciones();
                    camara.transform.position = POSICION_ALIADO;
                    yield return StartCoroutine("ActualizarVida");
                }
                if (estadisticasAliado.tecnica == estadisticasEnemigo.tecnica)
                {
                    //Explotar ambos proyectiles
                    proyectilAliado.SetActive(false);
                    proyectilEnemigo.SetActive(false);
                    RestablecerAnimaciones();
                    yield return estado = EstadoCombate.INPUT;
                }
                break;
        }
        yield return new WaitForSecondsRealtime(2f);
    }

    private IEnumerator GenerarAnimacionAtaque()
    {
        camara.transform.position = POSICION_ALIADO;
        yield return new WaitForSecondsRealtime(0.5f);
        if(ataqueAliado == TipoAtaque.HEAL)
        {
            curaUsada = true;
            golpe = -90;
            yield return StartCoroutine("ActualizarVida");
            animacionCompletada = true;
            yield break;
        }
        if(ataqueAliado != TipoAtaque.RUSH)
        {
            animacionAliado.SetBool("AtaqueEnergy", true);
            yield return new WaitForSecondsRealtime(1.5f);
        }
        else
        {
            animacionAliado.SetBool("AtaqueRush", true);
            //Desplazar sprite
            yield return new WaitForSecondsRealtime(0.15f);
            StartCoroutine(Desplazar(aliado, -10, 1.0));
            yield return new WaitForSecondsRealtime(0.5f);
        }


        camara.transform.position = POSICION_ENEMIGO;
        yield return new WaitForSecondsRealtime(0.5f);
        if (ataqueEnemigo != TipoAtaque.RUSH)
        {
            animacionEnemigo.SetBool("AtaqueEnergy", true);
            yield return new WaitForSecondsRealtime(1.5f);
        }
        else
        {
            animacionEnemigo.SetBool("AtaqueRush", true);
            //Desplazar sprite
            yield return new WaitForSecondsRealtime(0.15f);
            StartCoroutine(Desplazar(enemigo, 10, 1.0));
            yield return new WaitForSecondsRealtime(0.5f);
        }
        camara.transform.position = POSICION_INICIAL;
        if (ataqueAliado == TipoAtaque.RUSH || ataqueEnemigo == TipoAtaque.RUSH)
        {
            yield return StartCoroutine(ColisionDeDigimon());
        }
        else
        {
            yield return StartCoroutine(ColisionDeProyectiles());
        }
        yield return animacionCompletada = true;
    }

    private IEnumerator Desplazar(GameObject objeto, int direccion, double timer)
    {
        Stopwatch crono = new Stopwatch();
        float tiempo = 0;
        crono.Start();
        while (crono.Elapsed < TimeSpan.FromSeconds(timer))
        {
            objeto.transform.position = objeto.transform.position + new Vector3(direccion, 0, 0);
            tiempo += Time.deltaTime;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        crono.Reset();
        yield return new WaitForSecondsRealtime(0.1f);
    }

    private IEnumerator Desplazar(GameObject objeto1, int direccion1, GameObject objeto2, int direccion2, double timer)
    {
        Stopwatch crono = new Stopwatch();
        float tiempo = 0;
        crono.Start();
        while (crono.Elapsed < TimeSpan.FromSeconds(timer))
        {
            objeto1.transform.position = objeto1.transform.position + new Vector3(direccion1, 0, 0);
            objeto2.transform.position = objeto2.transform.position + new Vector3(direccion2, 0, 0);
            tiempo += Time.deltaTime;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        crono.Reset();
        yield return new WaitForSecondsRealtime(0.1f);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("FightScene") && animacionCompletada)
        {
            if (estado == EstadoCombate.INPUT)
            {
                SetAtaqueEnemigo();
                SceneManager.LoadScene("CombatMenu");
                estado = EstadoCombate.COMBATE;
                return;
            }
            if (estado == EstadoCombate.COMBATE)
            {
                camara = GameObject.Find("Main Camera");
                interfazVida = camara.transform.Find("InterfazVida").gameObject;
                InstancializarVariables();
                animacionCompletada = false;
                StartCoroutine("GenerarAnimacionAtaque");
                return;
            }
            if (estado == EstadoCombate.DERROTA || estado == EstadoCombate.VICTORIA)
            {
                AcabarCombate();
                return;
            }
        }
    }
    public void AcabarCombate()
    {
        Destroy(aliado);
        Destroy(enemigo);
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
        return;
    }

    public bool CuraUsada()
    {
        return curaUsada;
    }
}
