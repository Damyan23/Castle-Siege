using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using GDX.Collections.Generic;

public class StatusEffectIconCache
{
    public GameObject statusIconContainer;
    public Image statusBuildupFill;
    public Image statusAtiveTimerFill;
    public Image statusIcon;

    public StatusEffectIconCache (GameObject statusIconContainer, Image statusBuildupFill, Image statusAtiveTimerFill, Image statusIcon) 
    {
        this.statusIconContainer = statusIconContainer;
        this.statusBuildupFill = statusBuildupFill;
        this.statusAtiveTimerFill = statusAtiveTimerFill;
        this.statusIcon = statusIcon;
    }
}

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private GameObject statusEffectTemplate;

    [SerializeField] private SerializableDictionary<StatusEffectType, Sprite> statusEffectSpriteDic;
    private Dictionary<StatusEffectSO, StatusEffectIconCache> statusEffectDic;
    private Camera cam;
    private StatusEffectManager effectManager;

    private void Start ()
    {
        cam = Camera.main;
        statusEffectDic = new Dictionary<StatusEffectSO, StatusEffectIconCache> ();
        //effectManager = GetComponent<StatusEffectManager> ();
        effectManager = this.GetComponentInParent<StatusEffectManager> ();
        effectManager.ActivateEffect += OnActiveStatus;
        effectManager.UpdateEffect += OnUpdateStatusEffect;
        effectManager.DeactivateEffect += OnDeactivateStatusEffect;
    }

    void Update ()
    {
        transform.rotation = Quaternion.LookRotation (transform.parent.position - cam.transform.position);
    }

    private StatusEffectIconCache CreateStatusIcon (StatusEffectSO statusEffect)
    {
        if (statusEffectDic.ContainsKey (statusEffect))
        {
            statusEffectDic[statusEffect].statusIconContainer.SetActive (true);
            return statusEffectDic [statusEffect];
        }

        GameObject createdStatusIcon = Instantiate (statusEffectTemplate, transform);
        GameObject statusActiveTimer = createdStatusIcon.transform.Find ("StatusActiveTimer").gameObject;
        
        Image statusBuildRadialFill = createdStatusIcon.GetComponent<Image> ();
        statusBuildRadialFill.fillAmount = 0;

        Image statusActiveTimerFill = statusActiveTimer.GetComponent <Image> ();
        statusActiveTimerFill.fillAmount = 0;

        Image statusIcon = createdStatusIcon.transform.Find ("Icon").GetChild (0).GetComponent<Image> ();   
        statusIcon.sprite = statusEffectSpriteDic[statusEffect.statusEffectType];

        createdStatusIcon.SetActive (true);

        return new StatusEffectIconCache(createdStatusIcon, statusBuildRadialFill, statusActiveTimerFill, statusIcon);
    }

    private void OnActiveStatus (StatusEffectSO statusEffect, float bluidAmount)
    {
        StatusEffectIconCache statusEffectIconCache = CreateStatusIcon (statusEffect);

        statusEffectDic[statusEffect] = statusEffectIconCache;

        OnUpdateStatusEffect (statusEffect, bluidAmount, 0);
    } 

    private void OnUpdateStatusEffect (StatusEffectSO statusEffect, float buildupAmount, float duration)
    {
        statusEffectDic[statusEffect].statusBuildupFill.fillAmount = buildupAmount;
        statusEffectDic[statusEffect].statusAtiveTimerFill.fillAmount = duration;
    }
    
    private void OnDeactivateStatusEffect (StatusEffectSO statusEffect)
    {
        statusEffectDic[statusEffect].statusIconContainer.SetActive (false);
    }

}
