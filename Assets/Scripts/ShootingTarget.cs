using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using VRStandardAssets.Utils;
using VRStandardAssets.Common;

public class ShootingTarget : MonoBehaviour {
    public event Action<ShootingTarget> OnRemove;
    public int score = 1;
    public float timeOutDuration = 2f;
    public float destroyTimeOutDuration = 2f;
    public GameObject destroyPrefab;
    public AudioClip destroyClip;
    public AudioClip spawnClip;
    public AudioClip missedClip;

    private Transform cameraTransform;                            // Used to make sure the target is facing the camera.
    private VRInteractiveItem interactiveItem;                    // Used to handle the user clicking whilst looking at the target.
    private AudioSource audioSource;                                    // Used to play the various audio clips.
    private Renderer mRenderer;                                    // Used to make the target disappear before it is removed.
    private Collider mCollider;
    private bool isEnding;
    private void Awake()
    {
        // Setup the references.
        cameraTransform = Camera.main.transform;
        audioSource = GetComponent<AudioSource>();
        interactiveItem = GetComponent<VRInteractiveItem>();
        mRenderer = GetComponent<Renderer>();
        mCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        interactiveItem.OnDown += HandleDown;
    }

    private void OnDisable()
    {
        interactiveItem.OnDown -= HandleDown;
    }

    private void OnDestroy()
    {
        OnRemove = null;
    }

    public void Restart(float gameTimeRemaining)
    {
        mRenderer.enabled = true;
        mCollider.enabled = true;
        isEnding = false;
        audioSource.clip = spawnClip;
        audioSource.Play();
        transform.LookAt(cameraTransform);
        StartCoroutine(MissTarget());
        StartCoroutine(GameOver(gameTimeRemaining));
    }
	
    private IEnumerator MissTarget()
    {
        yield return new WaitForSeconds(timeOutDuration);
        if (isEnding)
            yield break;
        isEnding = true;
        mRenderer.enabled = false;
        mCollider.enabled = false;
        audioSource.clip = missedClip;
        audioSource.Play();
        yield return new WaitForSeconds(missedClip.length);
        if (OnRemove != null)
            OnRemove(this);
    }

    private IEnumerator GameOver(float gameTimeRemaining)
    {
        yield return new WaitForSeconds(gameTimeRemaining);
        if (isEnding)
            yield break;
        isEnding = true;
        mRenderer.enabled = false;
        mCollider.enabled = false;
        if (OnRemove != null)
            OnRemove(this);
    }

    private void HandleDown()
    {
        StartCoroutine(OnHit());
    }

    private IEnumerator OnHit()
    {
        if (isEnding)
           yield break;
        isEnding = true;
        mRenderer.enabled = false;
        mCollider.enabled = false;
        audioSource.clip = destroyClip;
        audioSource.Play();
        SessionData.AddScore(score);
        GameObject destroyedTarget = Instantiate(destroyPrefab, transform.position, transform.rotation) as GameObject;
        Destroy(destroyedTarget, destroyTimeOutDuration);
        yield return new WaitForSeconds(destroyClip.length);
        if (OnRemove != null)
            OnRemove(this);
    }
}
