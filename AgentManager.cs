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
    private float m_MaxLinearSpeed;
    [SerializeField]
    Material m_debugMat;
    List<GameObject> m_Agents;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        m_Agents = new List<GameObject>();
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        for (int i = 0; i < m_Agents.Count; i++)
        {
            float terrainSpeed = m_MaxLinearSpeed;

            Vector2Int currNodeIdx = m_FlowField.GetNodeIdxFromWorldPos(m_Agents[i].transform.position);
            Node currNode = m_FlowField.GetNodes()[currNodeIdx.x, currNodeIdx.y];
            //Slower terrain effect
            terrainSpeed /= currNode.travelCost;

            m_Agents[i].GetComponent<Rigidbody>().velocity =currNode.directionToEndNode * terrainSpeed * Time.deltaTime;
            //m_Agents[i].GetComponent<Renderer>().material = m_debugMat;


            if (i==0)
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
        
        Node[,] tempFF = m_FlowField.GetNodes();
        int rows = m_FlowField.GetAmountOfRows();
        int cols = m_FlowField.GetAmountOfCols();


        for (int i = 0; i < m_AgentAmount; i++)
        {
            
            
            Node randomSpawnNode = m_FlowField.GetNodes()[Random.Range(0,rows), Random.Range(0, cols)];
            //Obstacles cant be spawned upon
            while (randomSpawnNode.travelCost==255)
            {
                randomSpawnNode = m_FlowField.GetNodes()[Random.Range(0, rows), Random.Range(0, cols)];
            }

            GameObject currAgent = Instantiate(m_Agent);
            currAgent.transform.parent = transform;
            currAgent.transform.position = new Vector3(randomSpawnNode.pos.x, 0.5f, randomSpawnNode.pos.z);

            m_Agents.Add(currAgent);
         
        }

        
    }
}
