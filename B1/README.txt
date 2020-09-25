B1 Report

a. Describe your breaking mechanism for agents.

b. Describe a way for implementing how an agent can avoid obstacles without carving.

c. Explain the difference in behavior between carving and non-carving options for a NavMeshObstacle. When and why should you use carving? What is the issue with making all obstacles carving? What is the issue with making all obstacles non-carving?
  - Carving allows the obstacle to carve space around it into the nav mesh when it is stationary. Therefore, when a lot of obstacles are close to each other possibly causing obstructions of the path, carving will allow the path finder to better navigate the navigation mesh by carving out any navigation mesh that is too close. You should use carving on objects that can be moved or will obstruct the pathway of the player. Making all of the obstacles carving may end up carving out too much of the navigation mesh and not allow the player to move around or it may cause the pathfinder to not be able to find an optimal path around the obstacles. Making all of the obstacles non-carving may cause the pathfinder to get stuck on obstacles if there are too many blocking an optimal path.
