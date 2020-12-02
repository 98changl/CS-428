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
        NODE1,
        NODE2,
        NODE3,
        NODE4,
        NODE5,
        NODE6,
        NODE7,
        NODE8,
        NODE9,
        NODE10,
        NODE11,
        NODE12,
        NODE13
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
        Debug.Log(input);
	}

    private Node Node1()
    {
        return new Sequence(
                new LeafInvoke(() =>
                {
                    if (input == 1 && currArc == StoryArc.ROOT)
                    {
                        currArc = StoryArc.NODE1;
                        input = 0;
                    }
                }));
    }

    private Node Node2()
    {
        return new SequenceParallel(
                new LeafInvoke(() =>
                {
                    if (input == 2 && currArc == StoryArc.ROOT)
                    {
                        currArc = StoryArc.NODE2;
                        input = 0;
                    }
                }),
                Node4(),
                Node5(),
                Node6()
           );
    }

    private Node Node4()
    {
        return new Sequence(
                new LeafInvoke(() =>
                {
                    if (input == 4 && currArc == StoryArc.NODE2)
                    {
                        currArc = StoryArc.NODE4;
                        input = 0;
                    }
                })
           );
    }

    private Node Node5()
    {
        return new Sequence(
                new LeafInvoke(() =>
                {
                    if (input == 5 && currArc == StoryArc.NODE2)
                    {
                        currArc = StoryArc.NODE5;
                        input = 0;
                    }
                })
           );
    }

    private Node Node6()
    {
        return new Sequence(
                new LeafInvoke(() =>
                {
                    if (input == 6 && currArc == StoryArc.NODE2)
                    {
                        var rand = UnityEngine.Random.Range(0,10);
                        Debug.Log("Rand : " + rand);

                        input = 0;
                        
                        if(rand < 5)
                        {
                            currArc = StoryArc.NODE9;
                        }
                        else
                        {
                            currArc = StoryArc.NODE10;
                        }

                    }
                })
           );
    }

    private Node Node3()
    {
        return new SequenceParallel(
                new LeafInvoke(() =>
                {
                    if (input == 3 && currArc == StoryArc.ROOT)
                    {
                        currArc = StoryArc.NODE3;
                        input = 0;
                    }
                }),
                Node7(),
                Node8()
                );
    }

    private Node Node7()
    {
        return new Sequence(
                new LeafInvoke(() =>
                {
                    if (input == 4 && currArc == StoryArc.NODE3)
                    {
                        currArc = StoryArc.NODE7;
                        input = 0;
                    }
                })
           );
    }

    private Node Node8()
    {
        return new Sequence(
                new LeafInvoke(() =>
                {
                    if (input == 5 && currArc == StoryArc.NODE3)
                    {
                        var rand = UnityEngine.Random.Range(0, 10);
                        Debug.Log("Rand : " + rand);

                        input = 0;

                        if (rand < 5)
                        {
                            currArc = StoryArc.NODE11;
                        }
                        else
                        {
                            currArc = StoryArc.NODE12;
                        }

                    }
                })
           );
    }

    protected Node BuildTreeRoot()
    {
        Node mainStory = new DecoratorLoop(
            new Sequence(
            new SelectorParallel(
                    Node1(),
                    Node2(),
                    Node3()
                )
            )
        );
		return mainStory;
	}

    public void inputButton(int option)
    {
        input = option;
    }

    public int ArcGetter()
    {
        return (int) currArc;
    }

}
