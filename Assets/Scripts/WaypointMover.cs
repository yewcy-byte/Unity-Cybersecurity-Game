using UnityEngine;
using System.Collections;

public class WaypointMover : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Transform[] waypoint;
    private int currentWaypointIndex;
    private bool isWaiting;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        waypoint = new Transform[waypointParent.childCount];

        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoint[i] = waypointParent.GetChild(i);
        }
    }

    void Update()
    {
        if (PauseController.IsGamePaused || isWaiting)
        {
            animator.SetBool("isWalking", false);
            return;
        }
            

        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Transform target = waypoint[currentWaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;




        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        animator.SetFloat("InputX", direction.x);
        animator.SetFloat("InputY", direction.y);
        animator.SetBool ("isWalking", direction.magnitude > 0f);


        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAtWayPoint());
        }
    }

    IEnumerator WaitAtWayPoint()
    {
        isWaiting = true;
        animator.SetBool("isWalking", false);

        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = loopWaypoints
            ? (currentWaypointIndex + 1) % waypoint.Length
            : Mathf.Min(currentWaypointIndex + 1, waypoint.Length - 1);

        isWaiting = false;
    }
}
