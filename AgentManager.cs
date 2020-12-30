using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
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
    void Start()
    {
        InitAgents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitAgents()
    {
        m_Agents = new List<GameObject>();
        Node[,] tempFF = m_FlowField.GetNodes();
        int rows = m_FlowField.GetAmountOfRows();
        int cols = m_FlowField.GetAmountOfCols();


        for (int i = 0; i < m_AgentAmount; i++)
        {
            Node randomSpawnNode = tempFF[Random.Range(0,rows), Random.Range(0, cols)];
            GameObject currAgent = m_Agent;

            Vector3 newPos = new Vector3(Random.Range(0, randomSpawnNode.pos.x), 0, Random.Range(0, randomSpawnNode.pos.z));
            currAgent.transform.position = newPos;
           

        }

    }
}
