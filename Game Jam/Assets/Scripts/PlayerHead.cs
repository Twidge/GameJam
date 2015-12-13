using UnityEngine;

public class PlayerHead : Head
{
    private string _moveRightButton;
    private string _moveLeftButton;

    private float _timeSincePressLeftButton;
    private float _timeSincePressRightButton;

    private float _timeGivenToPressBothButtons;

    // Use this for initialization
    private void Start()
    {
        RestrictedByBorders = true;
        _timeGivenToPressBothButtons = 0.2f;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (_timeSincePressLeftButton < _timeGivenToPressBothButtons)
            _timeSincePressLeftButton += Time.deltaTime;

        if (_timeSincePressRightButton < _timeGivenToPressBothButtons)
            _timeSincePressRightButton += Time.deltaTime;

        if (ControlledMovementAllowed)
        {
            if (Input.GetKey(_moveLeftButton))
            {
                Turn(TurnDirection.LEFT);
            }
            else if (Input.GetKey(_moveRightButton))
            {
                Turn(TurnDirection.RIGHT);
            }

            if (Input.GetKeyDown(_moveLeftButton))
            {
                _timeSincePressLeftButton = 0;
            }

            if (Input.GetKeyDown(_moveRightButton))
            {
                _timeSincePressRightButton = 0;
            }

            if (_timeSincePressLeftButton < _timeGivenToPressBothButtons && _timeSincePressRightButton < _timeGivenToPressBothButtons)
            {
                if (HasFireball)
                {
                    Debug.Log("FIRE");
                    UseFireball();
                }
            }
        }
    }

    public void SetControlsAndStartingDirection(string moveLeftButton, string moveRightButton, float startingDirection)
    {
        _moveLeftButton = moveLeftButton;
        _moveRightButton = moveRightButton;
        TravelAngle = startingDirection*Mathf.Deg2Rad;
    }
}