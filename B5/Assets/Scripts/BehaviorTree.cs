using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class BehaviorTree : MonoBehaviour
{
	public GameObject participant;
	private BehaviorAgent behaviorAgent;

    private int input = 0;

    private enum StoryArc
    {
        ROOT = 0,
        EVIL,
        HACKER,
        EXPLODE
    }

    private StoryArc currArc = StoryArc.ROOT;

    // Use this for initialization
    void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}
	
	// Update is called once per frame
	void Update ()
	{
        //Debug.Log(input);
        Debug.Log(currArc);
	}

    private Node ExplodeBombFirst()
    {
            return new Sequence(
                new LeafAssert(() => input == 1),
                new LeafInvoke(() => currArc = StoryArc.EXPLODE)
                );
        
    }

    protected Node BuildTreeRoot()
    {
        Node roaming = new DecoratorLoop(
            new Sequence(
                new SelectorParallel(
                    ExplodeBombFirst()
                    )
                )           
        );
		return roaming;
	}

    public void inputButton(int option)
    {
        input = option;
    }

}
