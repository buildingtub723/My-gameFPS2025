using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("AI/Actions")]
public class FindPlayerAction : ActionTask
{
    // Blackboard variable to store the player reference
    public BBParameter<GameObject> player;

    protected override void OnExecute()
    {
        // Find the scene player by tag
        GameObject scenePlayer = GameObject.FindWithTag("Player");
        if (scenePlayer != null)
        {
            player.value = scenePlayer; // store in blackboard
        }
        EndAction(true); // finish this action
    }
}