using UnityEngine;
using UnityEngine.UI;

public class CannonShooting : MonoBehaviour
{
    public int cannonNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
   
    public float minLaunchForce = 15f; 
    public float maxLaunchForce = 30f; 
    public float maxChargeTime = 0.75f;
     
    private float currentLaunchForce;  
    private float chargeSpeed;         
    private bool isFired;

    private int delayFire = 0;

    private void OnEnable()
    {
        currentLaunchForce = minLaunchForce;
        delayFire = 0;
    }


    private void Start()
    {
        chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
    }

    private void Update()
    {
        GameObject playerTank = GameObject.FindWithTag("Player");
        if (playerTank == null) return;
        Vector2 posPlayer = new Vector2(playerTank.transform.position.x, playerTank.transform.position.z);
        Vector2 posSelf = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);

        Vector2 forwardTarget = new Vector2(posPlayer.x - posSelf.x, posPlayer.y - posSelf.y);
        Vector2 forwardSelf = new Vector2(gameObject.transform.forward.x, gameObject.transform.forward.z);

        float cosAlpha = ((forwardTarget.x * forwardSelf.x) + (forwardTarget.y * forwardSelf.y)) / (forwardTarget.magnitude * forwardSelf.magnitude);
        float alpha = (Mathf.Acos(cosAlpha) * 360) / (2 * Mathf.PI);

        float disPlayerTank = forwardTarget.magnitude;

        CannonMovement cannonMove = gameObject.GetComponentInChildren<CannonMovement>();

        if (disPlayerTank < 30f)
        {
            if (alpha > 20)
            {
                Quaternion turnRotation = Quaternion.Euler(0f, alpha, 0f);
                Rigidbody m_Rigidbody = gameObject.GetComponentInChildren<Rigidbody>();
                m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
            }

            cannonMove.isAttack = true;
            if (delayFire > 150)
            {
                int randValue = (int)(disPlayerTank - 5f);
                currentLaunchForce = randValue;
                Fire();
                delayFire = 0;
            }
            else
            {
                delayFire++;
            }

        }
        else
        {
            delayFire = 200;
            cannonMove.isAttack = false;
        }
    }

    private void Fire()
    {
        // Instantiate and launch the shell.
        isFired = true;
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = currentLaunchForce * m_FireTransform.forward;

        currentLaunchForce = minLaunchForce;
    }
}