using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager instance;

    public Door exitDoor;
    public int notesCollected = 0;
    public int requiredNotes = 4;

    public bool survivorRescued = false;
    public enum ObjectiveType
    {
        CollectNotes,
        DestroyNodes
    }

    [Header("Level Objective")]
    public ObjectiveType objectiveType;

    [Header("Progress")]
    public int nodesDestroyed = 0;
    public int totalNodes = 3;

    [Header("Objective Texts")]
    public string initialObjectiveText = "Destroy network nodes";
    public string completedObjectiveText = "Proceed to the Boss Chamber";

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (objectiveType == ObjectiveType.CollectNotes)
        {
            UIController.instance.UpdateObjective(
                "Collect 4 Scraped Notes and Rescue the Survivor"
            );
        }
        else if (objectiveType == ObjectiveType.DestroyNodes)
        {
            UpdateObjectiveUI();
        }
    }

    public void NodeDestroyed()
    {
        if (objectiveType != ObjectiveType.DestroyNodes)
            return;

        nodesDestroyed++;

        if (nodesDestroyed >= totalNodes)
        {
            UIController.instance.ShowMessage("All nodes destroyed! The Overseer is weakened.");
            UIController.instance.UpdateObjective(completedObjectiveText);
        }
        else
        {
            UpdateObjectiveUI();
        }
    }

    private void UpdateObjectiveUI()
    {
        if (objectiveType != ObjectiveType.DestroyNodes)
            return;

        string fullObjective = "Destroy Network Nodes (" + nodesDestroyed + "/" + totalNodes + ")";

        UIController.instance.UpdateObjective(fullObjective);
    }

    public bool CanExit()
    {
        return notesCollected >= requiredNotes && survivorRescued;
    }

    public void CollectNote()
    {
        if (objectiveType != ObjectiveType.CollectNotes)
            return;

        notesCollected++;

        UIController.instance.ShowMessage("Scraped Notes: " + notesCollected + "/" + requiredNotes);

        CheckLevel2Complete();
    }

    public void RescueSurvivor()
    {
        if (objectiveType != ObjectiveType.CollectNotes)
            return;

        survivorRescued = true;

        UIController.instance.ShowMessage("Survivor rescued!");

        CheckLevel2Complete();
    }

    private void CheckLevel2Complete()
    {
        if (notesCollected >= requiredNotes && survivorRescued)
        {
            UIController.instance.ShowMessage("Objectives complete! Proceed to the exit.");
            UIController.instance.UpdateObjective("Go to the Exit");

            exitDoor.Unlock();
        }
    }

}