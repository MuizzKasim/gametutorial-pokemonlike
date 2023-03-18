using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    public float moveSpeed;
    public bool allowDiagonalMovement = false;
    public LayerMask solidObjectsLayer;
    public LayerMask longGrassLayer;

    public event Action onEncountered;

    private bool isMoving;
    private Vector2 input;

    private Animator animator; 
    
    private void Awake(){ animator = GetComponent<Animator>();}

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal"); //Input.GetAxisRaw always give between -1 to 1. Hence corresponds to grid-like input.
            input.y = Input.GetAxisRaw("Vertical");

            //Disables diagonal
            if(allowDiagonalMovement == false)
                if (input.x != 0) input.y = 0;

            if(input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if(isWalkable(targetPos)) 
                    StartCoroutine(Move(targetPos));
            }
        }
        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        CheckForEncounters();
    }

    private bool isWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null) 
        { 
            return false; 
        } else { 
            return true;
        }
    }

    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, longGrassLayer) != null)
        {
            if(UnityEngine.Random.Range(1,101) <= 10)
            {
                Debug.Log("BattleStarted!");
                animator.SetBool("isMoving", false);
                onEncountered();   
            }
        }
    }
}
