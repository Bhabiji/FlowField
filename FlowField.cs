using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Node
{
    public Vector3 pos;
    public int row;
    public int col;
    public int travelCost;
    public int cellValue;
    public Node(Vector3 newPos, int currRow, int currCol, int currTravelCost)
        {
            pos = newPos;
            row = currRow;
            col = currCol;
            travelCost = currTravelCost;
        }

}
public class FlowField : MonoBehaviour
{

    [SerializeField]
    private float m_NodeSize;
    [SerializeField]
    GameObject m_Plane;
    [SerializeField]
    private bool m_Debug;


    private Node[,] m_FlowFieldGrid;
    private Vector3 m_GridSize;
    private int m_AmountOfRows;
    private int m_AmountOfCols;
    private Node m_EndNode;


    private void Start()
    {
        m_GridSize = m_Plane.GetComponent<Renderer>().bounds.size;
        //generate grid over whole plane dependant on nodeSize
        //-1 to def clamp withing plane
        m_AmountOfRows = (int)(m_GridSize.z / m_NodeSize) - 1;
        m_AmountOfCols = (int)(m_GridSize.x / m_NodeSize) - 1;
        Debug.Log(m_GridSize.z);
        Debug.Log(m_GridSize.x);
        Debug.Log(m_AmountOfRows);

        Debug.Log(m_AmountOfCols);

        m_FlowFieldGrid = new Node[m_AmountOfRows, m_AmountOfCols];


        InitGrid();
        InitCostField();
    }
    private void InitGrid()
    {
        for (int i = 0; i < m_AmountOfRows; i++)
        {
            for (int j = 0; j < m_AmountOfCols; j++)
            {
                Vector3 currNodePos = new Vector3((-m_GridSize.x / 2) + (m_NodeSize * j + m_NodeSize / 2), 0, (-m_GridSize.z / 2) + (m_NodeSize * i + m_NodeSize / 2)); //extra offset added so cells dont overlap
                m_FlowFieldGrid[i, j] = new Node(currNodePos, i, j, 1);
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

        int layer = LayerMask.GetMask("Mud", "Water", "Obstacle");
        //Only need colliders not full objects
        //GameObject[] gameObjectsWithLayers = FindGameObjectsInLayer(layer);
        Quaternion indentityQuat = new Quaternion(1, 0, 0, 0);

        for (int i = 0; i < m_AmountOfRows; i++)
        {
            for (int j = 0; j < m_AmountOfCols; j++)
            {


                Node currNode = m_FlowFieldGrid[i, j];
                //https://docs.unity3d.com/ScriptReference/Physics.OverlapBox.html
                Collider[] terrainLayers = Physics.OverlapBox(currNode.pos, Vector3.one * (m_NodeSize / 2), indentityQuat, layer);
                foreach (Collider collider in terrainLayers)
                {
                    if (collider.gameObject.layer == 8)
                    {
                        //Mud == slightly higher cost
                        m_FlowFieldGrid[i, j].travelCost = 5;
                        Debug.Log("Mud");
                    }
                    else if (collider.gameObject.layer == 4)
                    {
                        //Water == high cost
                        m_FlowFieldGrid[i, j].travelCost = 10;
                        Debug.Log("Water");
                    }
                    else if (collider.gameObject.layer == 9)
                    {
                        //Obstacle cant walk through
                        m_FlowFieldGrid[i, j].travelCost = 255;
                        Debug.Log("Obstacle");
                    }

                }
            }
        }
    }

    GameObject[] FindGameObjectsInLayer(int layer)
    {
        GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        //Need list functionality
        List<GameObject> gameObjectsList = new System.Collections.Generic.List<GameObject>();
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.layer == layer)
            {
                gameObjectsList.Add(gameObject);
            }
        }
        if (gameObjectsList.Count == 0)
        {
            return null;
        }
        return gameObjectsList.ToArray();
    }

    //IntegrationField
    //https://leifnode.com/2013/12/flow-field-pathfinding/
    public void InitIntegrationField()
    {
        //List<Node> openList = new List<Node>();
        //openList.Add(m_EndNode);

        Queue<Node> openList = new Queue<Node>();
        openList.Enqueue(m_EndNode);
        List<Node> connectedNodes;
        
        while (openList.Count != 0)
        {
            //get last cell in queue
            Node currNode = openList.Dequeue();
            connectedNodes = GetConnectedNodes(currNode, false);
            for (int i = 0; i < connectedNodes.Count; i++)
            {
                
            }
        }
    }

    private List<Node> GetConnectedNodes(Node centerNode, bool isConnectedDiagonal)
    {
        List<Node> connectedNodes = new List<Node>();
        List<Vector2Int> connections = new List<Vector2Int>();
        //Up
        connections.Add(new Vector2Int(0, 1));
        //Right
        connections.Add(new Vector2Int(1, 0));
        //Down
        connections.Add(new Vector2Int(0, -1));
        //Left
        connections.Add(new Vector2Int(-1, 0));
        if(isConnectedDiagonal)
        {
            //UpRight
            connections.Add(new Vector2Int(1, 1));
            //DownRight
            connections.Add(new Vector2Int(1, -1));
            //DownLeft
            connections.Add(new Vector2Int(-1, -1));
            //UpLeft
            connections.Add(new Vector2Int(-1, 1));
        }

        for (int i = 0; i < connections.Count; i++)
        {
            Vector2Int currIdx = new Vector2Int(centerNode.row, centerNode.col);
            Vector2Int connectedNodeIdx = currIdx + connections[i];

            //check if node is outside grid, if so, skip this direction
            if(connectedNodeIdx.x < 0 || connectedNodeIdx.y < 0 || connectedNodeIdx.x > m_AmountOfRows || connectedNodeIdx.y > m_AmountOfCols)
            {
                continue;
            }
            connectedNodes.Add(m_FlowFieldGrid[connectedNodeIdx.x, connectedNodeIdx.y]);
        }

        return connectedNodes;
    }



    //------------------------DEBUGGING---------------------------
    private void OnDrawGizmos()
    {
        if(m_Debug)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < m_AmountOfRows; i++)
            {
                for (int j = 0; j < m_AmountOfCols; j++)
                {
                    Vector3 currNodePos = new Vector3((-m_GridSize.x/2 + m_NodeSize/2) + (m_NodeSize * j + m_NodeSize / 2), 0, (-m_GridSize.z/2 + m_NodeSize / 2) + (m_NodeSize * i + m_NodeSize / 2)); //extra offset added so cells dont overlap
                    Vector3 nodeSize = Vector3.one * m_NodeSize * 2;

                    if(m_FlowFieldGrid[i,j].travelCost == 1)
                        Gizmos.color = Color.blue;

                    else if (m_FlowFieldGrid[i, j].travelCost == 5)
                        Gizmos.color = Color.green;

                    else if (m_FlowFieldGrid[i, j].travelCost == 10)
                        Gizmos.color = Color.yellow;

                    else if (m_FlowFieldGrid[i, j].travelCost == byte.MaxValue)
                        Gizmos.color = Color.black;

                    Gizmos.DrawWireCube(currNodePos, nodeSize);
                }
            }


        }
    }

}
