using UnityEngine;

public class TriggerPull : MonoBehaviour
{
    // This script determines if the participant hit the target or the penalty 
    // or if they passed the object all together.

    private GameObject trigger;
    private ControlLevel_Trials controlLevels;
    public GameObject manager;
    public bool passedRadius;
    public bool targetTouched;
    public bool penaltyTouched;

    public void Start()
    {
        trigger = GameObject.FindGameObjectWithTag("Trigger");
        manager = GameObject.FindGameObjectWithTag("Manager");
        controlLevels = manager.GetComponent<ControlLevel_Trials>();
        passedRadius = false;
        targetTouched = false;
        penaltyTouched = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Object"))
        {
            trigger.SetActive(true);
            targetTouched = true;
        }

        if (other.gameObject.CompareTag("PenaltyonTarget"))
        {
            trigger.SetActive(true);
            penaltyTouched = true;
        }

        if (other.gameObject.CompareTag("passedRadius"))
        {
            passedRadius = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("passedRadius"))
        {
            passedRadius = true;
        }
    }
}
