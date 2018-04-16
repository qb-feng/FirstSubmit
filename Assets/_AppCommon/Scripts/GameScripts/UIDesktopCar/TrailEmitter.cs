using UnityEngine;
using System.Collections.Generic;

// Created by Edward Kay-Coles a.k.a Hoeloe
public class TrailEmitter : MonoBehaviour
{

    //Stores all live trails
    private LinkedList<Trail> trails = new LinkedList<Trail>();

    //Parameters
    public float width = 0.1f;
    public float decayTime = 1f;
    public Material material;
    public int roughness = 0;
    public bool softSourceEnd = false;

    //Checks if the most recent trail is active or not
    public bool Active
    {
        get { return (trails.Count == 0 ? false : (!trails.Last.Value.Finished)); }
    }

    // Update is called once per frame
    void Update()
    {
        //Don't update if there are no trails
        if (trails.Count == 0) return;

        //Essentially a foreach loop, allowing trails to be removed from the list if they are finished
        LinkedListNode<Trail> t = trails.First;
        LinkedListNode<Trail> n;
        do
        {
            n = t.Next;
            t.Value.Update();
            if (t.Value.Dead)
                trails.Remove(t);
            t = n;
        } while (n != null);
    }

    /// <summary>
    /// Creates a new trail.
    /// </summary>
    public void NewTrail()
    {
        //Stops emitting the last trail and passes the parameters onto a new one
        EndTrail();
        trails.AddLast(new Trail(transform, material, decayTime, roughness, softSourceEnd, width));
    }

    /// <summary>
    /// Deactivate the last trail if it was already active.
    /// </summary>
    public void EndTrail()
    {
        if (!Active) return;
        trails.Last.Value.Finish();
    }
}