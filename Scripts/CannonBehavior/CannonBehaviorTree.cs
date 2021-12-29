using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CannonBehaviorTree : MonoBehaviour
{
    private BehaviorTree behaviorTree;

    public int cannonNumber = 1;
    //movement
    public float turnSpeed = 180f; // degree change to time       
    private Rigidbody m_Rigidbody;
    private float turnValue;
    private int alphaTurn = 0;
    private int delayTurn = 0;
    //health
    public float startingHealth = 100f;
    public Slider m_HealthSlider;
    public Image m_FillImage;
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;
    private AudioSource m_ExplosionAudio;
    private ParticleSystem m_ExplosionParticles;
    public GameObject m_ExplosionPrefab;
    private float currentHealth;
    //attack
    public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public float minLaunchForce = 15f;
    public float maxLaunchForce = 30f;
    public float maxChargeTime = 0.75f;
    private float currentLaunchForce;
    private int delayFire = 0;

    private void Awake()
    {
        //movement
        Transform temp = gameObject.transform.GetChild(0);
        m_Rigidbody = temp.gameObject.GetComponent<Rigidbody>();

        //m_Rigidbody = GetComponentInChildren<Rigidbody>();
        //health
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        m_ExplosionParticles.gameObject.SetActive(false);

        createTree();
    }

    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;
        turnValue = 1f;
        delayTurn = 0;
        currentLaunchForce = minLaunchForce;
        delayFire = 0;

        currentHealth = startingHealth;
        behaviorTree.isDead = false;
        SetHealthUI();
    }

    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }

    private void Update()
    {
        Behavior temp = behaviorTree.getBehavior();
        if (temp == Behavior.attack)
        {
            if (delayFire > 200)
            {
                Fire();
                delayFire = 0;
            }
            else
            {
                delayFire++;
            }
        }
        else if (temp == Behavior.dead)
        {
            OnDeath();
        }
        else if (temp == Behavior.move)
        {
            if (delayTurn >= 5)
            {
                Turn();
                delayTurn = 0;
            }
            else
            {
                delayTurn++;
            }
        }
        else
        {
            Debug.Log("No behavior");
        }
    }

    private void FixedUpdate()
    {
        GameObject playerTank = GameObject.FindWithTag("Player");
        if (playerTank == null) return;
        Transform mySelf = gameObject.transform.GetChild(0);
        Vector2 posPlayer = new Vector2(playerTank.transform.position.x, playerTank.transform.position.z);
        //Vector2 posSelf = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
        Vector2 posSelf = new Vector2(mySelf.position.x, mySelf.position.z);

        Vector2 forwardTarget = new Vector2(posPlayer.x - posSelf.x, posPlayer.y - posSelf.y);
        Vector2 forwardSelf = new Vector2(mySelf.forward.x, mySelf.forward.z);

        float cosAlpha = ((forwardTarget.x * forwardSelf.x) + (forwardTarget.y * forwardSelf.y)) / (forwardTarget.magnitude * forwardSelf.magnitude);
        float alpha = (Mathf.Acos(cosAlpha) * 360) / (2 * Mathf.PI);
        float disPlayerTank = forwardTarget.magnitude;

        if (disPlayerTank <= 30f && disPlayerTank >= 10)
        {
            Debug.Log(alpha);
            if (alpha >= 10)
            {
                Quaternion turnRotation = Quaternion.Euler(0f, alpha * Time.deltaTime, 0f);
                m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
            }
            int randValue = (int)(disPlayerTank - 5f);
            currentLaunchForce = randValue;

            behaviorTree.isAttack = true;
        }
        else
        {
            behaviorTree.isAttack = false;
        }

        behaviorTree.resetTree();
        behaviorTree.updateTree();
    }

    private void createTree()
    {

        Node root = new Node(Type.ROOT, State.waiting, Behavior.None);
        Node attackNode = new Node(Type.AND, State.waiting, Behavior.attack, root);
        Node deadNode = new Node(Type.AND, State.waiting, Behavior.dead, root);

        Node moveLeaf = new Node(Type.LEAF, State.success, Behavior.move, root);
        Node attackLeaf = new Node(Type.LEAF, State.waiting, Behavior.attack, attackNode, true, false);
        Node deadLeaf = new Node(Type.LEAF, State.waiting, Behavior.dead, deadNode, false, true);

        deadNode.lstChild.Add(deadLeaf);
        attackNode.lstChild.Add(attackLeaf);

        root.lstChild.Add(deadNode);
        root.lstChild.Add(attackNode);
        root.lstChild.Add(moveLeaf);

        behaviorTree = new BehaviorTree(root);
    }

    private void Fire()
    {
        // Instantiate and launch the shell.
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = currentLaunchForce * m_FireTransform.forward;

        currentLaunchForce = minLaunchForce;
    }

    public void TakeDamage(float damage)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        currentHealth -= damage / 2;
        SetHealthUI();
        if (currentHealth <= 0f)
        {
            behaviorTree.isDead = true;
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
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();
        gameObject.SetActive(false);
    }

    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turnDegree = turnValue * turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnDegree, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    public void reset()
    {
        behaviorTree.isAttack = false;
        behaviorTree.isDead = false;
        behaviorTree.resetTree();

        OnDisable();
        OnEnable();
    }
}
