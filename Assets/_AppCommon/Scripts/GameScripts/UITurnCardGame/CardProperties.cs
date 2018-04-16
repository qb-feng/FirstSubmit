using UnityEngine;

public class CardProperties : MonoBehaviour
{
    int pair = 0;
    public int Pair
    {
        get
        {
            return pair;
        }
        set
        {
            pair = value;
        }
    }

    bool solved = false;
    public bool Solved
    {
        get
        {
            return solved;
        }
        set
        {
            solved = value;
        }
    }

    bool selected = false;
    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            selected = value;
        }
    }
    public string soundA = null;
    public string SoundA
    {
        get
        {
            return soundA;
        }
        set
        {
            soundA = value;
        }
    }

    public string soundB = null;
    public string SoundB
    {
        get
        {
            return soundB;
        }
        set
        {
            soundB = value;
        }
    }

    private ConfigWordLibraryModel word = null;
    public ConfigWordLibraryModel Word
    {
        get
        {
            return word;
        }
        set
        {
            word = value;
        }
    }
}

