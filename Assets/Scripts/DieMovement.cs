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
    
    // Enum used to keep track of the state of the die as the game is played
    private enum DieState
    {
        Idle,
        Rolling,
        Immobile,
        Dead
    }
    
    // Private fields for dices
    private DieState state = DieState.Idle;
    private int movesToMake = 0;
    private Vector2 direction = Vector2.zero;
    private int dieValue;
    private AudioSource audioSource;
    private Rigidbody rb;

    // Coroutines
    private Coroutine rollCoroutine;
    
    // Serialized fields
    [SerializeField] private AudioClip[] rollSounds;
    [SerializeField] private float pitchVariation = 0.1f;
    [SerializeField] private AudioClip fallSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private GameObject model;
    [SerializeField] private DieFace[] faces;
    [SerializeField] private float rollDelay = 0.2f;

    // Initialize starting state of die   
    private void Start()
    {
        UpdateDieValue();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Roll die 
    private IEnumerator Roll()
    {
        while(movesToMake > 0)
        {
            movesToMake = movesToMake - 1;

            MoveOnce(direction);
            yield return new WaitForSeconds(rollDelay);
        }

        state = DieState.Idle;
        DoTileInteractions(); //can recursively be called while rolling down a hill
        UpdateDieValue();
        rollCoroutine = null;
    }

    // Public funtion called to move the die
    public void Move(Vector2 direction)
    {
        // Cannot move
        if (state != DieState.Idle)
        {
            return;
        }
        
        // State is idle and is ready to move
        state = DieState.Rolling;
        
        this.direction = direction;
        movesToMake = dieValue;

        GameManager.Instance.IncreaseRolls();
        
        rollCoroutine = StartCoroutine(Roll());
    }
    
    // Function used to move the die in a single direction once in the direction specified by the 2d direction vector
    private void MoveOnce(Vector2 direction)
    {
        var below = GetBelow();

        // Sets the direction of movement of the die as the strongest move direction
        Vector2 moveDirection;
        bool strongestDirection = (Math.Abs(direction.x) > Math.Abs(direction.y)); // true == X is strongest, false == Y is strongest
        moveDirection = (strongestDirection) ? new Vector2(Math.Sign(direction.x), 0): new Vector2(0, Math.Sign(direction.y));

        //check if tile you're moving to has change in incline
        float nextY= 0;
        float nextIncline = 0;
        float currentIncline = 0;
        int incline = 0; //steepness of slope in degrees

        // OverlapBox used to find the next tile's height and incline
        Collider[] nextTile = Physics.OverlapBox((transform.position + new Vector3(moveDirection.x, -0.5f, moveDirection.y)), new Vector3(0.45f, 0.5f, 0.45f));
        if (nextTile.Length > 0)
        {
            nextY = nextTile[0].transform.position.y + 0.5f - transform.position.y;
            nextIncline = (strongestDirection) ? nextTile[0].transform.rotation.z : nextTile[0].transform.rotation.x;
        }

        // Get current tile's incline
        if (below != null)
        {
            currentIncline = (strongestDirection) ? below.transform.rotation.z : below.transform.rotation.x; 
        }

        // Rotates the model according to rotationAxis, rotationPoint and difference in incline
        Vector3 rotationDirection = Vector3.zero;
        Vector3 rotationPoint = Vector3.zero;
        
        if (moveDirection.x > 0)
        {
            rotationDirection = new Vector3(0, 0, -1);
            rotationPoint = new Vector3(model.transform.position.x, model.transform.position.y, model.transform.position.z - 0.5f);
        }
        else if (moveDirection.x < 0)
        {
            rotationDirection = new Vector3(0, 0, 1);
            rotationPoint = new Vector3(model.transform.position.x, model.transform.position.y, model.transform.position.z + 0.5f);
        }
        else if (moveDirection.y > 0)
        {
            rotationDirection = new Vector3(1, 0, 0);
            rotationPoint = new Vector3(model.transform.position.x + 0.5f, model.transform.position.y, model.transform.position.z);
        }
        else if (moveDirection.y < 0)
        {
            rotationDirection = new Vector3(-1, 0, 0);
            rotationPoint = new Vector3(model.transform.position.x - 0.5f, model.transform.position.y, model.transform.position.z);
        }

        // Compares incline and height differences between current tile and the next tile to figure out how much it should rotate.
        float inclineDif = (Math.Abs(currentIncline) - Math.Abs(nextIncline));
        int isUpHill = (nextY > 0) ? -1 : 1;

        if (inclineDif > 0)
        {
            incline = isUpHill * 25;
        }
        else if (inclineDif < 0)
        {
            incline = isUpHill * -25;
        }

        // Rotates model based on current transform position, rotationAxis, and incline
        model.transform.RotateAround(transform.position, rotationDirection, 90 - (incline));

        // Move die by value of face in move direction
        transform.position += new Vector3(moveDirection.x, nextY, moveDirection.y);

        // Play random roll sound with slightly randomized pitch
        int soundIndex = UnityEngine.Random.Range(0, rollSounds.Length);
        audioSource.pitch = UnityEngine.Random.Range(1 - pitchVariation, 1 + pitchVariation);
        audioSource.PlayOneShot(rollSounds[soundIndex]);
    }

    // Get the die value of the most upward-facing die in faces array
    private void UpdateDieValue()
    {
        dieValue = faces.OrderBy(f => f.face.transform.position.y).Last().value;
    }

    // Checks if there is any interactions to be performed with the tile underneath the die
    private void DoTileInteractions()
    {
        var below = GetBelow();

        if (below == null)
        {
            // Die is gonna die
            StopAllCoroutines();
            StartCoroutine(DelayedDeath());
            rb.isKinematic = false;
            state = DieState.Immobile;
            return;
        }
    }

    // Die dies after a short delay
    private IEnumerator DelayedDeath()
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(fallSound);
        state = DieState.Dead;
    }

    // Checks to see if the level is won
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

    // Finds and returns GameObject directly below die. Returns null if no object is found. 
    private GameObject GetBelow()
    {
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
