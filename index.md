## FlowFields, RTS Pathfinding in Unity
## Introduction
The charm of an RTS game is, for many, having control over a high amount of units to strategically defeat the opponent.
Making sure that the player's actions do not get misinterpreted, pathfinding is implemented, which can be very performance heavy for your machine.

To minimize this performance tax while giving a somewhat efficient path to the player's high amount of units, Flowfields can be utilized.

## Less accuracy, more performance
### Modern viability
While Unity is developing a new workflow (ECS/DOTS), which allows for a simpler approach to multi-threading and high performance results, allowing for more accurate pathfinding with a higher amount of units, it is still in a very experimental (nevertheless interesting) state.
Consequentially Flowfields is so far still a viable option, giving less accuracy but a lower performance tax.

### How?
Originating from the Dijkstra algorithm, A Flowfield exists out of 4 major components:
- Cost field
- Vector field
- Integration field

### Grid
To start your Flowfield journey one must first implement atleast a 2D grid, since the nodes/cells in the grid will be utilized for path calculation.
The grid I created was given a cell size and calculated how many cells would fit in the 2D plane Gameobject carrying the grid.

### Cost Field
An area can sometimes exist out of different materials, some being easier for units to walk over (grass, asphalt etc.) while others might be more tedious for/slow units (water, mud etc.).

With Flowfields each gridnode, or grid cell, has a cost, the lower the cost, the "faster" the path.
This way impenetrable obstacles can be added but also water, mud etc.

In Unity, layer masks can be utilized to easily see if a specific Gridcell is within a specific layer mask, adding its corresponding cost.

<img border="0" alt="Own costfield implementation" src="blob/master/Images/CostField.JPG">
```markdown

```


```markdown
Syntax highlighted code block

# Unity Flowfields
## Header 2
### Header 3

- Bulleted
- List

1. Numbered
2. List

**Bold** and _Italic_ and `Code` text

[Link](url) and ![Image](src)
```

For more details see [GitHub Flavored Markdown](https://guides.github.com/features/mastering-markdown/).

### Jekyll Themes

Your Pages site will use the layout and styles from the Jekyll theme you have selected in your [repository settings](https://github.com/Bhabiji/FlowField/settings). The name of this theme is saved in the Jekyll `_config.yml` configuration file.

### Support or Contact

Having trouble with Pages? Check out our [documentation](https://docs.github.com/categories/github-pages-basics/) or [contact support](https://github.com/contact) and we’ll help you sort it out.
