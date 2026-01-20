using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualFeedbackManager : MonoBehaviour
{
    private static VisualFeedbackManager instance;

    public static VisualFeedbackManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<VisualFeedbackManager>();

                if (instance == null)
                {
                    GameObject go = new GameObject("VisualFeedbackManager");
                    instance = go.AddComponent<VisualFeedbackManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowInvincibilityFeedback(GameObject target, float duration)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        ParticleSystem particles = target.GetComponentInChildren<ParticleSystem>();
        StartCoroutine(InvincibilityEffect(renderers, particles, duration));
    }

    public void ShowSpeedBoostFeedback(GameObject target, float duration)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            StartCoroutine(SpeedBoostEffect(renderers, duration));
        }
    }

    public void ShowDamageFeedback(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            StartCoroutine(DamageEffect(renderers));
        }
    }

    public void ShowHealFeedback(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            StartCoroutine(HealEffect(renderers));
        }
    }

    // ---- EFECTOS VISUALES ----

    private IEnumerator InvincibilityEffect(Renderer[] renderers, ParticleSystem particles, float duration)
    {
        Color[] originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }

        float blinkSpeed = 0.15f;
        float elapsed = 0f;

        if (particles != null) particles.Play();

        while (elapsed < duration)
        {
            float hue = (elapsed / duration) % 1f;
            Color rainbowColor = Color.HSVToRGB(hue, 1f, 2f);

            foreach (Renderer rend in renderers)
            {
                rend.material.color = rainbowColor;
            }
            yield return new WaitForSeconds(blinkSpeed);

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = originalColors[i];
            }
            yield return new WaitForSeconds(blinkSpeed);

            elapsed += blinkSpeed * 2;
        }

        if (particles != null) particles.Stop();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
    }

    private IEnumerator SpeedBoostEffect(Renderer[] renderers, float duration)
    {
        Color[] originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }

        Color speedColor = new Color(0f, 1.5f, 2f, 1f);
        float elapsed = 0f;
        float blinkSpeed = 0.2f;

        while (elapsed < duration)
        {
            foreach (Renderer rend in renderers)
            {
                rend.material.color = speedColor;
            }
            yield return new WaitForSeconds(blinkSpeed);

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = originalColors[i];
            }
            yield return new WaitForSeconds(blinkSpeed);

            elapsed += blinkSpeed * 2;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
    }

    private IEnumerator DamageEffect(Renderer[] renderers)
    {
        Color[] originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }

        Color damageColor = new Color(2f, 0f, 0f, 1f);

        for (int flash = 0; flash < 3; flash++)
        {
            foreach (Renderer rend in renderers)
            {
                rend.material.color = damageColor;
            }
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = originalColors[i];
            }
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
    }

    private IEnumerator HealEffect(Renderer[] renderers)
    {
        Color[] originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }

        Color healColor = new Color(0f, 2f, 0f, 1f);

        for (int flash = 0; flash < 2; flash++)
        {
            foreach (Renderer rend in renderers)
            {
                rend.material.color = healColor;
            }
            yield return new WaitForSeconds(0.15f);

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = originalColors[i];
            }
            yield return new WaitForSeconds(0.15f);
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
    }
}