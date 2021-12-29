using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int numRoundsToWin = 5;        
    public float startDelay = 3f;         
    public float endDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;
    public GameObject m_CannonPrefab;
    public GameObject m_CannonBehaviorPrefab;
    public TankManager[] m_Tanks;
    public CannonManager[] m_Cannons;

    private int roundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    private int typePlay;


    const float k_MaxDepenetrationVelocity = float.PositiveInfinity;

    private void Start()
    {
        // This line fixes a change to the physics engine.
        Physics.defaultMaxDepenetrationVelocity = k_MaxDepenetrationVelocity;
        m_StartWait = new WaitForSeconds(startDelay);
        m_EndWait = new WaitForSeconds(endDelay);

    }

    public void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].playerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }

    public void SpawnAllCannons()
    {
        for (int i = 0; i < m_Cannons.Length; i++)
        {
            m_Cannons[i].m_Instance =
                Instantiate(m_CannonBehaviorPrefab, m_Cannons[i].m_SpawnPoint.position, m_Cannons[i].m_SpawnPoint.rotation) as GameObject;
            m_Cannons[i].cannonNumber = i + 1;
            m_Cannons[i].Setup();
        }
    }

    public void SetCameraPlayTwoPlayer()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }

    public void SetCameraPlayerVsAi()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }

    public void play(int type)
    {
        typePlay = type;
        if (type == 1) //Player vs Ai
        {
            SpawnAllTanks();
            SpawnAllCannons();
            SetCameraPlayerVsAi();
        }else if(type == 2) // Play Online
        {
            SpawnAllTanks();
            SetCameraPlayTwoPlayer();
        }
        else if (type == 3) // play Offline
        {
            SpawnAllTanks();
            SetCameraPlayTwoPlayer();
        }
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {

        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            if (typePlay == 1) //Player vs Ai
            {
                SetCameraPlayerVsAi();
            }
            else if (typePlay == 2) // Play Online
            {
                SetCameraPlayTwoPlayer();
            }
            else if (typePlay == 3) // play Offline
            {
                SetCameraPlayTwoPlayer();
            }
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        if (typePlay == 3) // play offline
        {
            DisableTankControl();

            m_CameraControl.SetStartPositionAndSize();
            roundNumber++;
            m_MessageText.text = "Round " + roundNumber;
            yield return m_StartWait;
        }else if(typePlay == 2) // play Online
        {
            DisableTankControl();

            m_CameraControl.SetStartPositionAndSize();
            roundNumber++;
            m_MessageText.text = "Round " + roundNumber;
            yield return m_StartWait;
        }
        else if(typePlay == 1) // play vs Ai
        {
            ResetAllCannons();
            DisableTankControlByAi();

            m_CameraControl.SetStartPositionAndSize();
            m_MessageText.text = "Player VS Ai";
            yield return m_StartWait;
        }
    }


    private IEnumerator RoundPlaying()
    {
        if (typePlay == 3) // play offline
        {
            EnableTankControl();
            m_MessageText.text = string.Empty;
            while (!OneTankLeft())
            {
                yield return null;
            }
        }
        else if (typePlay == 2) // play Online
        {
            EnableTankControl();
            m_MessageText.text = string.Empty;
            while (!OneTankLeft())
            {
                yield return null;
            }
        }
        else if (typePlay == 1) // play vs Ai
        {
            EnableTankControlByAi();
            m_MessageText.text = string.Empty;
            while (!OneTankLeft())
            {
                yield return null;
            }
        }
    }

    private IEnumerator RoundEnding()
    {
        if (typePlay == 3) // play offline
        {
            DisableTankControl();
            m_RoundWinner = null;

            m_RoundWinner = GetRoundWinner();

            if (m_RoundWinner != null)
            {
                m_RoundWinner.nWins++;
            }

            m_GameWinner = GetGameWinner();

            string message = EndMessage();
            m_MessageText.text = message;

            yield return m_EndWait;
        }
        else if (typePlay == 2) // play Online
        {
            DisableTankControl();
            m_RoundWinner = null;

            m_RoundWinner = GetRoundWinner();

            if (m_RoundWinner != null)
            {
                m_RoundWinner.nWins++;
            }

            m_GameWinner = GetGameWinner();

            string message = EndMessage();
            m_MessageText.text = message;

            yield return m_EndWait;
        }
        else if (typePlay == 1) // play vs Ai
        {
            DisableTankControlByAi();
            m_RoundWinner = GetRoundWinner();

            if (m_RoundWinner != null)
            {
                if (m_RoundWinner.playerNumber == 1)
                {
                    string message = "Player win!";
                    m_MessageText.text = message;
                }
                else
                {
                    string message = "Ai win!";
                    m_MessageText.text = message;
                }
                
            }
            yield return m_EndWait;
        }
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }

    //private bool checkAiLoop()
    //{
    //    int numTanksLeft = 0;
    //    bool isPlayerLived = false;

    //    for (int i = 0; i < m_Tanks.Length; i++)
    //    {
    //        if (m_Tanks[i].m_Instance.activeSelf)
    //        {
    //            numTanksLeft++;
    //            if (m_Tanks[i].playerNumber == 1) isPlayerLived = true;
    //        }
    //    }

    //    int numCannonsLeft = 0;
    //    for (int i = 0; i < m_Cannons.Length; i++)
    //    {
    //        if (m_Cannons[i].m_Instance.activeSelf)
    //            numCannonsLeft++;
    //    }

    //    if (isPlayerLived && numTanksLeft == 1 && numCannonsLeft <= 0) return true;
    //    if (!isPlayerLived) return true;

    //    return false;
    //}


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].nWins == numRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.coloredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].coloredPlayerText + ": " + m_Tanks[i].nWins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.coloredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }

    private void ResetAllCannons()
    {
        for (int i = 0; i < m_Cannons.Length; i++)
        {
            m_Cannons[i].Reset();
        }
    }

    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }

    private void EnableTankControlByAi()
    {
        m_Tanks[0].EnableControl();
        m_Tanks[0].m_Instance.tag = "Player";
        for (int i = 1; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControlByAi();
        }

        for (int i = 1; i < m_Cannons.Length; i++)
        {
            m_Cannons[i].EnableControl();
        }
    }

    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    private void DisableTankControlByAi()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }

        for (int i = 1; i < m_Cannons.Length; i++)
        {
            m_Cannons[i].DisableControl();
        }
    }
}