using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelRoadControl : MonoBehaviour
{
    public GameObject BackBlock;
    public GameObject DeleteRoadTrigger;

    // Start is called before the first frame update
    void Start()
    {
        BackBlock.SetActive(false);
        DeleteRoadTrigger.SetActive(true);
    }

    private void OnEnable()
    {
        BackBlock.SetActive(false);
        DeleteRoadTrigger.SetActive(true);
    }
}
