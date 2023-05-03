using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingGlow : MonoBehaviour
{

    public Image image;
    public float buttonGlowTransitionTime = 2f, buttonGlowHoldTime = 0.5f;
    public bool isGlowing = false;

    public void StartGlow()
    {
        isGlowing = true;
        StartCoroutine(ButtonGlowCoroutine());
    }

    public void StopGlow()
    {
        isGlowing = false;
    }

    private IEnumerator ButtonGlowCoroutine()
    {   
        Color fullColor = image.color;
        Color transColor = new Color(fullColor.r, fullColor.g, fullColor.b, 0f);
        Debug.Log(string.Format("Full: {0} || Trans: {1}", fullColor.ToString(), transColor.ToString()));
        while (isGlowing)
        {
            float deltaTime = 0f;
            while (deltaTime < buttonGlowTransitionTime && isGlowing)
            {
                image.color = Color.Lerp(transColor, fullColor, deltaTime / buttonGlowTransitionTime);
                Debug.Log(string.Format("Image 1: {0}", image.color.ToString()));
                deltaTime += Time.deltaTime;
                yield return null;
            }
            deltaTime = 0f;
            while (deltaTime < buttonGlowHoldTime && isGlowing)
            {
                deltaTime += Time.deltaTime;
                yield return null;
            }
            deltaTime = 0f;
            while (deltaTime < buttonGlowTransitionTime && isGlowing)
            {
                image.color = Color.Lerp(fullColor, transColor, deltaTime / buttonGlowTransitionTime);
                Debug.Log(string.Format("Image 1: {0}", image.color.ToString()));
                deltaTime += Time.deltaTime;
                yield return null;
            }
            deltaTime = 0f;
            while (deltaTime < buttonGlowHoldTime && isGlowing)
            {
                deltaTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}