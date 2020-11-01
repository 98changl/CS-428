Assignment B3 Report  
Unity Collisions

Data Structure Implementation Description:

For potential collisions, we implemented a QuadTree data structure. The QuadTree initializes by taking in the topLeft 
and bottomRight coordinates of the prism manager and setting it as the bounds of the root node. The initializeTree() is 
then called to set the depth of the QuadTree to determine the number of child nodes to build and the coverage of the
manager desired. We add prisms to the tree by calling the insertNode() method. This method checks each quadrant of the 
quad tree to see whether the four corners of the prism in question are inside of each quadrant. It does this check by 
checking if there is an overlap in coordinates using the topLeft, topRight, bottomLeft, and bottomRight coordinate values 
of both the QuadTreeNode and the prism. If true, the function will add that prism to the quadrant and then call insertNode()
on the child nodes of the parent node. If the child nodes are null, the insertion ends.

Once every prism has been inserted into the QuadTree, PotentialCollisions() checks every prism in the prisms list for a 
potential collision by calling getCollisionList() for the QuadTree. The method getCollisionList() checks the QuadTreeNode 
for the prism and returns a list of prisms that are in the same quadrant as the node being searched for. If the prism is 
not in the prisms list for the QuadTreeNode, it returns. Otherwise, the method recursively calls getCollisionList() on its 
child nodes until the QuadTreeNode's child nodes are null. If the QuadTreeNode has no child QuadTreeNodes, the method 
returns the list of prisms inside that QuadTreeNode. Given the list of collisions from the child nodes, the parent QuadTreeNode
then merges the results of the recursive call to the collisions list and removes the searched prism from the prisms list. 
This allows getCollisionList() to give the prism manager an accurate list of collisions as it pulls prisms from the deepest 
QuadTreeNodes which contain the most accurate coordinate data for prisms.  

Efficiency:

This implementation give an average case of O(nlogn) as for each prism n, the search for potential collisions with the method
getCollisionList() should take O(logn) time if the prism doesn't occupy a large number of quadrants. This is because only child
nodes containing the prism being searched for will be traversed. The average time for getCollisionList() can also be confirmed 
to be O(logn) as the method removes the prism it is searching for from the QuadNode's prisms list once found removing the 
possibility of repeat prism searches from the next getCollisionList() call on the root QuadTreeNode. Using this method gives us 
a worst case efficiency of O(n^2) in the cases where a large prism is colliding with another large prism. This is because 
overlapping prisms can cover multiple quadrants in the grid resulting in multiple QuadTreeNode searches resulting in the time 
similar to O(n^2).

(1)
The data structure behaves like a grid with each node representing a quadrant of the grid. The initial root node is the square 
covering the prism region + 2. Each subsequent child of the parent QuadTreeNode covers the topLeft, bottomRight, bottomLeft,
and topRight quadrants of the square. This continues up until a depth determined by the prismCount has been reached.

The data structure calculates the prisms's bounding volumes as Axis-Aligned Bounding Boxes. It achieves this by calculating the
prism's topLeft and bottomRight coordinate values using the list of points that define each and every prism.

(2)
The data structure stops the query when the points of a prism's bounding box are not found inside the bounds of the QuadTreeNode 
or when the max depth of the tree has been reached. These bounds for the QuadTreeNode are created from the topLeft and 
bottomRight (x,y,z) coordinates set during the initialization of the tree. In the cases where the depth of the QuadTree has not
been reached yet, the data structure stops the neighbor queries when the QuadTreeNode discovers that the prism is not within the
bounds of said node. The QuadTreeNode will then return to the parent, ceasing any more queries into the children of that quadrant.

For example, let's say prism A is located in the bottomRight region of the prism region. During the getCollisionList() search, 
while the method checks each child node for prism A, the program will find that prism A is not in the topLeft, topRight, and
bottomLeft child nodes of the parent QuadTreeNode. As such, the query will only continue the search in the bottomRight child node
and stop the query for the other children and their respective child nodes.


Extra Credit Attempts:
a. The data structure implemented from scratch in (4) would be the QuadTreeNode. You can find this implementation in QuadTreeNode.cs.

