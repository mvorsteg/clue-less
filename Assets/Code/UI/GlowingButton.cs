using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GlowingButton : MonoBehaviour
{
    public Sprite glowSprite, selectedSprite;
    public Color normalColor = new Color(0xFF, 0xFF, 0xFF);
    public Color highlightedColor = new Color(0xF5, 0xF5, 0xF5);
    public Color pressedColor = new Color(0xC8, 0xC8, 0xC8);
    public Color selectedColor = new Color(0xF5, 0xF5, 0xF5);

    private Color glowColor, transparentColor;

    public float buttonGlowTransitionTime = 2f, buttonGlowHoldTime = 0.5f;

    public Image glowOverlay;

    private bool isInteractable = false;
    private bool isSelected = false;

    public UnityEvent onClick;

    private bool ShouldGlow
    {
        get { return isInteractable && !isSelected; }
    }

    private void Awake()
    {
        glowColor = glowOverlay.color;
        transparentColor = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);

        glowOverlay.color = transparentColor;
    }

    public void SetInteractable(bool value)
    {
        isInteractable = value;
        if (isInteractable)
        {
            StartCoroutine(ButtonGlowCoroutine());
        }
        else
        {
            isSelected = false;
            glowOverlay.sprite = glowSprite;
            glowOverlay.color = transparentColor;
        }
    }

    public void OnPointerEnter()
    {
        if (isInteractable)
        {
            isSelected = true;
            glowOverlay.sprite = selectedSprite;
            glowOverlay.color = glowColor;
        }
    }

    public void OnPointerExit()
    {
        if (isInteractable)
        {
            isSelected = false;
            glowOverlay.sprite = glowSprite;
            glowOverlay.color = transparentColor;
        }
    }

    public void OnPointerClick()
    {
        if (isInteractable)
        {   
            onClick.Invoke();
            SetInteractable(false);
        }
    }

    private IEnumerator ButtonGlowCoroutine()
    {   
        while (isInteractable)
        {
            float deltaTime = 0f;
            while (deltaTime < buttonGlowTransitionTime)
            {
                if (ShouldGlow)
                {
                    glowOverlay.color = Color.Lerp(transparentColor, glowColor, deltaTime / buttonGlowTransitionTime);
                    //Debug.Log(string.Format("Image 1: {0}", image.color.ToString()));
                }
                deltaTime += Time.deltaTime;
                yield return null;
            }
            deltaTime = 0f;
            while (deltaTime < buttonGlowHoldTime)
            {
                deltaTime += Time.deltaTime;
                yield return null;
            }
            deltaTime = 0f;
            while (deltaTime < buttonGlowTransitionTime)
            {
                if (ShouldGlow)
                {
                    glowOverlay.color = Color.Lerp(glowColor, transparentColor, deltaTime / buttonGlowTransitionTime);
                    //Debug.Log(string.Format("Image 1: {0}", image.color.ToString()));
                }
                deltaTime += Time.deltaTime;
                yield return null;
            }
            deltaTime = 0f;
            while (deltaTime < buttonGlowHoldTime)
            {
                deltaTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}