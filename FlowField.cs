using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Node
{
    public Vector3 pos;
    public int col;
    public int row;
    public int travelCost;
    public Node(Vector3 newPos, int currCol, int currRow, int currTravelCost)
        {
            pos = newPos;
            col = currCol;
            row = currRow;
            travelCost = currTravelCost;
        }

}

public class FlowField : MonoBehaviour
{

    [SerializeField]
    private int m_AmountOfRows;
    [SerializeField]
    private int m_AmountOfCols;
    [SerializeField]
    GameObject m_Plane;
    [SerializeField]
    private bool m_Debug;


    private Node[,] m_FlowFieldGrid;
    private Vector3Int m_GridSize;
    private float m_NodeSize;

    private void Awake()
    {
        m_GridSize = new Vector3Int((int)m_Plane.GetComponent<Renderer>().bounds.size.x, (int)m_Plane.GetComponent<Renderer>().bounds.size.y, (int)m_Plane.GetComponent<Renderer>().bounds.size.z);
        m_NodeSize = m_GridSize.x / m_AmountOfCols;
        Debug.Log(m_GridSize.x);
        m_AmountOfCols -= 1;
        m_AmountOfRows -= 1;

        m_FlowFieldGrid = new Node[m_AmountOfRows, m_AmountOfCols];
        Debug.Log(m_NodeSize);

    }
    private void Start()
    {
        InitGrid();
    }
    private void InitGrid()
    {
        for (int i = 0; i < m_AmountOfRows; i++)
        {
            for (int j = 0; j < m_AmountOfCols; j++)
            {
                Vector3 currNodePos = new Vector3(-m_GridSize.x/2 + (m_NodeSize*i + m_NodeSize/2) , 0, -m_GridSize.z/2 + (m_NodeSize * j + m_NodeSize / 2)); //extra offset added so cells dont overlap
                m_FlowFieldGrid[i, j] = new Node(currNodePos, j, i, CalcTravelCost());
                Debug.Log("Node created with index");
                Debug.Log(i);
                Debug.Log(j);
                Debug.Log("On pos:");
                Debug.Log(currNodePos);

            }
        }
        Debug.Log("Grid Created");
    }

    //https://leifnode.com/2013/12/flow-field-pathfinding/
    private void InitCostField()
    {
        Physics.OverlapBox(Vector3.one * m_NodeSize);

    }
    private int CalcTravelCost()
    {
        //https://docs.unity3d.com/ScriptReference/Physics.OverlapBox.html
        return 0;
    }


    //DEBUGGING
    private void OnDrawGizmos()
    {
        if(m_Debug)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < m_AmountOfRows; i++)
            {
                for (int j = 0; j < m_AmountOfCols; j++)
                {
                    Vector3 currNodePos = new Vector3(-m_GridSize.x/2 + (m_NodeSize * i + m_NodeSize / 2), 0, -m_GridSize.z/2 + (m_NodeSize * j + m_NodeSize / 2)); //extra offset added so cells dont overlap
                    Vector3 nodeSize = Vector3.one * m_NodeSize * 2;
                    Gizmos.DrawWireCube(currNodePos, nodeSize);
                }
            }
            //Debug.Log("Grid Drawn");

        }
    }

}
