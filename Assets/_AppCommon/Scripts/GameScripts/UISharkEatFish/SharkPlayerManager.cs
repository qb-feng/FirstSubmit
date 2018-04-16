using UnityEngine;
using System.Collections;



enum PlayerState { Enabled, Disabled };
enum PlayerStatus { Idle, MovinUp, MovingDown, Sinking, Crashed };
enum PowerupUsage { Enabled, Disabled };
public class SharkPlayerManager : MonoBehaviour
{

    public float minDepth;                                              //Minimum depth
    public float maxDepth;						                        //Maximum depth
    public float maxRotation;						                    //The maximum rotation of the bird

    public float maxVerticalSpeed;					                    //The maximum vertical speed
    public float depthEdge;					                            //The edge fo the smoothing zone (minDepth- depthEdge and maxDepth - depthEdge)

    private PlayerStatus playerStatus;
    private PlayerState playerState;
    private PowerupUsage powerupUsage;

    private float speed;                                                //The actual vertical speed of the bird
    private float newSpeed;						                        //The new speed of the bird, used at the edges

    private float rotationDiv;										    //A variable used to calculate rotation
    private Vector3 newRotation;	                                    //Stores the new rotation angles

    private float distanceToMax;                                        //The current distance to the maximum depth
    private float distanceToMin;                                        //The current distance to the minimum depth

    private Vector2 startingPos;                                        //Holds the starting position of the bird

    // Use this for initialization
    void Start()
    {
        newRotation = new Vector3();
        startingPos = this.transform.position;

        playerStatus = PlayerStatus.Idle;
        playerState = PlayerState.Disabled;
        powerupUsage = PowerupUsage.Disabled;

        rotationDiv = maxVerticalSpeed / maxRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == PlayerState.Enabled)
        {
            if (playerStatus == PlayerStatus.MovingDown || playerStatus == PlayerStatus.MovinUp)
            {
                //Calculate smooth zone distance
                CalculateDistances();

                //Calculate player movement
                CalculateMovement();

                //Move and rotate the bird
                MoveAndRotate();
            }
            else if (playerStatus == PlayerStatus.Sinking)
            {
                Sink();
            }
        }
    }

    /// <summary>
    /// 检测碰撞是否真确
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        AudioManager.Instance.Play("SharkEatFish");//鲨鱼吃吃小鱼
        if (other.name.Contains("Wrong"))
        {
            Debug.Log("worng!!!");
            UISharkEatFish.Instance.SharkEatWrong(other);
        }
        else if (other.name.Contains("Right"))
        {
            other.transform.parent.gameObject.SetActive(false);
            Debug.Log("right!");
            UISharkEatFish.Instance.SharkEatRight(other);
        }
    }


    //Enables the bird
    public void EnableShark()
    {
        playerStatus = PlayerStatus.Idle;
        playerState = PlayerState.Enabled;
        powerupUsage = PowerupUsage.Enabled;

        StartCoroutine(FunctionLibrary.MoveElementBy(this.transform, new Vector2(0f, 0f), 0.5f));
    }
    //Sets the pause state of the bird
    public void SetPauseState(bool pauseState)
    {
        if (pauseState)
        {
            playerState = PlayerState.Disabled;
        }
        else
        {
            playerState = PlayerState.Enabled;
        }
    }

    //Resets the bird
    public void Reset()
    {
        playerStatus = PlayerStatus.Idle;
        playerState = PlayerState.Disabled;
        powerupUsage = PowerupUsage.Disabled;

        newRotation = new Vector3(0, 0, 0);

        this.transform.position = startingPos;
        this.transform.eulerAngles = newRotation;
    }
    //Revives the bird
    public void Revive()
    {
        StartCoroutine("ReviveProcess");
    }
    //Updates player input
    public void UpdateInput(bool inputActive)
    {
        if (playerStatus == PlayerStatus.Sinking || playerStatus == PlayerStatus.Crashed)
            return;

        if (playerStatus == PlayerStatus.Idle || playerStatus == PlayerStatus.MovingDown || inputActive)
            playerStatus = PlayerStatus.MovinUp;
        else if (playerStatus == PlayerStatus.MovinUp)
            playerStatus = PlayerStatus.MovingDown;
    }


    //Returns true, if a powerup can be activated
    public bool CanUsePowerup()
    {
        return playerState == PlayerState.Enabled && powerupUsage == PowerupUsage.Enabled;
    }

    //Calculate distances to minDepth and maxDepth
    private void CalculateDistances()
    {
        distanceToMax = this.transform.position.y - maxDepth;
        distanceToMin = minDepth - this.transform.position.y;
    }
    //Calculate movement based on input
    private void CalculateMovement()
    {
        //If the sub is moving up
        if (playerStatus == PlayerStatus.MovinUp)
        {
            //Increase speed
            speed += Time.deltaTime * maxVerticalSpeed;

            //If the sub is too close to the minDepth
            if (distanceToMin < depthEdge)
            {
                //Calculate maximum speed at this depth (without this, the sub would leave the gameplay are)
                newSpeed = maxVerticalSpeed * (minDepth - this.transform.position.y) / depthEdge;

                //If the newSpeed is lesser the the current speed
                if (newSpeed < speed)
                    //Make newSpeed the current speed
                    speed = newSpeed;
            }
            //If the sub is too close to the maxDepth
            else if (distanceToMax < depthEdge)
            {
                //Calculate maximum speed at this depth (without this, the sub would leave the gameplay are)
                newSpeed = maxVerticalSpeed * (maxDepth - this.transform.position.y) / depthEdge;

                //If the newSpeed is greater the the current speed
                if (newSpeed > speed)
                    //Make newSpeed the current speed
                    speed = newSpeed;
            }
        }
        //If the sub is moving down
        else
        {
            //Decrease speed
            speed -= Time.deltaTime * maxVerticalSpeed;

            //If the sub is too close to the maxDepth
            if (distanceToMax < depthEdge)
            {
                //Calculate maximum speed at this depth (without this, the sub would leave the gameplay are)
                newSpeed = maxVerticalSpeed * (maxDepth - this.transform.position.y) / depthEdge;

                //If the newSpeed is greater the the current speed
                if (newSpeed > speed)
                    //Make newSpeed the current speed
                    speed = newSpeed;
            }
            //If the sub is too close to the minDepth
            else if (distanceToMin < depthEdge)
            {
                //Calculate maximum speed at this depth (without this, the sub would leave the gameplay are)
                newSpeed = maxVerticalSpeed * (minDepth - this.transform.position.y) / depthEdge;

                //If the newSpeed is lesser the the current speed
                if (newSpeed < speed)
                    //Make newSpeed the current speed
                    speed = newSpeed;
            }
        }
    }
    //Move and rotate the bird based on speed
    private void MoveAndRotate()
    {
        //Calculate new rotation
        newRotation.z = speed / rotationDiv;

        //Apply new rotation and position
        this.transform.eulerAngles = newRotation;
        this.transform.position += Vector3.up * speed * Time.deltaTime;
    }
    //Sinks the bird until it crashes to the sand
    private void Sink()
    {
        float crashDepth = maxDepth - 0.8f;
        float crashDepthEdge = 0.5f;

        float distance = this.transform.position.y - crashDepth;

        //If the sub is too close to minDepth
        if (distanceToMin < depthEdge)
        {
            //Calculate maximum speed at this depth (without this, the sub would leave the gameplay are)
            newSpeed = maxVerticalSpeed * (minDepth - this.transform.position.y) / depthEdge;

            //If the newSpeed is greater the the current speed
            if (newSpeed < speed)
                //Make newSpeed the current speed
                speed = newSpeed;
        }
        //If the distance to the sand is greater than 0.1
        if (distance > 0.1f)
        {
            //Reduce speed
            speed -= Time.deltaTime * maxVerticalSpeed * 0.6f;

            //If the distance to the sand smaller than the crashDepthEdge
            if (distance < crashDepthEdge)
            {
                //Calculate new speed for impact
                newSpeed = maxVerticalSpeed * (crashDepth - this.transform.position.y) / crashDepthEdge;

                //If newSpeed is greater than speed
                if (newSpeed > speed)
                    //Apply new speed to speed
                    speed = newSpeed;
            }

            //Apply the above to the bird
            MoveAndRotate();

        }
        //If the distance to the sand is smaller than 0.1
        else
        {
            //Disable this function from calling, and stop the level
            playerStatus = PlayerStatus.Crashed;
            //levelManager.StopLevel();
        }
    }

    //Enables player vulnerability after time
    private IEnumerator EnableVulnerability(float time)
    {
        //Declare variables, get the starting position, and move the object
        float i = 0.0f;
        float rate = 1.0f / time;

        while (i < 1.0)
        {
            //If the game is not paused, increase t
            if (playerState == PlayerState.Enabled)
                i += Time.deltaTime * rate;

            //Wait for the end of frame
            yield return 0;
        }

    }
}
