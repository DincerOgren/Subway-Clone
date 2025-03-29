using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] TextMeshPro countdownText;
    [SerializeField] float countdwonTime = 3f;

    public void PauseGaem()
    {
        Time.timeScale = 0;
    }
    public void ResumeButton()
    {
        // CountdownRutin
        StartCoroutine(Countdown());
    }

  
    IEnumerator Countdown()
    {
        float a = 0;
        float cdValue = countdwonTime;
        countdownText.text = cdValue.ToString();
        countdownText.gameObject.SetActive(true);
        while (true)
        {

            if (a >= 1)
            {
                a = 0;
                cdValue -= 1;
                if (cdValue <= 0)
                {
                    Time.timeScale = 1;
                    countdownText.gameObject.SetActive(false);
                    break;
                }
                countdownText.text = cdValue.ToString();
                Color c = countdownText.color;
                c.a = 1;
                countdownText.color = c;

               
                print("Text kapandý acýldý ");
            }
            StartCoroutine(FadeOutText(.7f));

            print("A " + a);
            print("cdVal  + " + cdValue);
            a += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    IEnumerator FadeOutText(float duration)
    {
        Color colorVal = countdownText.color; 
        float elapsedTime = 0f;
        float startAlpha = colorVal.a;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime; 
            float t = elapsedTime / duration; 
            colorVal.a = Mathf.Lerp(startAlpha, 0, t); 
            countdownText.color = colorVal; 
            yield return null; 
        }

        // Ensure alpha is set to 0 at the end
        colorVal.a = 0;
        countdownText.color = colorVal;
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}