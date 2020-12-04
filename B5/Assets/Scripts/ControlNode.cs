using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;

public class ControlNode : NodeGroup
{
    public static int newCoins;

    void Update()
    {
        newCoins = CoinScript.coins;
    }
    public ControlNode(params Node[] children)
            : base(children)
        {
        }

    public override IEnumerable<RunStatus> Execute()
    {
        foreach (Node node in this.Children)
        {
            this.Selection = node;
            node.Start();

            RunStatus result;
            while ((result = this.TickNode(node)) == RunStatus.Running)
                yield return RunStatus.Running;

            node.Stop();

            this.Selection.ClearLastStatus();
            this.Selection = null;
            
            // If already collected 3 coins, can go to NODE 7

            if( newCoins == 3)
            {
                yield return RunStatus.Success;
                yield break;
            }

            //*/

            yield return RunStatus.Running;
        }
        yield return RunStatus.Success;
        yield break;
    }
}
