
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class DieMovement : MonoBehaviour
{
    [System.Serializable]
    private struct DieFace
    {
        public GameObject face;
        public int value;
    }
    
    private enum DieState
    {
        Idle,
        Rolling,
        Immobile,
        Dead
    }
    
    private DieState state = DieState.Idle;
    private int movesToMake = 0;
    private Vector2 direction = Vector2.zero;
    private int dieValue;
    private AudioSource audioSource;
    private Rigidbody rb;

    private Coroutine rollCoroutine;
    

    [SerializeField] private AudioClip[] rollSounds;
    [SerializeField] private float pitchVariation = 0.1f;
    [SerializeField] private AudioClip fallSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private GameObject model;
    [SerializeField] private DieFace[] faces;
    [SerializeField] private float rollDelay = 0.2f;

    private void Start()
    {
        UpdateDieValue();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private IEnumerator Roll()
    {
        if (state == DieState.Immobile || state == DieState.Dead)
            yield return null;
        
        if (state == DieState.Rolling && movesToMake > 0)
        {
            MoveOnce(direction);
            UpdateDieValue();
            movesToMake--;
            yield return new WaitForSeconds(rollDelay);
            rollCoroutine = StartCoroutine(Roll());
        } else
        {
            state = DieState.Idle;
            DoTileInteractions();
            rollCoroutine = null;
        }
    }

    public void Move(Vector2 direction)
    {
        // Cannot move
        if (state != DieState.Idle)
        {
            return;
        }
        
        // State is idle
        
        state = DieState.Rolling;
        
        this.direction = direction;
        movesToMake = dieValue;

        
        GameManager.Instance.IncreaseRolls();
        
        rollCoroutine = StartCoroutine(Roll());
    }
    
    private void MoveOnce(Vector2 direction)
    {
        // Get strongest move direction
        Vector2 moveDirection;

        if (Math.Abs(direction.x) > Math.Abs(direction.y))
        {
            moveDirection = new Vector2(Math.Sign(direction.x), 0);
        }
        else
        {
            moveDirection = new Vector2(0, Math.Sign(direction.y));
        }
        
        // Move die by value of face in move direction
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y);
        
        // Rotate the model accordingly
        Vector3 rotation = Vector3.zero;
        
        if (moveDirection.x > 0)
        {
            rotation = new Vector3(0, 0, -90);
        }
        else if (moveDirection.x < 0)
        {
            rotation = new Vector3(0, 0, 90);
        }
        else if (moveDirection.y > 0)
        {
            rotation = new Vector3(90, 0, 0);
        }
        else if (moveDirection.y < 0)
        {
            rotation = new Vector3(-90, 0, 0);
        }
        
        model.transform.RotateAround(model.transform.position, rotation, 90);
                
        // Play random roll sound with slightly randomized pitch
        int soundIndex = UnityEngine.Random.Range(0, rollSounds.Length);
        audioSource.pitch = UnityEngine.Random.Range(1 - pitchVariation, 1 + pitchVariation);
        audioSource.PlayOneShot(rollSounds[soundIndex]);
    }

    private void UpdateDieValue()
    {
        // Get the die value of the most upward-facing die in faces array
        dieValue = faces.OrderBy(f => f.face.transform.position.y).Last().value;
    }

    private void DoTileInteractions()
    {
        var below = GetBelow();

        if (below == null)
        {
            // Die is gonna die or maybe win
            StopAllCoroutines();
            StartCoroutine(DelayedDeath());
            rb.isKinematic = false;
            state = DieState.Immobile;
            return;
        }
    }

    private IEnumerator DelayedDeath()
    {
        // Die after a short delay
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(fallSound);
        state = DieState.Dead;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != DieState.Immobile)
        {
            return;
        }

        if (collision.gameObject.CompareTag("goal"))
        {
            StopAllCoroutines();
            GameManager.Instance.Win();
            audioSource.PlayOneShot(winSound);
        }
    }

    private GameObject GetBelow()
    {
        // Return object below this die
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            if (hit.collider.gameObject == gameObject)
                Debug.Log("Hit self");
            return hit.collider.gameObject;
        }
        return null;
    }
}
