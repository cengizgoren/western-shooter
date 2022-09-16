// MoveTo.cs
using UnityEngine;
using UnityEngine.AI;

public class DummyEnemyAnimator : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Animate();
    }

    private void Animate()
    {
        float currentHorizontalSpeed = new Vector3(agent.velocity.x, 0.0f, agent.velocity.z).magnitude;
        Vector3 relativeVelocity = transform.InverseTransformDirection(agent.velocity);

        if (animator)
        {
            animator.SetFloat("SpeedZ", relativeVelocity.z);
            animator.SetFloat("SpeedX", relativeVelocity.x);
            animator.SetFloat("SpeedHorizontal", currentHorizontalSpeed);
        }
    }
}