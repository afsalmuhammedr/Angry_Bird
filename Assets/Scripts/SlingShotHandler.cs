using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderer")]
    [SerializeField] private LineRenderer leftLine;
    [SerializeField] private LineRenderer rightLine;

    [Header("Transforms")]
    [SerializeField] private Transform leftStartPosition;
    [SerializeField] private Transform rightStartPosition;
    [SerializeField] private Transform centrePos;
    [SerializeField] private Transform idlePos;
    [SerializeField] private Transform elasticTransform;

    [Header("Sling Shot Stats")]
    [SerializeField] private float maxStretchDistance = 3.5f;
    [SerializeField] private float shortForce;
    [SerializeField] private float birdRespawnTimer = 2f;
    [SerializeField] private float elasticDivider = 1.2f;
    [SerializeField] private float maxAnimationTime = 1f;
    [SerializeField] private AnimationCurve elasticCurve;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea slingRange;
    [SerializeField] private CameraManager cameraManager;

    [Header("Sounds")]
    [SerializeField] private AudioClip elasticPulledClip;
    [SerializeField] private AudioClip[] elasticReleasedClips;

    [Header("Angie Bird")]
    [SerializeField] private AngieBird angieBirdPrefab;
    [SerializeField] private float angieBirdOffsetPos = 2f;

    [Header("Power Ups")]
    [SerializeField] private bool isZeroGravityActive = false;
    [SerializeField] private bool isBouncyBirdActive = false;
    [SerializeField] private PhysicsMaterial2D physicsMaterial;
    [SerializeField] private GameObject zeroGravityBirdPanel;
    [SerializeField] private GameObject bouncyBirdPanel;

    private bool isWithinRange;
    private bool birdMounted;

    private Vector2 slingShotLinesPos;

    private Vector2 direction;
    private Vector2 directionNormalized;

    private AudioSource audioSource;
    private AngieBird slingShotBird;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        zeroGravityBirdPanel.SetActive(false);

        leftLine.enabled = false;
        rightLine.enabled = false;
    }
    private void Start()
    {
        isWithinRange = false;

        SpawnBird();
    }
    void Update()
    {
        if(InputManager.wasLeftButtonPressed && slingRange.IsWithinArea())
        {
            isWithinRange = true;
            if (birdMounted)
            {
                SoundManager.instance.PlayClip(elasticPulledClip, audioSource);
                cameraManager.switchToFollowCam(slingShotBird.transform);
            }
        }
        if (InputManager.isLeftButtonPressed && isWithinRange && birdMounted)
        {
            DrawLines();
            PositionAndRotateBird();
        }
        if (InputManager.wasLeftButtonReleased && birdMounted && isWithinRange)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                isWithinRange = false;
                if (isZeroGravityActive)
                {
                    slingShotBird.LaunchZeroGravity(direction, shortForce);
                    //isZeroGravityActive = false;
                }
                else if (isBouncyBirdActive)
                {
                    slingShotBird.LaunchBouncyBird(direction, shortForce);
                    //isBouncyBirdActive = false;
                }
                else
                {
                    slingShotBird.LaunchBird(direction, shortForce);
                }

                SoundManager.instance.PlayRandomClip(elasticReleasedClips, audioSource);

                GameManager.instance.UsedShot();
                birdMounted = false;
                //SetLine(centrePos.position);
                AnimateSlingshot();

                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnAngieBirdDelay());
                }
            }
        }
    }

    #region SLingshot Methods
    private void DrawLines()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.mousePosition);

        slingShotLinesPos = centrePos.position + Vector3.ClampMagnitude(touchPosition - centrePos.position, maxStretchDistance);

        SetLine(slingShotLinesPos);

        direction = (Vector2)centrePos.position - slingShotLinesPos;
        directionNormalized = direction.normalized;
        
    }

    private void SetLine(Vector2 touchposition)
    {
        if (!leftLine.enabled && !rightLine.enabled)
        {
            leftLine.enabled = true;
            rightLine.enabled = true;
        }

        leftLine.enabled = true;
        rightLine.enabled = true;

        leftLine.SetPosition(0, touchposition);
        leftLine.SetPosition(1, leftStartPosition.position);

        rightLine.SetPosition(0, touchposition);
        rightLine.SetPosition(1, rightStartPosition.position);

    }

    #endregion

    #region AngieBird Methods

    public void ActivateZeroGravity()
    {
        isZeroGravityActive = true;
        zeroGravityBirdPanel.SetActive(true);
    }
    public void ActivateBouncyBird()
    {
        isBouncyBirdActive = true;
        bouncyBirdPanel.SetActive(true);
    }

    private void SpawnBird()
    {
        if (isBouncyBirdActive)
        {
            physicsMaterial.bounciness = 0.5f;
            isBouncyBirdActive = false;
            bouncyBirdPanel.SetActive(false);
        }
        else if (isZeroGravityActive)
        {
            isZeroGravityActive = false;
            zeroGravityBirdPanel.SetActive(false);
        }
        elasticTransform.DOComplete();
        SetLine(idlePos.position);

        Vector2 dir = centrePos.position - idlePos.position;
        Vector2 spawnPosition = (Vector2)idlePos.position + dir * angieBirdOffsetPos;

        slingShotBird = Instantiate(angieBirdPrefab, spawnPosition, Quaternion.identity);
        slingShotBird.transform.right = directionNormalized;

        birdMounted = true;
    }

    private void PositionAndRotateBird()
    {
        slingShotBird.transform.position = slingShotLinesPos + directionNormalized * angieBirdOffsetPos;
        slingShotBird.transform.right = directionNormalized;
    }

    private IEnumerator SpawnAngieBirdDelay()
    {
        yield return new WaitForSeconds(birdRespawnTimer);

        SpawnBird();
        cameraManager.switchToIdleCam();
    }

    #endregion

    #region Animate Slingshot

    private void AnimateSlingshot()
    {
        elasticTransform.position = leftLine.GetPosition(0);
        float dist = Vector2.Distance(elasticTransform.position, centrePos.position);
        float time = dist / elasticDivider;

        elasticTransform.DOMove(centrePos.position, time).SetEase(elasticCurve);
        StartCoroutine(AnimateSingshotLines(elasticTransform, time));
    }
    private IEnumerator AnimateSingshotLines(Transform trans, float time)
    {
        float elapsedtime = 0f;
        while(elapsedtime < time && elapsedtime < maxAnimationTime)
        {
            elapsedtime += Time.deltaTime;

            SetLine(trans.position);

            yield return null;
        }
    }

    #endregion
}

