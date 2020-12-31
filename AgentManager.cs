using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public static AgentManager instance;
    [SerializeField]
    private int m_AgentAmount;
    [SerializeField]
    GameObject m_Agent;
    [SerializeField]
    FlowField m_FlowField;
    [SerializeField]
    public float m_MaxLinearSpeed;

    List<GameObject> m_Agents;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        for (int i = 0; i < m_Agents.Count; i++)
        {
            Vector2Int currNodeIdx = m_FlowField.GetNodeIdxFromWorldPos(m_Agents[i].transform.position);
            Node currNode = m_FlowField.GetNodes()[currNodeIdx.x, currNodeIdx.y];

            m_Agents[i].GetComponent<Rigidbody>().AddForce(currNode.directionToEndNode * m_MaxLinearSpeed, ForceMode.VelocityChange );

            if(i==0)
            {
                Debug.Log("Direction Of Node");
                Debug.Log(currNode.directionToEndNode);

                Debug.Log("Velocity Of Agent");
                Debug.Log(m_Agents[i].GetComponent<Rigidbody>().velocity);

            }

        }
    }

    public void InitAgents()
    {
        m_Agents = new List<GameObject>();
        Node[,] tempFF = m_FlowField.GetNodes();
        int rows = m_FlowField.GetAmountOfRows();
        int cols = m_FlowField.GetAmountOfCols();


        for (int i = 0; i < m_AgentAmount; i++)
        {
            Node randomSpawnNode = m_FlowField.GetNodes()[Random.Range(0,rows), Random.Range(0, cols)];
            GameObject currAgent = m_Agent;

            Vector3 newPos = new Vector3(Random.Range(0, randomSpawnNode.pos.x), 0.5f, Random.Range(0, randomSpawnNode.pos.z));
            currAgent.transform.position = newPos;

            m_Agents.Add(currAgent);
            Instantiate(m_Agents[m_Agents.Count - 1]);
        }

        
    }
}
