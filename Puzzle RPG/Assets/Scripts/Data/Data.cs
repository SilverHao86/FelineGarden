using UnityEngine;

public class Data : ScriptableObject
{
    public int score = 0;
    public float timePlayed = 0;

    public Data() { }
    public Data(
        int score,
        float timePlayed)
    {
        this.score = score;
        this.timePlayed = timePlayed;
    }
}