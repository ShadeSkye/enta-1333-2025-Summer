using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BuildingInstance : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private GameObject healthBarPrefab;

    private Image healthBarFill;
    private Transform healthBar;
    private BuildingData buildingData;
    private int currentHealth;

    private void Start()
    {
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform);
            healthBar = hb.transform;
            healthBar.localPosition = new Vector3(0, 0, buildingData.Height + 1f); // adjust based on building height
            healthBarFill = healthBar.Find("HealthBarBackground/HealthBarFill").GetComponent<Image>();

            UpdateHealthBar();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        float percent = (float)currentHealth / buildingData.Health;
        if (healthBarFill != null)
            healthBarFill.fillAmount = percent;
    }

    private void Die()
    {
        Destroy(gameObject); // or set inactive
    }

    public void Initialize(BuildingData data)
    {
        buildingData = data;
        currentHealth = buildingData.Health;

        SetupHealthBar();
    }

    private void SetupHealthBar()
    {
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform);
            healthBar = hb.transform;
            healthBar.localPosition = new Vector3(0, 3f, 0);
            healthBarFill = healthBar.Find("HealthBarBackground/HealthBarFill").GetComponent<Image>();
            UpdateHealthBar();

            // Optional: make it face camera
            if (healthBar.GetComponent<FaceCamera>() == null)
                healthBar.gameObject.AddComponent<FaceCamera>();
        }
    }
}
