using UnityEngine;

public class CannonMovement : MonoBehaviour
{
    public int cannonNumber = 1;         
    
    public float turnSpeed = 180f; // degree change to time       
    public float pitchRange = 0.2f;
    
    private Rigidbody m_Rigidbody;         
    private float turnValue;        
    private float originalPitch;

    private int delayTurn = 0;
    public bool isAttack = false;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        turnValue = 0f;
        delayTurn = 0;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
    }
    

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        if (delayTurn < 30 && !isAttack)
        {
            Turn();
            delayTurn++;
        }
        else
        {
            turnValue = turnValue * -1;
            delayTurn = 0;
        }
    }

    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turnDegree = turnValue * turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnDegree, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}