// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in DemoAnimator to deal with the networked player instance
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

#if UNITY_5 && (!UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3) || UNITY_6
#define UNITY_MIN_5_4
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoAnimator
{
    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class PlayerManager : Photon.PunBehaviour, IPunObservable
    {
        #region Public Variables

        [Tooltip("The Player's UI GameObject Prefab")]
        public GameObject PlayerUiPrefab;

        [Tooltip("The Player's UI Panel GameObject Prefab")]
        public GameObject PlayerUiPanelPrefab;

        [Tooltip("The Beams GameObject to control")]
        public GameObject Beams;

        private GameObject _uiGo;

        //[Tooltip("The current Health of our player")]
        public float Health = 1; // { get; set; }

        [Tooltip("The Bullet's prefab")]
        public GameObject bulletPrefab;

        [Tooltip("The Boost prefab")]
        public GameObject boostPrefab;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        

        public Sprite tankBackSprite;
        public Sprite tankSideSprite;
        public Sprite tankFrontSprite;
        public Sprite DPSBackSprite;
        public Sprite DPSSideSprite;
        public Sprite DPSFrontSprite;
        public Sprite healerBackSprite;
        public Sprite healerSideSprite;
        public Sprite healerFrontSprite;

        public Sprite healerLogoSprite;
        public Sprite tankLogoSprite;
        public Sprite DPSLogoSprite;

        public GameObject playerLogoPrefab;
        public GameObject playerSprite;
        public GameObject CBTPrefab;

        public ParticleSystem chargeHP;
        public ParticleSystem chargeDMG;
        public ParticleSystem healEffect;
        //public SkillManager skillMGR;


        private int indexPlayer = 0;
        private Dictionary<string, Sprite> spritesNames = new Dictionary<string, Sprite>();
        private string spriteName;
        #endregion

        #region Private Variables

        //True, when the user is firing
        bool IsFiring;
        private float maxHealth;
        private float playerPower;
        private float currentPlayerPower;

        private float mana;
        private float critChance;
        private float res;
        private float speed;

        private float boostCooldown;
        
        private GameObject gameMGRObj;
        private bool boostHPFlag = false;
        private float boostHPCooldownStart = 0f;
        private float boostHPCooldown;
        private float boostENGCooldownStart = 0f;
        private float boostENGCooldown;
        private float boostSPDCooldownStart = 0f;
        private float boostSPDCooldown;
        private float boostDMGCooldownStart = 0f;
        private float boostDMGCooldown;
        private float critDamage;
        private string playerClass;
        private bool spriteFlag = false;
        private Dictionary<int, Quaternion> spriteOffsetDict = new Dictionary<int, Quaternion>();
        private Dictionary<string, int> spriteToIndexDict = new Dictionary<string, int>();
        private int indexTmp;
        private float indexF;
        private bool rotFlag;
        private GameObject _logo;
        private Image _playerLogoImage;
        private bool logoFlag;
        private GameObject[] playerPosArr;
        private float baseHealth = 1000;
        private float tmpMaxValue;
        private bool boostDMGFlag;
        private float basePower;
        private float playerGravity;
        private float playerElectroMagnetism;
        private float playerWeakForce;
        private float playerStrongForce;
        private int playerLevel;




        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        public void Awake()
        {
            if (this.Beams == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> Beams Reference.", this);
            }
            else
            {
                this.Beams.SetActive(false);
            }
            
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.isMine)
            {
                LocalPlayerInstance = gameObject;
            }

            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            //DontDestroyOnLoad(gameObject);
        }

        public void setPlayerPosArr(GameObject[] _playerPosArr)
        {
            playerPosArr = _playerPosArr;
        }

        public void setPlayerPosBool()
        {
            this.gameObject.GetComponent<PhotonView>().RPC("setPlayerPos", PhotonTargets.AllBuffered);            
        }
        [PunRPC]
        public void setPlayerPos()
        {
            Debug.Log("setting player pos bool. " + playerPosArr[indexPlayer - 1].name);
            playerPosArr[indexPlayer - 1].GetComponent<PlayerPosBoolCheck>().setOccupiedTrue();
        }
        public float getHealth()
        {
            return Health;
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        public void Start()
        {
            gameMGRObj = GameObject.Find("Game Manager");
            if(gameMGRObj.GetComponent<GameManager>() != null)
            {
                Debug.Log("found game manager");
            }
            testCamera _cameraWork = gameObject.GetComponent<testCamera>();

            
            
            
            spriteFlag = true;


            //playerSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("aethers_front_sides");

            //GameManager.setIndexArray(indexPlayer);
            //this.gameObject.GetComponent<PhotonView>().RPC("setIndexArray", PhotonTargets.AllBufferedViaServer);
            //setStats();
            //this.gameObject.GetComponent<PhotonView>().RPC("setStats", PhotonTargets.AllBuffered);
            //setHealth();
            //this.gameObject.GetComponent<PhotonView>().RPC("setHealth", PhotonTargets.All);
            //this.gameObject.GetComponent<PhotonView>().RPC("setRotFlag",PhotonTargets.AllBuffered);
            //this.gameObject.GetComponent<PhotonView>().RPC("setLogoFlag", PhotonTargets.AllBuffered);
            // Create the UI
            if (this.PlayerUiPrefab != null)
            {
                _uiGo = Instantiate(this.PlayerUiPrefab) as GameObject;

                Debug.Log("adjusting ui for player with health: " + Health);
                _uiGo.GetComponent<PlayerUI>().SetTarget(this);

                //_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                
            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
            /*
            if(this.playerClass != null && photonView.isMine && indexPlayer != 0)
            {
                //setLogo();
                this.gameObject.GetComponent<PhotonView>().RPC("setLogo", PhotonTargets.AllBuffered);
                
            }
            */
            
            #if UNITY_MIN_5_4
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            //UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            #endif
            maxHealth = Health;
            //playerPower = getStrength();
            currentPlayerPower = getStrength();
            //_cameraWork.ChangePos(LocalPlayerInstance.transform.position);
        }

        

        public void setIndex(int i)
        {
            indexPlayer = i;
            Debug.Log("players index was set to: " + i);
        }

        public int getIndex()
        {
            return indexPlayer;
        }
        public string getClass()
        {
            return playerClass;
        }

        [PunRPC]
        public void setPlayerStats(int pIndex, string pClass, float pGr, float pEm, float pWf, float pSf, int pLvl)
        {
            indexPlayer = pIndex;
            playerClass = pClass;
            playerGravity = pGr;
            playerElectroMagnetism = pEm;
            playerWeakForce = pWf;
            playerStrongForce = pSf;
            playerLevel = pLvl;
            setSprite();
            setLogo();
            setHealth();
            setStats();
        }

        private void setSprite()
        {
            spritesNames.Add("healerBackSprite", healerBackSprite);
            spritesNames.Add("healerFrontSprite", healerFrontSprite);
            spritesNames.Add("healerSideSprite", healerSideSprite);
            spritesNames.Add("DPSBackSprite", DPSBackSprite);
            spritesNames.Add("DPSFrontSprite", DPSFrontSprite);
            spritesNames.Add("DPSSideSprite", DPSSideSprite);
            spritesNames.Add("tankBackSprite", tankBackSprite);
            spritesNames.Add("tankFrontSprite", tankFrontSprite);
            spritesNames.Add("tankSideSprite", tankSideSprite);
            spriteToIndexDict.Add("healerBackSprite", 1);
            spriteToIndexDict.Add("healerSideSprite", 2);
            spriteToIndexDict.Add("healerFrontSprite", 3);
            spriteToIndexDict.Add("DPSBackSprite", 1);
            spriteToIndexDict.Add("DPSSideSprite", 2);
            spriteToIndexDict.Add("DPSFrontSprite", 3);
            spriteToIndexDict.Add("tankBackSprite", 1);
            spriteToIndexDict.Add("tankSideSprite", 2);
            spriteToIndexDict.Add("tankFrontSprite", 3);
            Quaternion tmp = Quaternion.Euler(30f, 0, 0);
            spriteOffsetDict.Add(1, tmp);
            tmp = Quaternion.Euler(20f, 70f, 0);
            spriteOffsetDict.Add(2, tmp);
            tmp = Quaternion.Euler(10f, 140f, 0f);
            spriteOffsetDict.Add(3, tmp);
            tmp = Quaternion.Euler(10f, -140f, 0f);
            spriteOffsetDict.Add(4, tmp);
            tmp = Quaternion.Euler(20f, -70f, 0f);
            spriteOffsetDict.Add(5, tmp);

            //indexF = (float)indexPlayer;

            //playerClass = PlayerPrefs.GetString("Class");

            if (playerClass == "healer")
            {
                if (indexPlayer == 1)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "healerBackSprite";
                    //spriteToIndexDict.Add("healerBackSprite", 1);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = healerBackSprite;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 2)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "healerSideSprite";
                    //spriteToIndexDict.Add("healerSideSprite", 2);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = healerSideSprite;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 3)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "healerFrontSprite";
                    //spriteToIndexDict.Add("healerFrontSprite", 3);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = healerFrontSprite;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 4)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "healerFrontSprite";
                    spriteToIndexDict["healerFrontSprite"] = 4;
                    //spriteToIndexDict.Add("healerFrontSprite", 4);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = healerFrontSprite;
                    playerSprite.GetComponent<SpriteRenderer>().flipX = true;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 5)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "healerSideSprite";
                    spriteToIndexDict["healerSideSprite"] = 5;
                    //spriteToIndexDict.Add("healerSideSprite", 5);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = healerSideSprite;
                    playerSprite.GetComponent<SpriteRenderer>().flipX = true;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }


            }
            else if (playerClass == "DPS")
            {
                if (indexPlayer == 1)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "DPSBackSprite";
                    //spriteToIndexDict.Add("DPSBackSprite", 1);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = DPSBackSprite;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 2)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "DPSSideSprite";
                    //spriteToIndexDict.Add("DPSSideSprite", 2);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = DPSSideSprite;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 3)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "DPSFrontSprite";
                    //spriteToIndexDict.Add("DPSFrontSprite", 3);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = DPSFrontSprite;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 4)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "DPSFrontSprite";
                    spriteToIndexDict["DPSFrontSprite"] = 4;
                    //spriteToIndexDict.Add("DPSFrontSprite", 4);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = DPSFrontSprite;
                    playerSprite.GetComponent<SpriteRenderer>().flipX = true;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 5)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "DPSSideSprite";
                    spriteToIndexDict["DPSSideSprite"] = 5;
                    //spriteToIndexDict.Add("DPSSideSprite", 5);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = DPSSideSprite;
                    playerSprite.GetComponent<SpriteRenderer>().flipX = true;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
            }
            else if (playerClass == "tank")
            {
                if (indexPlayer == 1)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "tankBackSprite";
                    //spriteToIndexDict.Add("tankBackSprite", 1);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = tankBackSprite;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 2)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "tankSideSprite";
                    //spriteToIndexDict.Add("tankSideSprite", 2);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = tankSideSprite;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 3)
                {
                    indexTmp = indexPlayer;
                    spriteName = "tankFrontSprite";
                    //spriteToIndexDict.Add("tankFrontSprite", 3);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = tankFrontSprite;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 4)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "tankFrontSprite";
                    spriteToIndexDict["tankFrontSprite"] = 4;
                    //spriteToIndexDict.Add("tankFrontSprite", 4);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = tankFrontSprite;
                    playerSprite.GetComponent<SpriteRenderer>().flipX = true;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
                else if (indexPlayer == 5)
                {
                    //indexTmp = indexPlayer;
                    spriteName = "tankSideSprite";
                    spriteToIndexDict["tankSideSprite"] = 5;
                    //spriteToIndexDict.Add("tankSideSprite", 5);
                    playerSprite.GetComponent<SpriteRenderer>().sprite = tankSideSprite;
                    playerSprite.GetComponent<SpriteRenderer>().flipX = true;
                    tmp = spriteOffsetDict[indexPlayer];
                    Debug.Log("index player= " + indexPlayer + " tmp quaternion= " + tmp.eulerAngles);
                    playerSprite.transform.rotation = Quaternion.Euler(tmp.z, tmp.y, tmp.z);
                }
            }

        }

        [PunRPC]
        public void setHealth()
        {
            Health = playerGravity * 1000 + playerLevel * 10;
            
        }

        [PunRPC]
        public void setLogo()
        {
            if (indexPlayer!=0)
            {
                Debug.Log("creating logo on object: Player" + indexPlayer.ToString());
                //_logo = Instantiate(this.PlayerUiPrefab) as GameObject;
                _playerLogoImage = GameObject.Find("Player" + indexPlayer.ToString()).GetComponent<Image>();
                _playerLogoImage.gameObject.GetComponent<PlayerLogo>().SetTarget(this);
                if (playerClass == "healer")
                {
                    _playerLogoImage.sprite = healerLogoSprite;
                }
                else if (playerClass == "tank")
                {
                    _playerLogoImage.sprite = tankLogoSprite;
                }
                else if (playerClass == "DPS")
                {
                    _playerLogoImage.sprite = DPSLogoSprite;
                }
            }
            
        }

        [PunRPC]
        public void setRotFlag()
        {
            rotFlag = true;
        }
        [PunRPC]
        public void setLogoFlag()
        {
            logoFlag = true;
        }
        [PunRPC]
        public void instantiateUI()
        {
            if (!this.photonView.isMine)
            {
                return;
            }
            GameObject _uiGo = Instantiate(this.PlayerUiPrefab) as GameObject;
            _uiGo.GetComponent<PlayerUI>().SetTarget(this);
        }
        //[PunRPC]
        public void setHealth(float boostHealth)
        {
            Health += boostHealth;
            PlayerUiPrefab.GetComponent<Slider>().value = Health;
            PlayerUiPrefab.GetComponent<Slider>().maxValue = Health;
        }

        [PunRPC]
        public void setStats()
        {
            mana = playerElectroMagnetism * 1000 + playerLevel * 10;
            critChance = (float)Math.Sqrt(playerWeakForce);
            res = playerStrongForce / 10;
            speed = playerGravity / 100;
            critDamage = (float)Math.Sqrt(playerElectroMagnetism);
            if (playerClass == "DPS")
            {
                playerPower = playerWeakForce * 2 + playerLevel * 2;
            } else if (playerClass == "healer")
            {
                playerPower = playerElectroMagnetism * 2 + playerLevel * 2;
            } else if (playerClass == "tank")
            {
                playerPower = playerStrongForce * 2 + playerLevel * 2;
            }
            
        }

        [PunRPC]
        public void setHealthUi()
        {
            Debug.Log("punRPC UI update **************************");
            PlayerUiPrefab.GetComponent<Slider>().maxValue = Health;
            PlayerUiPrefab.GetComponent<Slider>().value = Health;
        }

        [PunRPC]
        public void setIndexArray()
        {
            GameManager.Instance.setIndexArrayBool(indexPlayer, 
                                                   this.gameObject.GetComponent<PhotonView>().viewID,
                                                   this.gameObject.transform.position,
                                                   this.gameObject.transform.rotation);
        }
        

        [PunRPC]
        public void updatePosition(Vector3 newPosition, Quaternion newRotation)
        {
            Debug.Log("setting player: " + this.gameObject.GetComponent<PhotonView>().viewID + " new position and rotation");
            this.gameObject.transform.position = newPosition;
            this.gameObject.transform.rotation = newRotation;
        }
        public void OnDisable()
		{
			#if UNITY_MIN_5_4
			//UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
			#endif
		}


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// Process Inputs if local player.
        /// Show and hide the beams
        /// Watch for end of game, when local player health is 0.
        /// </summary>
        public void Update()
        {
            // we only process Inputs and check health if we are the local player
            if (photonView.isMine)
            {
                this.ProcessInputs();

                if (this.Health <= 0f)
                {
                    //this.gameObject.GetComponent<PhotonView>().RPC("updateIndex", PhotonTargets.All, indexPlayer);
                    GameManager.Instance.LeaveRoom();
                }
            }
            if (_uiGo != null)
            {
                if (_uiGo.GetComponent<Slider>().maxValue < Health)
                {
                    tmpMaxValue = Health - _uiGo.GetComponent<Slider>().maxValue;
                    //this.gameObject.GetComponent<PhotonView>().RPC("updateMaxValue", PhotonTargets.AllBuffered, tmpMaxValue);
                    _uiGo.GetComponent<Slider>().maxValue += Health - _uiGo.GetComponent<Slider>().maxValue;
                }
            }
            if (this.Beams != null && this.IsFiring != this.Beams.GetActive())
            {
                this.Beams.SetActive(this.IsFiring);
            }
            
            if (boostDMGFlag && Time.time > boostDMGCooldownStart + boostDMGCooldown)
            {
                chargeHP.Pause();
                playerPower -= basePower;
                //boostDMGFlag = false;
                boostDMGCooldownStart = Time.time;
                // TODO subtract skillBoostPower from currentPlayerPower for multiple effects
            }
            if (boostHPFlag)
            {
                if (!chargeHP.isPlaying)
                {
                    chargeHP.Play();
                }
            } else
            {
                if (chargeHP.isPlaying)
                {
                    chargeHP.Stop();
                }
            }
            if (boostHPFlag && Time.time > boostHPCooldownStart + boostHPCooldown)
            {
                chargeHP.Stop();
                Health -= baseHealth;
                initCBT("-" + ((int)baseHealth).ToString(), false);
                _uiGo.GetComponent<Slider>().maxValue = maxHealth;
                //this.gameObject.GetComponent<PhotonView>().RPC("updateMaxValue", PhotonTargets.AllBuffered, tmpMaxValue);
                //boostHPFlag = false;
                this.gameObject.GetComponent<PhotonView>().RPC("setBoostHPFlag",PhotonTargets.AllBuffered,false);
                boostHPCooldownStart = Time.time;
                // TODO subtract skillBoostPower from currentPlayerPower for multiple effects
            }
            if (Time.time > boostENGCooldownStart + boostENGCooldown)
            {
                boostENGCooldownStart = Time.time;
                // TODO subtract skillBoostPower from currentPlayerPower for multiple effects
            }
            if (Time.time > boostSPDCooldownStart + boostSPDCooldown)
            {


                boostSPDCooldownStart = Time.time;
                // TODO subtract skillBoostPower from currentPlayerPower for multiple effects
            }
            
        }

        
        public float getStrength()
        {
            return playerPower;
        }

        /// <summary>
        /// MonoBehaviour method called when the Collider 'other' enters the trigger.
        /// Affect Health of the Player if the collider is a beam
        /// Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
        /// One could move the collider further away to prevent this or check if the beam belongs to the player.
        /// </summary>
        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.isMine)
            {
                return;
            }

            
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            string name = other.name;
            
            
            if (!other.name.Contains("BoostCapsule"))
            {
                return;
            }
            
        }
        

        public void skillImplementation(PlayerSkill currentSkill, int hitStrength)
        {
            if (currentSkill.getSkillType() == SkillType.DAMAGING)
            {
                this.Health -= hitStrength * currentSkill.getEffectMultiplier();
            }
            if (currentSkill.getSkillType() == SkillType.HEALING)
            {
                this.Health += hitStrength * currentSkill.getEffectMultiplier();
            }
        }

        /// <summary>
        /// MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
        /// We're going to affect health while the beams are interesting the player
        /// </summary>
        /// <param name="other">Other.</param>
        public void OnTriggerStay(Collider other)
        {
            // we dont' do anything if we are not the local player.
            if (!photonView.isMine)
            {
                return;
            }

            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }

            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
            this.Health -= 0.1f*Time.deltaTime;
        }

        public float getCritChance()
        {
            return critChance;
        }



#if !UNITY_MIN_5_4
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            //this.CalledOnLevelWasLoaded(level);
        }
#endif


        /// <summary>
        /// MonoBehaviour method called after a new level of index 'level' was loaded.
        /// We recreate the Player UI because it was destroy when we switched level.
        /// Also reposition the player if outside the current arena.
        /// </summary>
        /// <param name="level">Level index loaded</param>
        /*

        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject _uiGo = Instantiate(this.PlayerUiPrefab) as GameObject;
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        */

        #endregion

        #region Private Methods

        /*
		#if UNITY_MIN_5_4
		void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
		{
			
			this.CalledOnLevelWasLoaded(scene.buildIndex);
		}
		#endif
        */
        /// <summary>
        /// Processes the inputs. This MUST ONLY BE USED when the player has authority over this Networked GameObject (photonView.isMine == true)
        /// </summary>
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // we don't want to fire when we interact with UI buttons for example. IsPointerOverGameObject really means IsPointerOver*UI*GameObject
                // notice we don't use on on GetbuttonUp() few lines down, because one can mouse down, move over a UI element and release, which would lead to not lower the isFiring Flag.
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    //	return;
                }

                if (!this.IsFiring)
                {
                    this.IsFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (this.IsFiring)
                {
                    this.IsFiring = false;
                }
            }
        }

        #endregion

        
        [PunRPC]
        public void setBoostHPFlag(bool flag)
        {
            boostHPFlag = flag;
        }
        [PunRPC]
        public void updateMaxValue(float newValue)
        {
            _uiGo.GetComponent<Slider>().maxValue -= newValue;
        }

        [PunRPC]
        public void triggerShot(string currentSkillName, float effectMultiplier, 
                                SkillType skilltype, float skillEffectDuration)
        {
            Vector3 playerV3 = new Vector3(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.y + 5f, this.gameObject.transform.localPosition.z + 5f); ;
            if (indexPlayer == 1)
            {
                playerV3 = new Vector3(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.y + 5f, this.gameObject.transform.localPosition.z + 5f);
            }
            else if (indexPlayer == 2)
            {
                playerV3 = new Vector3(this.gameObject.transform.localPosition.x - 20f, this.gameObject.transform.localPosition.y + 5f, this.gameObject.transform.localPosition.z + 5f);
            }
            else if (indexPlayer == 3)
            {
                playerV3 = new Vector3(this.gameObject.transform.localPosition.x - 0f, this.gameObject.transform.localPosition.y + 5f, this.gameObject.transform.localPosition.z + 10f);
            }
            else if (indexPlayer == 4)
            {
                playerV3 = new Vector3(this.gameObject.transform.localPosition.x - 0f, this.gameObject.transform.localPosition.y + 5f, this.gameObject.transform.localPosition.z + 10f);
            }
            else if (indexPlayer == 5)
            {
                playerV3 = new Vector3(this.gameObject.transform.localPosition.x + 20f, this.gameObject.transform.localPosition.y + 5f, this.gameObject.transform.localPosition.z + 5f);
            }


            GameObject shot = Instantiate(bulletPrefab, playerV3, this.gameObject.transform.rotation) as GameObject;

            if (shot.GetComponent<ProjectileManager>() != null)
            {
                shot.GetComponent<ProjectileManager>().setProjectileValues(currentSkillName, getStrength(), effectMultiplier, skilltype, skillEffectDuration, getCritChance(), getCritDamage());
            }
        }
        [PunRPC]
        public void triggerBoost(float pPower, float cChance, float cDMG,
                                 string currentSkillName, float effectMultiplier, 
                                 SkillType skilltype, float skillEffectDuration)
        {
            boostPlayer(effectMultiplier, pPower, skilltype, skillEffectDuration, cChance, cDMG);
            
        }
        [PunRPC]
        public void triggerHeal(float playerPower, float critChance, float critDMG, string skillName, float effectMultiplier)
        {
            healPlayer(playerPower, critChance, critDMG, skillName, effectMultiplier);
            
        }

        private void healPlayer(float playerPower, float critChance, float critDMG, string skillName, float effectMultiplier)
        {
            healEffect.Play();
            float temp = UnityEngine.Random.Range(0, 100);
            float tmp;
            if (temp <= critChance)
            {
                tmp = playerPower * effectMultiplier * 10 * critDMG;
                initCBT("+" + ((int)tmp).ToString(),true);
            } else
            {
                tmp = playerPower * effectMultiplier * 10;
                initCBT("+" + ((int)tmp).ToString(), false);
            }
            
            if (_uiGo.GetComponent<Slider>().maxValue <= Health + tmp)
            {
                Health = _uiGo.GetComponent<Slider>().maxValue;
            } else
            {
                Health += tmp;
            }
            Debug.Log("Healing amount of: " + tmp);
        }

        [PunRPC]
        public void getHit(float dmg, float critC, float critDmg)
        {
            Debug.Log("player got hit");
            float temp = UnityEngine.Random.Range(0, 100);
            float tmp = dmg;
            if (temp <= critC)
            {
                tmp *= critDmg;
                initCBT("-" + ((int)tmp).ToString(), true);
                //this.gameObject.GetComponent<PhotonView>().RPC("initCBT", PhotonTargets.All, tmp.ToString(), true);
                Health -= tmp;
            }
            else
            {
                initCBT("-" + ((int)tmp).ToString(), false);
                //this.gameObject.GetComponent<PhotonView>().RPC("initCBT", PhotonTargets.All, tmp.ToString(), false);
                Health -= tmp;
            }
            //Health -= dmg;
        }
        public void initCBT(string text, bool isCrit)
        {
            GameObject tempCBT = Instantiate(CBTPrefab) as GameObject;
            RectTransform tempRect = tempCBT.GetComponent<RectTransform>();
            tempCBT.transform.SetParent(_uiGo.transform);
            tempRect.transform.localPosition = CBTPrefab.transform.localPosition;
            tempRect.transform.localScale = CBTPrefab.transform.localScale;
            tempRect.transform.localRotation = CBTPrefab.transform.localRotation;

            tempCBT.GetComponent<Text>().text = text;
            if (isCrit)
            {
                tempCBT.GetComponent<Animator>().SetTrigger("Crit");
            }
            else
            {
                tempCBT.GetComponent<Animator>().SetTrigger("Hit");
            }
            Destroy(tempCBT.gameObject, 2);
        }

        public float getCritDamage()
        {
            return critDamage;
        }

        #region IPunObservable implementation

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                // We own this player: send the others our data
                //stream.SendNext(IsFiring);
                //stream.SendNext(this.indexTmp);
                stream.SendNext(this.Health);
                /*
                if (_uiGo != null)
                {
                    stream.SendNext(this._uiGo.GetComponent<Slider>().maxValue);
                } else
                {
                    stream.SendNext(this.Health);
                }
                stream.SendNext(this.maxHealth);
                stream.SendNext(this.indexPlayer);
                stream.SendNext(this.playerClass);
                stream.SendNext(this.spriteName);
                stream.SendNext(this.indexF);
                stream.SendNext(this.playerPower);
                stream.SendNext(this.critChance);
                stream.SendNext(this.critDamage);
                stream.SendNext(this.speed);
                stream.SendNext(this.res);
                stream.SendNext(this.mana);
                */
            }
            else
            {
                // Network player, receive data
                //this.IsFiring = (bool)stream.ReceiveNext();
                //this.indexTmp = (int)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
                /*
                if (_uiGo != null)
                {
                    this._uiGo.GetComponent<Slider>().maxValue = (float)stream.ReceiveNext();
                } else
                {
                    maxHealth = (float)stream.ReceiveNext();
                }
                this.maxHealth = (float)stream.ReceiveNext();              
                this.indexPlayer = (int)stream.ReceiveNext();
                this.playerClass = (string)stream.ReceiveNext();
                this.spriteName = (string)stream.ReceiveNext();
                this.indexF = (float)stream.ReceiveNext();
                this.playerPower = (float)stream.ReceiveNext();
                this.critChance = (float)stream.ReceiveNext();
                this.critDamage = (float)stream.ReceiveNext();
                this.speed = (float)stream.ReceiveNext();
                this.res = (float)stream.ReceiveNext();
                this.mana = (float)stream.ReceiveNext();
                */
            }
        }

        #endregion


        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                // We own this player: send the others our data
                //stream.SendNext(this.IsFiring);
                //stream.SendNext(this.indexTmp);
                
                stream.SendNext(this.Health);
                /*
                if (_uiGo != null)
                {
                    stream.SendNext(this._uiGo.GetComponent<Slider>().maxValue);
                } else
                {
                    stream.SendNext(this.Health);
                }
                stream.SendNext(this.maxHealth);
                stream.SendNext(this.indexPlayer);
                stream.SendNext(this.playerClass);
                stream.SendNext(this.spriteName);
                stream.SendNext(this.indexF);
                stream.SendNext(this.playerPower);
                stream.SendNext(this.critChance);
                stream.SendNext(this.critDamage);
                stream.SendNext(this.speed);
                stream.SendNext(this.res);
                stream.SendNext(this.mana);
                //stream.SendNext(this._uiGo);
                */
            }
            else
            {
                // Network player, receive data
                //this.IsFiring = (bool)stream.ReceiveNext();
                //this.indexTmp = (int)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
                /*
                if (_uiGo != null)
                {
                    this._uiGo.GetComponent<Slider>().maxValue = (float)stream.ReceiveNext();
                } else
                {
                    maxHealth = (float)stream.ReceiveNext();
                }
                this.maxHealth = (float)stream.ReceiveNext();
                this.indexPlayer = (int)stream.ReceiveNext();
                this.playerClass = (string)stream.ReceiveNext();
                this.spriteName = (string)stream.ReceiveNext();
                this.indexF = (float)stream.ReceiveNext();
                this.playerPower = (float)stream.ReceiveNext();
                this.critChance = (float)stream.ReceiveNext();
                this.critDamage = (float)stream.ReceiveNext();
                this.speed = (float)stream.ReceiveNext();
                this.res = (float)stream.ReceiveNext();
                this.mana = (float)stream.ReceiveNext();
                //this._uiGo = (GameObject)stream.ReceiveNext();
                */
            }
        }

        [PunRPC]
        public void instantiateEnemy()
        {
            gameMGRObj.GetComponent<GameManager>().instantiateEnemyExecute();
        }
        
        public void boostPlayer(float EffectMultiplier, float playerPower, SkillType skillType, 
                                float skillEffectDuration, float critChance, float critDamage)
        {
            float temp = UnityEngine.Random.Range(0, 100);

            if (skillType == SkillType.BOOSTDAMAGE)
            {
                
                if (!boostDMGFlag)
                {
                    chargeDMG.Play();
                    basePower = EffectMultiplier * playerPower * 10;
                    Debug.Log("boosting DMG " + baseHealth + " for duration of " + skillEffectDuration + " seconds");
                    playerPower += baseHealth;
                    boostDMGCooldown = skillEffectDuration;
                    boostDMGCooldownStart = Time.time;
                    boostDMGFlag = true;
                }
            } else if (skillType == SkillType.BOOSTDEF)
            {

            } else if (skillType == SkillType.BOOSTENG)
            {

            } else if (skillType == SkillType.BOOSTHP)
            {
                
                if (!boostHPFlag)
                {
                    if(temp <= critChance)
                    {
                        baseHealth = EffectMultiplier * playerPower * 10 * critDamage;
                        Debug.Log("boosting hp " + baseHealth + " for duration of " + skillEffectDuration + " seconds");
                        Health += baseHealth;
                        initCBT("+" + ((int)baseHealth).ToString(), true);
                        boostHPCooldown = skillEffectDuration;
                    } else
                    {
                        baseHealth = EffectMultiplier * playerPower * 10;
                        Debug.Log("boosting hp " + baseHealth + " for duration of " + skillEffectDuration + " seconds");
                        Health += baseHealth;
                        initCBT("+" + ((int)baseHealth).ToString(), true);
                        boostHPCooldown = skillEffectDuration;
                    }
                    boostHPCooldownStart = Time.time;
                    boostHPFlag = true;
                    chargeHP.Play();
                    
                }
            }
            else if (skillType == SkillType.BOOSTSPD)
            {

            }
        }

        #endregion

        

        [PunRPC]
        public void exitArena()
        {
            GameManager.Instance.LeaveRoom();
            //gameMGRObj.GetComponent<GameManager>().LeaveRoom();
        }

        [PunRPC]
        public void updatePlayersPos(int playerIndex)
        {
            
            this.gameObject.transform.position = gameMGRObj.GetComponent<GameManager>().getNewPos(playerIndex);
            this.gameObject.transform.rotation = gameMGRObj.GetComponent<GameManager>().getNewRot(playerIndex);
            Debug.Log("players index: " + playerIndex + " has changed position");
            indexPlayer = playerIndex;

        }

    }

}