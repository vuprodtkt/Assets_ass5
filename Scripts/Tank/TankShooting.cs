using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int playerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float minLaunchForce = 15f; 
    public float maxLaunchForce = 30f; 
    public float maxChargeTime = 0.75f;

    private string fireButton;         
    private float currentLaunchForce;  
    private float chargeSpeed;         
    private bool isFired;

    public bool isAi = false;
    private int delayFire = 0;

    private void OnEnable()
    {
        currentLaunchForce = minLaunchForce;
        m_AimSlider.value = minLaunchForce;

        delayFire = 0;
    }


    private void Start()
    {
        fireButton = "Fire" + playerNumber;

        chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
    }

    private void Update()
    {
        if (isAi)
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

            TankMovement tankMove = gameObject.GetComponent<TankMovement>();

            if (disPlayerTank < 20f)
            {
                if(alpha > 20)
                {
                    Quaternion turnRotation = Quaternion.Euler(0f, alpha, 0f);
                    Rigidbody m_Rigidbody = gameObject.GetComponent<Rigidbody>();
                    m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
                }

                tankMove.isAttack = true;

                if (delayFire > 150)
                {
                    int randValue = (int) (disPlayerTank);

                    m_AimSlider.value = randValue;
                    currentLaunchForce = randValue;
                    Fire();
                    delayFire = 0;
                }
                else
                {
                    m_AimSlider.value = 0;
                    delayFire++;
                }
            }
            else
            {
                delayFire = 250;
                tankMove.isAttack = false;
            }
        }
        else
        {
            // Track the current state of the fire button and make decisions based on the current launch force.
            m_AimSlider.value = minLaunchForce;

            if (currentLaunchForce >= maxLaunchForce && !isFired)
            {
                currentLaunchForce = maxLaunchForce;
                Fire();
            }
            else if (Input.GetButtonDown(fireButton))
            {
                // pressed fire button for the first time
                isFired = false;
                currentLaunchForce = minLaunchForce;
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }
            else if (Input.GetButton(fireButton) && !isFired)
            {
                // holding the fire button, not yet fired
                currentLaunchForce += chargeSpeed * Time.deltaTime;
                m_AimSlider.value = currentLaunchForce;

            }
            else if (Input.GetButtonUp(fireButton) && !isFired)
            {
                // released the button, having not fired yet
                Fire();
            }
        }
    }

    private void Fire()
    {
        // Instantiate and launch the shell.
        isFired = true;
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = currentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        currentLaunchForce = minLaunchForce;
    }
}