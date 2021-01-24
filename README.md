## Flow Fields, RTS Pathfinding in Unity
## Introduction
The charm of an RTS game is, for many, having control over a high amount of units to strategically defeat the opponent.
Making sure that the player's actions do not get misinterpreted, pathfinding is implemented, which can be very performance heavy for your machine.

To minimize this performance tax while giving a somewhat efficient path to the player's high amount of units, Flow fields can be utilized.

### Modern viability
While algorithms such as A* dominate the game A.I. market, it is isn't the optimal solution in every scenario. Flowfields are generally more preferred when there's a very dense graph with a high amount of cells/nodes, there are a high amount of units that have to reach the same location and when positions of units have to be changed constantly (which is very common in RTS games).

## How?
Derived from the Dijkstra algorithm, A Flow field exists out of 2 major components (in order of execution):
- Cost field
- Integration field

In Unity there are different approaches to tackle this algorithm, from a performance standpoint the optimal approach would be through the DOTS architecture, 
making more optimal usage of your CPU/GPU through multi-threading. Since time was a constraint, a more traditional Monobehavior implementation was used.

### Grid
To start your Flowfield journey one must first implement atleast a 2D grid, since the nodes/cells in the grid will be utilized for path calculation.
The grid I created was given a cell size and calculated how many cells would fit in the 2D plane Gameobject carrying the grid.

## 1. Cost Field
An area can sometimes exist out of different materials, some being easier for units to walk over (grass, asphalt etc.), 
while others might be more tedious for units (water, mud etc.).

With Flow fields, each gridnode or grid cell has a cost, the lower the cost, the "faster" that node can be travelled through.
This way impenetrable obstacles can be added but also water, mud etc.

In this Unity implementation, layer masks can be utilized to easily see if a specific gridcell is within a specific layer mask.
Using simple coditionals, you can add travel costs to nodes that collide with an object of a specific layer.


![Image of my CostField](https://raw.githubusercontent.com/Bhabiji/FlowField/master/Images/CostField.JPG)
### Pseudo code Cost Field
```C#
 void InitCostField()
    {
        int layer = GetAllLayerMasks();
        //GameObject[] objectsInLayers = FindGameObjectsInLayers(layer);
        for(each row in grid)
        {
            for(//each column in grid)
            {
                Collider[] terrainLayers = GetAllTheCollidersOfCellsCollidingWithLayers()
                foreach (Collider collider in terrainLayers)
                {
                    if(collider collides specific layer)
                    {
                        m_FlowFieldGrid[i, j].travelCost = 5;
                    }
                    else if(collider collides with other layer)
                    {
                        m_FlowFieldGrid[i, j].travelCost = 10;
                    }
                    else if(collider collides with other layer)
                    {
                        m_FlowFieldGrid[i, j].travelCost = 255; //<--This is a full byte == impassable obstacle
                    }

                }
            }
        }
    }
```

## 2. Integration Field
### The core of Flowfields
The integration field is the core of the flow field where the pathfinding calculations are done, deriving from the well known Dijkstra algorithm to 
find the value of each node starting from the goal node, keeping their travelCosts in mind.

The integration field is calculated by first setting the value of each cell to a high value. 
After this the goal node's value is set to 0 and is pushed to an open list. 
Now we get the node at the start of the open list and set the value of its connected nodes to the node 
from the open list + the travelCost of the connected node (see Cost Field section).
This is done until the open list is empty and thus all nodes have their corresponding value.

So in short:

1. Set node values to high value
2. Set goal node value to 0, Add to open list
3. Get the first node in open list, throw it out of open list
4. Get gotten node connectedNodes
5. Set connected node values to gotten node + connected node travelCost
6. Reiterate last 3 steps until open list is empty

### pseudo code Integration Field
```C#
private void InitIntegrationField()
    {
        //A* reset for end node to start the loop
        m_EndNode.nodeValue = 0;
        m_EndNode.travelCost = 0;

        Queue<Node> openList = new Queue<Node>();
        openList.Enqueue(m_EndNode);

        List<Node> connectedNodes;

        while(there are nodes in openList)
        {
            //get last node in queue, throw that node out of queue 
            Node currNode = GetBeginningNodeInOpenList();
            connectedNodes = GetConnectedNodes();
            for(all the Connected Nodes)
            {
                int gCost = connectedNodes[i].travelCost + currNode.nodeValue;
                //This will generate the distance values for each node from the end node  and be 65535 for obstacles
                if(gCost is lower than nodeValue of connectedNode)
                {
                    connectedNodes[i].nodeValue = gCost;
                    Add connectedNode[i] to openList
                }
            }
        }
    }
```

## Flow field
Having 2 aformentioned components, a Flow field, synonymously called vector field, can be formed.
The results of the integration field are used to calculate each gridNode's optimal direction towards the goalNode.

Each gridNode's value is compared with all its connected nodes to find the lowest value node, calculating the direction from the current node to the connected node (if the connectedNode's value is lower than the current, thus is closer to the goalNode), and storing that connection within the currentNode, iterating until all node's have a desired direction towards the goalNode.

![Image of Flow field](https://raw.githubusercontent.com/Bhabiji/FlowField/master/Images/Flowfield.JPG)

## Basic units
Now a pathfinding algorithm would be nothing without units testing its efficiency, thus agents were implemented.
This agent script was rather simplistic and initialized agents randomly over the entire grid (repositioning them if the given position was on an impenetrable obstacle).

The node on which an agent resided was gathered (getting the node's index from the worldPos of the agent), then the agent's speed was determined depending on the node's travelCost (higher cost == slower speed). The direction to the goalNode of the node where the agent was positioned on was then used to change the current agent's velocity and
This was done for each agent.

### All these elements combined giving the final result
![Image of Flow field](https://raw.githubusercontent.com/Bhabiji/FlowField/master/Images/ezgif.com-gif-maker.gif)
![Image of Flow field](https://raw.githubusercontent.com/Bhabiji/FlowField/master/Images/GIF2.gif)

## Conclusion/Discussion
As you can see both units walking around vs. units going through the slower material arrive at about the same time. The robust and efficient pathfinding is visualized through the usage of many agents and shows the potential for usage in RTS games and fluid simulations. Positions can be changed constantly, giving a very flexible control.

Bringing me to personal future interests, while less interested in improving the algorithms themselves, I am more interested in learning/mastering the Unity DOTS workflow to implement more traditional algorithms and test them out in Unity DOTS. I have tried this with Flow fields already but with no success, having time constraints at the time of writing and an overall lack of knowledge when it comes to this new workflow, the decision was made to first have a better understanding with traditional implementations of the flowfield. 

Over the next few months of writing this dev-blog I will be familiarising myself more with Unity's DOTS way of programming to implement this in what used to be very computation heavy systems, try to integrate it in new technologies (think VR), and help improve documentation (through youtube, github projects etc.) since documentation is definitely lacking.

Specific, planned out future goals (by order of planned):
- Pong in DOTS
- PAC-MAN in DOTS
- A* Pathfinding in DOTS
- Flow fields in DOTS
- VR hyper-realistic physics sandbox in DOTS <-- big personal portfolio goal

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
## Bibliography
- [flow-fields-and-dynamic-obstacle-avoidance](https://bcaptain.wordpress.com/2017/11/24/flow-fields-and-dynamic-obstacle-avoidance/)
- [howtorts](https://howtorts.github.io/)
