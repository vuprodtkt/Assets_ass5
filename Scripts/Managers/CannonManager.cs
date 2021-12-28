using System;
using UnityEngine;

[Serializable]
public class CannonManager
{       
    public Transform m_SpawnPoint;         
    [HideInInspector] public int cannonNumber;             
    [HideInInspector] public GameObject m_Instance;                              

    private CannonMovement m_Movement;       
    private CannonShooting m_Shooting;
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        m_Movement = m_Instance.GetComponentInChildren<CannonMovement>();
        m_Shooting = m_Instance.GetComponentInChildren<CannonShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.cannonNumber = cannonNumber;
        m_Shooting.cannonNumber = cannonNumber;
    }


    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }

    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
