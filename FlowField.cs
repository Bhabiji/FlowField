using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 pos;
    public int row;
    public int col;
    public int travelCost;
    public int nodeValue;
    public Vector3 directionToEndNode;

    public Node(Vector3 newPos, int currRow, int currCol, int currTravelCost)
        {
            pos = newPos;
            row = currRow;
            col = currCol;
            travelCost = currTravelCost;
            nodeValue = 65535;
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
    [SerializeField]
    private Sprite m_DebugSprite;


    private Node[,] m_FlowFieldGrid;
    private Vector3 m_GridSize;
    private int m_AmountOfRows;
    private int m_AmountOfCols;
    private Node m_EndNode;
    private List<GameObject> m_DebugSprites = new List<GameObject>();


    private void Start()
    {
        m_GridSize = m_Plane.GetComponent<Renderer>().bounds.size;
        //generate grid over whole plane dependant on nodeSize
        //-1 to def clamp withing plane
        m_AmountOfRows = (int)(m_GridSize.z / m_NodeSize);
        m_AmountOfCols = (int)(m_GridSize.x / m_NodeSize);
        //Debug.Log(m_GridSize.z);
        //Debug.Log(m_GridSize.x);
        //Debug.Log(m_AmountOfRows);

        //Debug.Log(m_AmountOfCols);

        m_FlowFieldGrid = new Node[m_AmountOfRows, m_AmountOfCols];


        InitGrid();
        InitCostField();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ResetNodeDirections();
            InitGrid();
            InitCostField();
            HandleInput();
            CalculateNodeDirections();
            DrawDirections();
            for (int i = 0; i < m_AmountOfRows; i++)
            {
                for (int j = 0; j < m_AmountOfCols; j++)
                {
                    
                    //Debug.Log(m_FlowFieldGrid[i, j].nodeValue);
                }
            }

        }
    }

    void ResetNodeDirections()
    {
        for (int i = 0; i < m_DebugSprites.Count; i++)
        {
            Destroy(m_DebugSprites[i]);
        }
    }
    void DrawDirections()
    {

        for (int i = 0; i < m_AmountOfRows; i++)
        {
            for (int j = 0; j < m_AmountOfCols; j++)
            {
                Vector3 currNodeDirection = m_FlowFieldGrid[i, j].directionToEndNode;
                GameObject debugDirection = new GameObject();
                m_DebugSprites.Add(debugDirection);

                SpriteRenderer debugSprite = debugDirection.AddComponent<SpriteRenderer>();
                debugSprite.transform.position = new Vector3(m_FlowFieldGrid[i, j].pos.x - m_NodeSize/2, 0, m_FlowFieldGrid[i, j].pos.z - m_NodeSize/2);
                debugSprite.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                debugSprite.sprite = m_DebugSprite;
                debugSprite.color = Color.blue;
                //Up
                if (currNodeDirection == new Vector3(0, 0, 1))
                {
                    debugSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                //Right
                else if (currNodeDirection == new Vector3(1, 0, 0))
                {
                    debugSprite.transform.rotation = Quaternion.Euler(90, 90, 0);
                }
                //Down
                else if (currNodeDirection == new Vector3(0, 0, -1))
                {
                    debugSprite.transform.rotation = Quaternion.Euler(90, 180, 0);
                }
                //Left
                else if (currNodeDirection == new Vector3(-1, 0, 0))
                {
                    debugSprite.transform.rotation = Quaternion.Euler(90, -90, 0);
                }
                //UpRight
                else if (currNodeDirection == new Vector3(1, 0, 1))
                {
                    debugSprite.transform.rotation = Quaternion.Euler(90, 45, 0);
                }
                //DownRight
                else if (currNodeDirection == new Vector3(1, 0, -1))
                {
                    debugSprite.transform.rotation = Quaternion.Euler(90, 135, 0);
                }
                //DownLeft
                else if (currNodeDirection == new Vector3(-1, 0, -1))
                {
                    debugSprite.transform.rotation = Quaternion.Euler(90, 225, 0);
                }
                //DownLeft
                else if (currNodeDirection == new Vector3(-1, 0, 1))
                {
                    debugSprite.transform.rotation = Quaternion.Euler(90, -45, 0);
                }
            }
        }


    }

    void HandleInput()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        //Debug.Log(mousePos.x);
        //Debug.Log(mousePos.z);

        Vector2Int endNodeIdx = GetNodeIdxFromWorldPos(mousePos);
        m_EndNode = m_FlowFieldGrid[endNodeIdx.x, endNodeIdx.y];
        InitIntegrationField();
    }

    private void InitGrid()
    {
        for (int i = 0; i < m_AmountOfRows; i++)
        {
            for (int j = 0; j < m_AmountOfCols; j++)
            {
                Vector3 currNodePos = new Vector3((-m_GridSize.x / 2 + m_NodeSize / 2) + (m_NodeSize * j + m_NodeSize / 2), 0, (-m_GridSize.z / 2 + m_NodeSize / 2) + (m_NodeSize * i + m_NodeSize / 2)); //extra offset added so Nodes dont overlap
                m_FlowFieldGrid[i, j] = new Node(currNodePos, i, j, 1);
                //Debug.Log("Node created with index");
                //Debug.Log(i);
                //Debug.Log(j);
                //Debug.Log("On pos:");
                //Debug.Log(currNodePos);

            }
        }
        //Debug.Log("Grid Created");
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
                        //Debug.Log("Mud");
                    }
                    else if (collider.gameObject.layer == 4)
                    {
                        //Water == high cost
                        m_FlowFieldGrid[i, j].travelCost = 10;
                        //Debug.Log("Water");
                    }
                    else if (collider.gameObject.layer == 9)
                    {
                        //Obstacle cant walk through
                        m_FlowFieldGrid[i, j].travelCost = 255;
                        //Debug.Log("Obstacle");
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
    private void InitIntegrationField()
    {
        //List<Node> openList = new List<Node>();
        //openList.Add(m_EndNode);

        //A* reset for end node to start the loop
        m_EndNode.nodeValue = 0;
        m_EndNode.travelCost = 0;

        Queue<Node> openList = new Queue<Node>();
        openList.Enqueue(m_EndNode);

        List<Node> connectedNodes;

        while (openList.Count != 0)
        {
            //get last node in queue, throw that node out of queue 
            Node currNode = openList.Dequeue();
            connectedNodes = GetConnectedNodes(currNode, false, false);
            for (int i = 0; i < connectedNodes.Count; i++)
            {
                int gCost = connectedNodes[i].travelCost + currNode.nodeValue;
                //This will generate the distance values for each node from the end node  and be 65535 for obstacles
                if (gCost < connectedNodes[i].nodeValue)
                {
                    connectedNodes[i].nodeValue = gCost;
                    openList.Enqueue(connectedNodes[i]);
                }
            }
        }
    }

    private List<Node> GetConnectedNodes(Node centerNode, bool isConnectedDiagonal, bool includeOwn)
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
        if(includeOwn)
        {
            //Own
            connections.Add(new Vector2Int(1, 1));
        }

        for (int i = 0; i < connections.Count; i++)
        {
            Vector2Int currIdx = new Vector2Int(centerNode.row, centerNode.col);
            Vector2Int connectedNodeIdx = currIdx + connections[i];

            //check if node is outside grid, if so, skip this direction
            if(connectedNodeIdx.x < 0 || connectedNodeIdx.y < 0 || connectedNodeIdx.x >= m_AmountOfRows || connectedNodeIdx.y >= m_AmountOfCols)
            {
                continue;
            }
            else
            {
                connectedNodes.Add(m_FlowFieldGrid[connectedNodeIdx.x, connectedNodeIdx.y]);
            }
        }
        

        return connectedNodes;
    }

    private Vector2Int GetNodeIdxFromWorldPos(Vector3 pos)
    {
        //Add offset to convert 0,0 origin to leftBottom of gameObject
        pos.x += m_GridSize.x / 2;
        pos.z += m_GridSize.z / 2;

        float xRatio = Mathf.Clamp(pos.x / (m_AmountOfCols * m_NodeSize), 0,1);
        float yRatio = Mathf.Clamp(pos.z / (m_AmountOfRows * m_NodeSize), 0, 1); //If u think traditional 2d even tho working in z-axis

        Vector2Int nodeIdx = new Vector2Int(Mathf.Clamp((int)((m_AmountOfRows) * yRatio), 0, m_AmountOfRows - 1), Mathf.Clamp((int)((m_AmountOfCols) * xRatio), 0, m_AmountOfCols - 1));
        return nodeIdx;
    }


    //ALL DIRECTIONS FOR PATH TO ENDNODE
    void CalculateNodeDirections()
    {
        for (int i = 0; i < m_AmountOfRows; i++)
        {
            for (int j = 0; j < m_AmountOfCols; j++)
            {
                Node currNode = m_FlowFieldGrid[i, j];

                List<Node> connectedNodes = GetConnectedNodes(currNode, true, true);
                for (int k = 0; k < connectedNodes.Count; k++)
                {
                    int nodeValue = currNode.nodeValue;
                    if(connectedNodes[k].nodeValue < nodeValue)
                    {
                        nodeValue = connectedNodes[k].nodeValue;
                        Vector3 nodeDirection = connectedNodes[k].pos - currNode.pos;
                        nodeDirection /= m_NodeSize;
                        currNode.directionToEndNode = nodeDirection;
                    }
                }
            }
        }
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
                    Vector3 currNodePos = m_FlowFieldGrid[i, j].pos;
                    Vector3 nodeSize = Vector3.one * m_NodeSize * 2;

                    //if(m_FlowFieldGrid[i,j].travelCost == 1)
                    //    Gizmos.color = Color.blue;

                    //else if (m_FlowFieldGrid[i, j].travelCost == 5)
                    //    Gizmos.color = Color.green;

                    //else if (m_FlowFieldGrid[i, j].travelCost == 10)
                    //    Gizmos.color = Color.yellow;

                    //else if (m_FlowFieldGrid[i, j].travelCost == byte.MaxValue)
                    //    Gizmos.color = Color.black;



                    float currNodeValue = m_FlowFieldGrid[i, j].nodeValue;
                    Gizmos.color = new Color(currNodeValue / 10, currNodeValue / 10, currNodeValue / 10);
                    Gizmos.DrawWireCube(currNodePos, nodeSize);
                    

                }
            }


        }
    }

}
