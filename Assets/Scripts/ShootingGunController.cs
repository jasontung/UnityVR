using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.VR;
public class ShootingGunController : MonoBehaviour
{
    public AudioSource audioSource;
    public VRInput vrInput;
    public Transform gunEnd;
    public ParticleSystem flareParticle;
    public LineRenderer gunFlare;
    public Transform cameraTransform;
    public Reticle reticle;
    public Transform gunContainer;
    public ShootingGalleryController shootingGalleryController;
    public VREyeRaycaster eyeRaycaster;
    public float defaultLineLength = 70f;
    public float gunFlareVisibleSeconds = 0.07f;
    public float damping = 0.5f;
    private const float dampingCoef = -20f;
    public float gunContainerSmooth = 10f;

    private void OnEnable()
    {
        vrInput.OnDown += HandleDown;
    }

    private void OnDisable()
    {
        vrInput.OnDown -= HandleDown;
    }

    private void HandleDown()
    {
        if (shootingGalleryController.IsPlaying == false)
            return;
        ShootingTarget shootingTarget = eyeRaycaster.CurrentInteractible ? eyeRaycaster.CurrentInteractible.GetComponent<ShootingTarget>() : null;
        Transform target = shootingTarget ? shootingTarget.transform : null;
        StartCoroutine(Fire(target));
    }

    private IEnumerator Fire(Transform target)
    {
        audioSource.Play();
        float lineLength = defaultLineLength;
        if (target)
            lineLength = Vector3.Distance(gunEnd.position, target.position);
        flareParticle.Play();
        gunFlare.enabled = true;
        yield return StartCoroutine(MoveLineRenderer(lineLength));
        gunFlare.enabled = false;
    }

    private IEnumerator MoveLineRenderer(float lineLength)
    {
        float timer = 0f;
        while(timer < gunFlareVisibleSeconds)
        {
            gunFlare.SetPosition(0, gunEnd.position);
            gunFlare.SetPosition(1, gunEnd.position + gunEnd.forward * lineLength);
            yield return null;
            timer += Time.deltaTime;
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, InputTracking.GetLocalRotation(VRNode.Head), damping * (1 - Mathf.Exp(dampingCoef * Time.deltaTime)));
        transform.position = cameraTransform.position;
        Quaternion lookAtRotation = Quaternion.LookRotation(reticle.ReticleTransform.position - gunContainer.position);
        gunContainer.rotation = Quaternion.Slerp(gunContainer.rotation, lookAtRotation, gunContainerSmooth * Time.deltaTime);
    }
}
