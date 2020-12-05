using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class BehaviorTree : MonoBehaviour
{
    public GameObject participant;
    public GameObject evil;
    public GameObject hacker;
    public GameObject coin;
    public Transform Spawn1, Spawn2, Spawn3;

    public DialogueManager dialogue;

    private BehaviorAgent behaviorAgent;

    public int input = 0;

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
    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(input);
        Debug.Log(currArc);
        //Debug.Log(input);
    }

    #region CheckArcs

    private Node CheckStartArc()
    {
        return new Sequence(
                new LeafAssert(() => (StoryArc)input == StoryArc.ROOT),
                new LeafInvoke(() => currArc = StoryArc.ROOT)
                );
    }

    private Node CheckButtonArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.ROOT),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE1),
                new LeafInvoke(() => currArc = StoryArc.NODE1)
                );
    }

    private Node CheckComplyArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.ROOT),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE2),
                new LeafInvoke(() => currArc = StoryArc.NODE2)
                );
    }

    private Node CheckHackerArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.ROOT),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE3),
                new LeafInvoke(() => currArc = StoryArc.NODE3)
                );
    }

    private Node CheckRefuseArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.NODE2),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE4),
                new LeafInvoke(() => currArc = StoryArc.NODE4)
                );
    }

    private Node CheckPickpocketArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.NODE2),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE5),
                new LeafInvoke(() => currArc = StoryArc.NODE5)
                );
    }

    private Node CheckThreatenKeysArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.NODE2),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE6),
                new LeafInvoke(() => currArc = StoryArc.NODE6)
                );
    }

    private Node CheckCoinsArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.NODE3),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE7),
                new LeafInvoke(() => currArc = StoryArc.NODE7)
                );
    }

    private Node CheckThreatenDefuseArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.NODE3),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE8),
                new LeafInvoke(() => currArc = StoryArc.NODE8)
                );
    }

    private Node CheckLoseKeysArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.NODE6),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE9),
                new LeafInvoke(() => currArc = StoryArc.NODE9)
                );
    }

    private Node CheckWinKeysArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.NODE6),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE10),
                new LeafInvoke(() => currArc = StoryArc.NODE10)
                );
    }

    private Node CheckLoseDefuseArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.NODE8),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE11),
                new LeafInvoke(() => currArc = StoryArc.NODE11)
                );
    }

    private Node CheckWinDefuseArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.NODE8),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE12),
                new LeafInvoke(() => currArc = StoryArc.NODE12)
                );
    }

    private Node CheckDeliverArc()
    {
        return new Sequence(
                new LeafAssert(() => currArc == StoryArc.NODE10),
                new LeafAssert(() => (StoryArc)input == StoryArc.NODE13),
                new LeafInvoke(() => currArc = StoryArc.NODE13)
                );
    }

    #endregion

    #region Arcs

    private Node StartArc()
    {
        return new Sequence(
                CheckStartArc(),
                WaitForInput(),
                new Selector(
                    ButtonArc(),
                    ComplyArc(),
                    HackerArc()
                    ));
    }

    private Node ButtonArc()
    {
        return new Sequence(
                CheckButtonArc(),
                GameOver()
                );
    }

    private Node ComplyArc()
    {
        return new Sequence(
                CheckComplyArc(),
                WaitForInput(),
                new Selector(
                    RefuseArc(),
                    PickpocketArc(),
                    ThreatenKeysArc()
                ));
    }

    private Node HackerArc()
    {
        return new Sequence(
                CheckHackerArc(),
                WaitForInput(),
                new Selector(
                    CoinsArc(),
                    ThreatenDefuseArc()
                ));
    }

    private Node RefuseArc()
    {
        return new Sequence(
                CheckRefuseArc(),
                new LeafInvoke(() => currArc = StoryArc.NODE1),
                GameOver()
                );
    }

    private Node PickpocketArc()
    {
        return new Sequence(
                CheckPickpocketArc(),
                WaitForInput()
                );
    }

    private Node ThreatenKeysArc()
    {
        return new Sequence(
                CheckThreatenKeysArc(),
                new LeafInvoke(() =>
                {
                    var rand = UnityEngine.Random.Range(0, 10);
                    if (rand < 5)
                        input = (int)StoryArc.NODE9;
                    else
                        input = (int)StoryArc.NODE10;
                })
                );
    }

    private Node CoinsArc()
    {
        return new Sequence(
                CheckCoinsArc(),
                MakeCoins(),
                WaitForInput()
                );
    }

    private Node ThreatenDefuseArc()
    {
        return new Sequence(
                CheckThreatenDefuseArc(),
                new LeafInvoke(() =>
                {
                    var rand = UnityEngine.Random.Range(0, 10);
                    if (rand < 5)
                        input = (int)StoryArc.NODE11;
                    else
                        input = (int)StoryArc.NODE12;
                })
                );
    }

    private Node LoseKeysArc()
    {
        return new Sequence(
                CheckLoseKeysArc(),
                GameOver()
                );
    }

    private Node WinKeysArc()
    {
        return new Sequence(
                CheckWinKeysArc(),
                new LeafInvoke(() => input = (int)StoryArc.NODE13)
                );
    }

    private Node LoseDefuseArc()
    {
        return new Sequence(
                CheckLoseDefuseArc(),
                GameOver()
                );
    }

    private Node WinDefuseArc()
    {
        return new Sequence(
                CheckWinDefuseArc(),
                GameOver()
                );
    }

    private Node DeliverArc()
    {
        return new Sequence(
                CheckDeliverArc(),
                GameOver()
                );
    }

    #endregion

    #region Functions

    private Node GameOver()
    {
        return new Sequence(
            new LeafInvoke(() => Time.timeScale = 0f),
            new LeafInvoke(() => { return RunStatus.Failure; })
            );
    }

    private Node MakeCoins()
    {
        return new Sequence(
                new LeafInvoke(() => Instantiate(coin, Spawn1.position, Spawn1.rotation)),
                new LeafInvoke(() => Instantiate(coin, Spawn2.position, Spawn2.rotation)),
                new LeafInvoke(() => Instantiate(coin, Spawn3.position, Spawn3.rotation))
                );
    }

    private Node WaitForInput()
    {
        return new DecoratorInvert(
                new DecoratorLoop(
                    new Sequence(
                        new LeafInvoke(
                            () => {
                                if (input != (int)currArc)
                                {
                                    return RunStatus.Failure;
                                }
                                else
                                {
                                    return RunStatus.Running;
                                }
                            }
                        ),
                        new LeafInvoke(() => Debug.Log(currArc))
                    )
                )
            );
    }

    #endregion

    #region IfCode
    /*
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
                    if (input == 1 && currArc == StoryArc.NODE2)
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
                    if (input == 4 && currArc == StoryArc.NODE2)
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
                    if (input == 5 && currArc == StoryArc.NODE2)
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
    */
    #endregion

    protected Node BuildTreeRoot()
    {
        return new Sequence(
                    StartArc()
                    );
        /*
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
        */
    }

    public void inputButton(int option)
    {
        input = option;
    }

    public int ArcGetter()
    {
        return (int)currArc;
    }

}
