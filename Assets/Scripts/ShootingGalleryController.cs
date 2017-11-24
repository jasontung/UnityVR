using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Common;
using VRStandardAssets.Utils;
using UnityEngine.UI;
public class ShootingGalleryController : MonoBehaviour
{
    public UIController uiController;
    public Reticle reticle;
    public SelectionRadial selectionRadial;
    public SelectionSlider selectionSlider;
    public Image timerBar;
    public Collider spawnCollider;
    public ObjectPool targetObjectPool;
    public float gameDuration = 30f;
    public float endDelay = 1.5f;
    public float spawnProbabilty = 0.7f;
    public float spawnInterval = 1f;
    public bool IsPlaying
    {
        private set;
        get;
    }

    private IEnumerator Start()
    {
        SessionData.SetGameType(SessionData.GameType.SHOOTER180);
        while(true)
        {
            yield return StartCoroutine(StartPhase());
            yield return StartCoroutine(PlayPhase());
            yield return StartCoroutine(EndPhase());
        }
    }

    private IEnumerator StartPhase()
    {
        yield return StartCoroutine(uiController.ShowIntroUI());
        reticle.Show();
        selectionRadial.Hide();
        yield return StartCoroutine(selectionSlider.WaitForBarToFill());
        yield return StartCoroutine(uiController.HideIntroUI());
    }

    private IEnumerator PlayPhase()
    {
        yield return StartCoroutine(uiController.ShowPlayerUI());
        IsPlaying = true;
        reticle.Show();
        SessionData.Restart();
        yield return StartCoroutine(PlayUpdate());
        IsPlaying = false;
    }

    private IEnumerator EndPhase()
    {
        reticle.Hide();
        yield return StartCoroutine(uiController.ShowOutroUI());
        yield return new WaitForSeconds(endDelay);
        yield return StartCoroutine(selectionRadial.WaitForSelectionRadialToFill());
        yield return StartCoroutine(uiController.HideOutroUI());
    }

    private IEnumerator PlayUpdate()
    {
        float gameTimer = gameDuration;
        float spawnTimer = 0f;
        while (gameTimer > 0f)
        {
            if(spawnTimer <= 0f)
            {
                if(Random.value < spawnProbabilty)
                {
                    spawnTimer = spawnInterval;
                    Spawn(gameTimer);
                }
            }
            yield return null;
            gameTimer -= Time.deltaTime;
            spawnTimer -= Time.deltaTime;
            timerBar.fillAmount = gameTimer / gameDuration;
        }
    }

    private void Spawn(float timeRemaining)
    {
        GameObject target = targetObjectPool.GetGameObjectFromPool();
        target.transform.position = SpawnPosition();
        //---------------------------------------------------
        ShootingTarget shootingTarget = target.GetComponent<ShootingTarget>();
        shootingTarget.Restart(timeRemaining);
        shootingTarget.OnRemove += HandleTargetRemoved;
        //---------------------------------------------------
    }
    //---------------------------------------------------
    private void HandleTargetRemoved(ShootingTarget target)
    {
        target.OnRemove -= HandleTargetRemoved;
        targetObjectPool.ReturnGameObjectToPool(target.gameObject);
    }
    //---------------------------------------------------
    private Vector3 SpawnPosition()
    {
        Vector3 center = spawnCollider.bounds.center;
        Vector3 extents = spawnCollider.bounds.extents;
        float x = Random.Range(center.x - extents.x, center.x + extents.x);
        float y = Random.Range(center.y - extents.y, center.y + extents.y);
        float z = Random.Range(center.z - extents.z, center.z + extents.z);
        return new Vector3(x, y, z);
    }
}
