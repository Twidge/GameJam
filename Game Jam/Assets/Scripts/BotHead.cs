using UnityEngine;
using System.Collections;

public class BotHead : Head {

	// Use this for initialization
	void Start ()
	{
	    RestrictedByBorders = false;
	    ControlledMovementAllowed = true;
	    TravelAngle = Random.Range(0, FULL_TURN) * Mathf.Deg2Rad;
	}

    // Update is called once per frame
    public override void Update()
    {
        base.Update();


    }
}
