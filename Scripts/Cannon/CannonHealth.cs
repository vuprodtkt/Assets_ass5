using UnityEngine;
using UnityEngine.UI;

public class CannonHealth : MonoBehaviour
{
    public float startingHealth = 100f;          
    public Slider m_HealthSlider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float currentHealth;  
    private bool isDead;            


    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        currentHealth = startingHealth;
        isDead = false;

        SetHealthUI();
    }

    public void TakeDamage(float damage)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        currentHealth -= damage/2;
        SetHealthUI();
        if(currentHealth <= 0f && !isDead)
        {
            OnDeath();
        }
    }

    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_HealthSlider.value = currentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, currentHealth / startingHealth);
    }

    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        isDead = true;

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();
        gameObject.SetActive(false);
    }
}