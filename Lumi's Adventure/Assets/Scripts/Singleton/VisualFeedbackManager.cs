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

    // Diccionario para guardar materiales originales
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();

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

    // Activa feedback de invencibilidad en un objeto
    public void ShowInvincibilityFeedback(GameObject target, float duration)
    {
        // Buscar renderers (importante para modelos con multiples partes)
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("No se encontró ningún Renderer en " + target.name);
            return;
        }

        Debug.Log($"Encontrados {renderers.Length} renderers en {target.name}");

        ParticleSystem particles = target.GetComponentInChildren<ParticleSystem>();

        StartCoroutine(InvincibilityEffect(renderers, particles, duration));
    }

    // Activa feedback de velocidad en un objeto
    public void ShowSpeedBoostFeedback(GameObject target, float duration)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            StartCoroutine(SpeedBoostEffect(renderers, duration));
        }
    }

    // Activa feedback de daño
    public void ShowDamageFeedback(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            StartCoroutine(DamageEffect(renderers));
        }
    }

    // Activa feedback de curación
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
        List<Material> originalMaterials = new List<Material>();

        foreach (Renderer rend in renderers)
        {
            originalMaterials.Add(rend.material);
            rend.material = new Material(rend.material);
        }

        float blinkSpeed = 0.15f;
        float elapsed = 0f;

        if (particles != null)
        {
            particles.Play();
            Debug.Log("Partículas activadas");
        }

        Debug.Log("Iniciando efecto de invencibilidad arcoíris");

        while (elapsed < duration)
        {
            // Color arcoíris que cambia con el tiempo
            float hue = (elapsed / duration) % 1f; 
            Color rainbowColor = Color.HSVToRGB(hue, 1f, 2f); // Saturacion y brillo maximos

            // Activar color arcoíris
            foreach (Renderer rend in renderers)
            {
                rend.material.color = rainbowColor;
            }
            yield return new WaitForSeconds(blinkSpeed);

            // Volver a color original
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = originalMaterials[i].color;
            }
            yield return new WaitForSeconds(blinkSpeed);

            elapsed += blinkSpeed * 2;
        }

        if (particles != null)
        {
            particles.Stop();
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }

        Debug.Log("Efecto de invencibilidad finalizado");
    }

    private IEnumerator SpeedBoostEffect(Renderer[] renderers, float duration)
    {
        List<Material> originalMaterials = new List<Material>();

        foreach (Renderer rend in renderers)
        {
            originalMaterials.Add(rend.material);
            rend.material = new Material(rend.material);
        }

        Color speedColor = new Color(0f, 1.5f, 2f, 1f); // Azul
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
                renderers[i].material.color = originalMaterials[i].color;
            }
            yield return new WaitForSeconds(blinkSpeed);

            elapsed += blinkSpeed * 2;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
    }

    private IEnumerator DamageEffect(Renderer[] renderers)
    {
        List<Material> originalMaterials = new List<Material>();

        foreach (Renderer rend in renderers)
        {
            originalMaterials.Add(rend.material);
            rend.material = new Material(rend.material);
        }

        Color damageColor = new Color(1f, 0f, 0f, 1f); // Rojo

        for (int i = 0; i < 3; i++)
        {
            foreach (Renderer rend in renderers)
            {
                rend.material.color = damageColor;
            }
            yield return new WaitForSeconds(0.1f);

            for (int j = 0; j < renderers.Length; j++)
            {
                renderers[j].material.color = originalMaterials[j].color;
            }
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
    }

    private IEnumerator HealEffect(Renderer[] renderers)
    {
        List<Material> originalMaterials = new List<Material>();

        foreach (Renderer rend in renderers)
        {
            originalMaterials.Add(rend.material);
            rend.material = new Material(rend.material);
        }

        Color healColor = new Color(0f, 1f, 0f, 1f); // Verde

        for (int i = 0; i < 2; i++)
        {
            foreach (Renderer rend in renderers)
            {
                rend.material.color = healColor;
            }
            yield return new WaitForSeconds(0.15f);

            for (int j = 0; j < renderers.Length; j++)
            {
                renderers[j].material.color = originalMaterials[j].color;
            }
            yield return new WaitForSeconds(0.15f);
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
    }
}