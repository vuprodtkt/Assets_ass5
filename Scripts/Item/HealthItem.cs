using UnityEngine;

public class HealthItem : MonoBehaviour
{
    private int delayHealth = 0;
    private int maxDelayHealth = 80;
    private void Start()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        TankHealth target = other.GetComponent(typeof(TankHealth)) as TankHealth;

        if (delayHealth >= maxDelayHealth)
        {
            target.Health();
            delayHealth = 0;
        }
        else delayHealth++;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        delayHealth = 0;
    }
}