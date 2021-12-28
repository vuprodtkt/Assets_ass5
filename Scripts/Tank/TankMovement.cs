using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int playerNumber = 1;         
    public float speed = 12f; // speed of tank            
    public float turnSpeed = 180f; // degree change to time       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float pitchRange = 0.2f;
    
    private string movementAxisName;     
    private string turnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float movementInputValue;    
    private float turnInputValue;        
    private float originalPitch;

    private int delayTurn = 0;
    private int delayMove = 0;
    public bool isAi = false;
    public bool isAttack = false;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        movementInputValue = 0f;
        turnInputValue = 0f;

        delayMove = 0;
        delayTurn = 0;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        movementAxisName = "Vertical" + playerNumber;
        turnAxisName = "Horizontal" + playerNumber;

        originalPitch = m_MovementAudio.pitch;
    }
    

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        movementInputValue = Input.GetAxis (movementAxisName);// get input.Vertical1 in unity
        turnInputValue = Input.GetAxis (turnAxisName);//get input.Horizontal1 in unity

        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if (Mathf.Abs(movementInputValue) < 0.1f && Mathf.Abs(turnInputValue) < 0.1f)
        {
            // tank is idling
            if(m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            // tank is moving
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        // Move and turn the tank.
        if (isAi)
        {
            if (delayMove < 30 && !isAttack)
            {
                Move();
                delayMove++;
            }
            else
            {
                if (delayTurn < 30 && !isAttack)
                {
                    Turn();
                    delayTurn++;
                }
                else
                {
                    delayTurn = 0;
                    delayMove = 0;
                }
            }
        }
        else
        {
            Move();
            Turn();
        }
        
    }

    private void Move()
    {
        if (isAi)
        {
            movementInputValue = 1;
        }
        // Adjust the position of the tank based on the player's input.
        // cal move for tank
        Vector3 movement = transform.forward * movementInputValue * speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        if (isAi)
        {
            turnInputValue = 1;
        }
        // Adjust the rotation of the tank based on the player's input.
        float turnDegree = turnInputValue * turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnDegree, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}