Assignment B1 Report
Unity Navigation Basics

a. Describe your braking mechanism for agents.
  - Agents in the program have auto braking on where once an agent encounters an obstacle, it stops and recalculates a new path for the agent to get to the destination. The braking mechanism is simple as the agent will stop and recalculate a new path once a game object enters the agent's nav mesh agent obstacle avoidance radius. This mechanism will only be applied to objects on the NavMesh.

b. Describe a way for implementing how an agent can avoid obstacles without carving.
  - A method of avoiding obstacles without carving is to take into account agent speed. Without carving, if an agent encounters an obstacle on the NavMesh that's blocking the path, the agent will slow down significantly or stop due to the obstruction. By implementing a tracker for agent speed in Update(), once an agent's speed is less than the max speed of an agent or has hit zero, the AgentScript can assume that an obstacle has disrupted the agent. Once that condition has been fulfilled, if the current position of the agent is not the desired destination, then the AgentScript will tell the program to recalculate a new path for the agent.

c. Explain the difference in behavior between carving and non-carving options for a NavMeshObstacle. When and why should you use carving? What is the issue with making all obstacles carving? What is the issue with making all obstacles non-carving?
  - Carving allows the obstacle to carve space around it into the nav mesh when it is stationary. Therefore, when a lot of obstacles are close to each other possibly causing obstructions of the path, carving will allow the path finder to better navigate the navigation mesh by carving out any navigation mesh that is too close. You should use carving on objects that can be moved or will obstruct the pathway of the player. Making all of the obstacles carving may end up carving out too much of the navigation mesh and not allow the player to move around or it may cause the pathfinder to not be able to find an optimal path around the obstacles. Making all of the obstacles non-carving may cause the pathfinder to get stuck on obstacles if there are too many blocking an optimal path.

Extra credit opportunities:

a. Create an adversarial agent which all normal agents avoid. The more apparent their avoidance, the more points will be given. You should be able to select and move adversarial agents just like your normal agents (using the mouse’s left and right clicks). You are not permitted to use a large-radius collider to push normal agents away.
  - The adversarial agents have almost the exact same functionalities as a standard agent. The major difference between an adversarial agent and standard agent is their obstacle avoidance radius.
  - (This is in parenthesis cause I think it needs changes. The adversarial agent also has a much higher obstacle avoidance priority compared to other agents and some obstacles. As such, the standard agents would be much less likely to ignore an adversarial agent should an agent encounter one.)

b. Extract the navigation mesh into a graph and implement your own navigation from scratch using A*. You must be able to navigate to positions that are not points on the triangulation, and your paths must be as straight as possible.
  - This has not been implemented.
