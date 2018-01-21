using ExitGames.Demos.DemoAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLogo : MonoBehaviour
{

    public Sprite healerLogoSprite;
    public Sprite tankLogoSprite;
    public Sprite DPSLogoSprite;
    private PlayerManager _target;
    private Transform _targetTransform;
    private Renderer _targetRenderer;
    private Vector3 _targetPosition;
    private int playerIndex;
    private string playerClass;

    public Sprite uiMask;

    void Awake()
    {
        
    }
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null)
        {
            this.gameObject.GetComponent<Image>().sprite = uiMask;
            //Destroy(this.gameObject.GetComponent<Image>().sprite);
            //Destroy(this.gameObject);
            return;
        }
    }
    void LateUpdate()
    {

        // #Critical
        // Follow the Target GameObject on screen.
        

    }

    public void SetTarget(PlayerManager target)
    {
        if (target == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        Debug.Log("setting my logo ui");

        // Cache references for efficiency because we are going to reuse them.
        _target = target;
        playerIndex = _target.getIndex();
        playerClass = _target.getClass();
        //_targetTransform = _target.GetComponent<Transform>();
        //_targetRenderer = _target.GetComponent<Renderer>();


    }
}
