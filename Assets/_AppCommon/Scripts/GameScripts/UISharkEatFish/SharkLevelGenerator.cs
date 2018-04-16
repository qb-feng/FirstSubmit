using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SharkLevelGenerator : MonoBehaviour
{
    public List<SharkMovingLayer> sharkMovingLayers;                      //Holds the moving layer
    public List<ScrollingLayer> scrollingLayers;                //Holds the scrolling layers

    public float speedIncreaseRate;                             //The scrolling speed increase rate per second
    public float distance;                                      //The current distance

    private float speedMultiplier;                              //Holds the speed multiplier
    private float lastSpeedMultiplier;                          //Holds the last speed multiplier

    private bool paused;                                        //True, if the game is paused
    private bool canModifySpeed;                                //True, if the generator can modify the current scrolling speed
    // Use this for initialization
    void Start()
    {
        speedMultiplier = 1;
        paused = true;
    }

    // Update is called once per frame
    void Update()
    {
        //If the game is not paused
        if (!paused)
        {
            //If the speed can be modified
            if (canModifySpeed)
            {
                //Increase scrolling speed
                speedMultiplier += speedIncreaseRate * Time.deltaTime;
            }

            //Increase distance
            distance += 10 * speedMultiplier * Time.deltaTime;

            //Pass speed multiplier to the layers
            foreach (SharkMovingLayer item in sharkMovingLayers)
                item.UpdateSpeedMultiplier(speedMultiplier);

            foreach (ScrollingLayer item in scrollingLayers)
                item.UpdateSpeedMultiplier(speedMultiplier);
        }
    }
    //Changes the speed multiplier to newValue in time
    IEnumerator ChangeScrollingMultiplier(float newValue, float time, bool enableIncrease)
    {
        //Declare variables, get the starting position, and move the object
        float i = 0.0f;
        float rate = 1.0f / time;

        float startValue = speedMultiplier;

        while (i < 1.0)
        {
            //If the game is not paused, increase t, and scale the object
            if (!paused)
            {
                i += Time.deltaTime * rate;
                speedMultiplier = Mathf.Lerp(startValue, newValue, i);
            }

            //Wait for the end of frame
            yield return 0;
        }
    }


    //Resume the generator after a revive
    public void ContinueGeneration()
    {
        StartCoroutine(ChangeScrollingMultiplier(lastSpeedMultiplier, 0.5f, true));
    }
    //Resets the level generator
    public void Reset()
    {

        paused = true;
        canModifySpeed = false;

        speedMultiplier = 1;
        distance = 0;

        StopAllCoroutines();

        foreach (SharkMovingLayer item in sharkMovingLayers)
            item.Reset();

        foreach (ScrollingLayer item in scrollingLayers)
            item.SetPauseState(true);

    }
    //Starts the level Generator
    public void StartToGenerate()
    {
        paused = false;
        canModifySpeed = true;

        //playTriggerer.SetActive(false);

        foreach (SharkMovingLayer item in sharkMovingLayers)
            item.StartGenerating();

        foreach (ScrollingLayer item in scrollingLayers)
            item.SetPauseState(false);

    }
    //Stops the level generaton under time
    public void StopGeneration(float time)
    {
        lastSpeedMultiplier = speedMultiplier;
        canModifySpeed = false;

        StartCoroutine(ChangeScrollingMultiplier(0, time, false));
    }
    //Set the pause state of the level generator in time
    public void SetPauseState(bool state)
    {
        paused = state;

        foreach (SharkMovingLayer item in sharkMovingLayers)
            item.SetPauseState(state);

        foreach (ScrollingLayer item in scrollingLayers)
            item.SetPauseState(state);
    }
    //Return the current distance as an int
    public int CurrentDistance()
    {
        return (int)distance;
    }
    //Starts the extra speed powerup effect
    public void StartExtraSpeed(float newSpeed)
    {
        lastSpeedMultiplier = speedMultiplier;
        canModifySpeed = false;

        speedMultiplier = newSpeed;
    }
    //Stops the extra speed powerup effect
    public void EndExtraSpeed()
    {
        speedMultiplier = lastSpeedMultiplier;
        canModifySpeed = true;
    }
}
