using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraDetection : MonoBehaviour
{
    private const float COLOR_OSCURO = 0.07f;
    private const int PORCENTAJE_ACEPTABLE = 85;
    private float [] valorColorMedioInicio;
    private Texture2D primeraImg;
    private Texture2D imagenReferencia;
    private bool camaraDetectada;
    private WebCamTexture camara;

    private bool ataqueListo = false;
    private int[] codigoAtaque;

    [SerializeField] private GameObject detection;
    [SerializeField] private GameObject countdown;

    void Start()
    {
        codigoAtaque = new int[] { -1, -1, -1 };
        WebCamDevice[] camaras = WebCamTexture.devices;
        if (camaras.Length == 0)
        {
            camaraDetectada = false;
            return;
        }
        camaraDetectada = true;
        camara = new WebCamTexture();
        camara.Play();
        StartCoroutine("CuentaAtras");
    }

    private IEnumerator CuentaAtras()
    {
        Animator animacionCuentaAtras = countdown.GetComponent<Animator>();
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSecondsRealtime(0.7f);
            animacionCuentaAtras.SetTrigger("Count");
        }
        countdown.SetActive(false);
        detection.SetActive(true);
        StartCoroutine("DeteccionMovimiento");
    }
    private IEnumerator DeteccionMovimiento()
    {
        Animator animacionDetector = detection.GetComponent<Animator>();
        Stopwatch crono = new Stopwatch();
        
        for (int i = 0; i < 3; i++)
        {
            bool mano = false;
            imagenReferencia = new Texture2D(camara.width, camara.height);
            primeraImg = imagenReferencia;
            primeraImg.SetPixels(camara.GetPixels());
            primeraImg.Apply();
            valorColorMedioInicio = ColorMedioCamara(primeraImg);
            yield return new WaitForSecondsRealtime(0.4f);
            animacionDetector.ResetTrigger("Ciclo");
            crono.Start();
            while (crono.Elapsed < TimeSpan.FromSeconds(1.7))
            {
                Texture2D segundaImg = new Texture2D(camara.width, camara.height);
                segundaImg.SetPixels(camara.GetPixels());
                segundaImg.Apply();
                if (mano == true)
                {
                    if (CambioFase2(primeraImg, segundaImg))
                    {
                        codigoAtaque[i] = 1;
                        yield return new WaitForSecondsRealtime(0.1f);
                        break;
                    }
                }
                if (mano == false) {
                    if (CambioFase(primeraImg, segundaImg))
                    {
                        mano = true;
                        yield return new WaitForSecondsRealtime(0.3f);
                    }
            }
                primeraImg = segundaImg;
            }
            crono.Reset();
            if (codigoAtaque[i] != 1)
            {
                codigoAtaque[i] = 0;
            }
            animacionDetector.SetTrigger("Ciclo");
            yield return new WaitForSecondsRealtime(0.2f);
        }
            ataqueListo = true;
        yield return null;
    }
    private void GenerarTipoAtaque(int[] codigo)
    {
        GameObject GOcontroladorCombate = GameObject.Find("ControladorFightScene");
        FightController controladorCombate = GOcontroladorCombate.GetComponent<FightController>();
        if (!camaraDetectada)
        {
            controladorCombate.SetAtaqueAliado("RANDOM");
            return;
        }
        if (codigo[0] == 1)
        {
            if (codigo[1] == 1)
            {
                if (codigo[2] == 1)
                {
                    controladorCombate.SetAtaqueAliado("ENERGY");
                    return;
                }
                else
                {
                    controladorCombate.SetAtaqueAliado("ABILITY");
                    return;
                }
            }
            else
            {
                if (codigo[2] == 1)
                {
                    controladorCombate.SetAtaqueAliado("RUSH");
                    return;
                }
            }
        }
        controladorCombate.SetAtaqueAliado("RANDOM");
    }

    private bool CamaraTapada(Texture2D img)
    {
        int pixelsCorrectos = 0;
        Color[] pixels = img.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r < COLOR_OSCURO && pixels[i].g < COLOR_OSCURO && pixels[i].b < COLOR_OSCURO)
            {
                pixelsCorrectos++;
            }
            int porcentaje = (100 * pixelsCorrectos) / pixels.Length;
            if (porcentaje > PORCENTAJE_ACEPTABLE)
            {
                return true;
            }
        }
        return false;
    }
    private float [] ColorMedioCamara(Texture2D img)
    {
        float[] mediaColor = { 0, 0, 0 };
        Color[] pixels = img.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r += mediaColor[0];
            pixels[i].g += mediaColor[1];
            pixels[i].b += mediaColor[2];
        }
        return mediaColor;
    }

    private float [][] MinMaxDifCamara(Texture2D img)
    {
        float[] diferenciaColor = { 0, 0, 0 };
        float[] minColor = { 1, 1, 1 };
        float[] maxColor = { 0, 0, 0 };
        Color[] pixels = img.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r > maxColor[0])
            {
                maxColor[0] = pixels[i].r;
            }
            if (pixels[i].r < maxColor[0])
            {
                minColor[0] = pixels[i].r;
            }
            if (pixels[i].g > maxColor[1])
            {
                maxColor[1] = pixels[i].g;
            }
            if (pixels[i].g < maxColor[1])
            {
                minColor[1] = pixels[i].g;
            }
            if (pixels[i].b > maxColor[2])
            {
                maxColor[2] = pixels[i].b;
            }
            if (pixels[i].b < maxColor[2])
            {
                minColor[2] = pixels[i].b;
            }
        }
        diferenciaColor[0] = maxColor[0] - minColor[0];
        diferenciaColor[1] = maxColor[1] - minColor[1];
        diferenciaColor[2] = maxColor[2] - minColor[2];
        float[][] minMaxDif = { minColor, maxColor, diferenciaColor };
        return minMaxDif;
    }

    private bool CambioFase(Texture2D img1, Texture2D img2)
    {
        float[][] valores1 = MinMaxDifCamara(imagenReferencia);
        float[][] valores2 = MinMaxDifCamara(img2);
        if (valores2[2][0] < valores1[2][0] && valores2[2][1] < valores1[2][1] && valores2[2][2] < valores1[2][2])
        {
            if (CamaraTapada(img2))
            {
                if (Math.Abs(valores1[2][0] - valores2[2][0]) > COLOR_OSCURO && Math.Abs(valores1[2][1] - valores2[2][1]) > COLOR_OSCURO && Math.Abs(valores1[2][2] - valores2[2][2]) > COLOR_OSCURO)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CambioFase2(Texture2D img1, Texture2D img2)
    {
        float[] colorMedio2 = ColorMedioCamara(img2);
        if (Math.Abs(colorMedio2[0]-valorColorMedioInicio[0]) < COLOR_OSCURO && Math.Abs(colorMedio2[1] - valorColorMedioInicio[1]) < COLOR_OSCURO && Math.Abs(colorMedio2[2] - valorColorMedioInicio[2]) < COLOR_OSCURO)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        if (ataqueListo)
        {
            GenerarTipoAtaque(codigoAtaque);
            SceneManager.LoadScene("FightScene");
        }
    }
}
