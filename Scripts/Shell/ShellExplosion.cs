using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public LayerMask m_CannonMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float maxDamage = 100f;                  
    public float explosionForce = 1000f;            
    public float maxLifeTime = 2f;                  
    public float explosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        Collider[] targetColider = Physics.OverlapSphere(transform.position, explosionRadius, m_TankMask);

        for(int i = 0;i< targetColider.Length; i++)
        {
            Rigidbody targetRigidbody = targetColider[i].GetComponent<Rigidbody>();
            if (!targetRigidbody) continue;

            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            TankHealth targetTankHealth = targetRigidbody.GetComponent<TankHealth>();

            if (!targetTankHealth) continue;

            targetTankHealth.TakeDamage(CalculateDamage(targetRigidbody.position));
        }

        Collider[] cannonColider = Physics.OverlapSphere(transform.position, explosionRadius, m_CannonMask);

        for (int i = 0; i < cannonColider.Length; i++)
        {
            Rigidbody targetRigidbody = cannonColider[i].GetComponent<Rigidbody>();
            if (!targetRigidbody) continue;

            //CannonHealth targetCannonHealth = cannonColider[i].GetComponent<CannonHealth>();
            CannonBehaviorTree targetCannonHealth = cannonColider[i].GetComponent<CannonBehaviorTree>();
            
            if (!targetCannonHealth) continue;

            targetCannonHealth.TakeDamage(CalculateDamage(targetRigidbody.position));
        }

        m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        Destroy(m_ExplosionParticles.gameObject,m_ExplosionParticles.duration);
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude; //cal distance between target position and O of radius

        float relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;

        float damage = relativeDistance * maxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}