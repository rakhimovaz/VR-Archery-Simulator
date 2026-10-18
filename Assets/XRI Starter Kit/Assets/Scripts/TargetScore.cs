using UnityEngine;
using MikeNspired.XRIStarterKit;

public class TargetScore : MonoBehaviour
{
    public static int TotalScore = 0;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("TARGET COLLISION: " + collision.gameObject.name);

        Arrow arrow = collision.gameObject.GetComponentInParent<Arrow>();

        if (arrow != null)
        {
            TotalScore += 10;
            Debug.Log("TARGET HIT! +10 POINTS");
            Debug.Log("TOTAL SCORE: " + TotalScore);
        }
    }
}