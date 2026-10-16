
using UnityEngine;

public class ArcheryTargetScore : MonoBehaviour
{
    public int totalScore = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.name.Contains("Arrow"))
            return;

        Vector3 hitPoint = collision.GetContact(0).point;
        Vector3 localHit = transform.InverseTransformPoint(hitPoint);

        float distance = Mathf.Sqrt(
            localHit.x * localHit.x +
            localHit.y * localHit.y
        );

        int points;

        if (distance < 0.15f)
            points = 10;
        else if (distance < 0.35f)
            points = 5;
        else
            points = 2;

        totalScore += points;

        Debug.Log("Hit! +" + points +
                  " | Total Score: " + totalScore);
    }
}
