using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class DeathVFX : MonoBehaviour
{
    private SpriteRenderer playerSpriteRenderer;

    private VisualEffect deathEffect;
    private VisualEffect levelTransitionEffect;
    private VisualEffect mothmanEffect1;
    private VisualEffect mothmanEffect2;

    private VisualEffect homeRadiusEffect;

    public static DeathVFX Instance;

    private Transform homeTransform;

    private void Awake()
    {
        Instance = this;
        deathEffect = GetComponent<VisualEffect>();
        mothmanEffect1 = GameObject.Find("Mothman_VFX").GetComponent<VisualEffect>();
        mothmanEffect2 = GameObject.Find("Mothman_VFX_BehindPlayer").GetComponent<VisualEffect>();
        homeRadiusEffect = GameObject.Find("HomeRadiusVFX").GetComponent<VisualEffect>();
        //levelTransitionEffect = GameObject.Find("LevelTransitionVFX").GetComponent<VisualEffect>();
        homeTransform = GameObject.Find("Home").transform;
        playerSpriteRenderer = GameObject.Find("Player").GetComponent<SpriteRenderer>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(DeathRoutine());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            playerSpriteRenderer.color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            //levelTransitionEffect.SendEvent("OnLevelTransition");
            //  levelTransitionEffect.SetVector3("TargetPosition", Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0f)));
        }
    }

    public void ActivateMothmanEffects()
    {
        mothmanEffect1.Play();
        mothmanEffect2.Play();
    }

    public void Death()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        deathEffect.SetVector3("TargetPos", homeTransform.position);
        deathEffect.SetBool("GoToRespawnLocation", false);
        mothmanEffect1.Stop();
        mothmanEffect2.Stop();
        deathEffect.SendEvent("OnDeath");
        StartCoroutine(Utility.LerpColorRoutine(playerSpriteRenderer, new Color(1, 1, 1, 0), 2f, false));
        yield return new WaitForSeconds(2f);
        homeRadiusEffect.SetBool("PlayerRespawning", true);
        deathEffect.SetBool("GoToRespawnLocation", true);
        yield return new WaitForSeconds(1f);
        deathEffect.SetBool("GoToRespawnLocation", false);
        yield return new WaitForSeconds(1f);
        deathEffect.SetBool("GoToRespawnLocation", true);
        yield return new WaitForSeconds(3f);
        homeRadiusEffect.SetBool("PlayerRespawning", false);

    }
}
