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
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace ExitGames.Demos.DemoAnimator
{
    public class EnemyManager : Photon.PunBehaviour, IPunObservable
    {
        #region Public Variables

        [Tooltip("The Player's UI GameObject Prefab")]
        public GameObject EnemyUiPrefab;

        [Tooltip("The Beams GameObject to control")]
        public GameObject Beams;

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        public GameObject HitEffect;
        public GameObject hurricane;
        public GameObject CBTPrefab;

        public Dictionary<int, int> scenePlayers = new Dictionary<int, int>();

        private RaycastHit rayHit;
        private GameObject gameMGRObj;

        private GameObject _uiGo;

        private float enemyHitCooldown = 10f;
        bool IsFiring;
        private float enemyHitCooldownStart = 0f;
        private float critChance = 5;
        private float critDamage = 2;
        private int playerCount;
        private int tmpPlayerCount;
        private float power = 3000;
        private float basePower = 3000;
        private float baseHealth = 100000;
        private float tmpMaxValue;
        private bool synced = false;
        private float tmpHealth;
        private int tmp;
        #endregion
        // Use this for initialization
        void Start()
        {
            playerCount = PhotonNetwork.playerList.Count<PhotonPlayer>();
            tmpPlayerCount = 1;
            gameMGRObj = GameObject.Find("Game Manager");
            if (gameMGRObj.GetComponent<GameManager>() != null)
            {
                Debug.Log("found game manager");
            }
            if (PhotonNetwork.playerList.Count<PhotonPlayer>() == 1)
            {
                gameObject.GetComponent<Animator>().SetTrigger("BlackHole");
            } else
            {
                gameObject.GetComponent<Animator>().SetTrigger("Start");
            }


            // Create the UI
            if (this.EnemyUiPrefab != null)
            {
                //EnemyUiPrefab.GetComponent<Slider>().value = Health;
                //EnemyUiPrefab.GetComponent<Slider>().maxValue = Health;
                _uiGo = Instantiate(this.EnemyUiPrefab) as GameObject;
                //GameObject _uiGo = Instantiate(this.EnemyUiPrefab) as GameObject;
                _uiGo.GetComponent<EnemyUI>().SetTarget(this);
                //_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
            //synced = true;

        }

        // Update is called once per frame
        void Update()
        {
            if (Health <= 0)
            {
                this.gameObject.GetComponent<PhotonView>().RPC("exitArena", PhotonTargets.All);
            }
            if (_uiGo != null)
            {
                if (_uiGo.GetComponent<Slider>().maxValue < Health)
                {
                    tmpMaxValue = Health - _uiGo.GetComponent<Slider>().maxValue;
                    _uiGo.GetComponent<Slider>().maxValue += Health - _uiGo.GetComponent<Slider>().maxValue;
                }
                if (_uiGo.GetComponent<Slider>().maxValue < 100000)
                {
                    //tmpMaxValue = Health - _uiGo.GetComponent<Slider>().maxValue;
                    _uiGo.GetComponent<Slider>().maxValue = 100000;
                }
            }

            //anim_Animator.GetCurrentAnimatorStateInfo(0).IsName("MyAnimationName")
            //gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("BlackHole")
            /*
            if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BlackHole"))
            {
                gameObject.GetComponent<PhotonView>().RPC("setTriggerAppear", PhotonTargets.AllBufferedViaServer);
            }
            */
            
            if (PhotonNetwork.playerList.Count<PhotonPlayer>() > playerCount)
            {
                tmp = (PhotonNetwork.playerList.Count<PhotonPlayer>() - playerCount);
                Health += (baseHealth * tmp);
                power += (basePower * tmp);
                playerCount = PhotonNetwork.playerList.Count<PhotonPlayer>();
            } else if (PhotonNetwork.playerList.Count<PhotonPlayer>() < playerCount)
            {
                tmp = (PhotonNetwork.playerList.Count<PhotonPlayer>() - playerCount);
                Debug.Log("Player left. reducing enemy health by: "+ (baseHealth * tmp) + " and power by: " + (basePower * tmp));
                tmpHealth = (baseHealth * tmp);
                Health += tmpHealth;                
                power += (basePower * tmp);
                playerCount = PhotonNetwork.playerList.Count<PhotonPlayer>();
                if (_uiGo != null)
                {
                    Debug.Log("reducing enemy slider max health by: " + tmpHealth);
                    _uiGo.GetComponent<Slider>().maxValue += tmpHealth;
                }
            }


            //gameObject.GetComponent<Animator>().SetTrigger("Appear");
            if (Time.time > enemyHitCooldown + enemyHitCooldownStart)
            {
                //PhotonPlayer[] roomPlayers = PhotonNetwork.playerList;
                if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Enemy_Static"))
                {
                    foreach (PhotonPlayer pl in PhotonNetwork.playerList)
                    {
                        Debug.Log("enemy targeting player's ID: " + pl.ID);
                        if (PhotonView.Find(pl.ID * 1000 + 1) != null)
                        {
                            if (PhotonView.Find(pl.ID * 1000 + 1).gameObject.GetComponent<PlayerManager>() != null)
                            {
                                PhotonView.Find(pl.ID * 1000 + 1).gameObject.GetComponent<PlayerManager>().getHit(power, critChance, critDamage);
                            }
                        }
                        
                    }
                    initHurricane();
                }
                enemyHitCooldownStart = Time.time;
            }
            
            
        }
        [PunRPC]
        public void setTriggerAppear()
        {
            gameObject.GetComponent<Animator>().SetBool("Appear",true);
        }

        public void skillImplementation(PlayerSkill currentSkill, int hitStrength)
        {
            if (currentSkill.getSkillType() == SkillType.DAMAGING)
            {
                this.Health -= hitStrength * currentSkill.getEffectMultiplier();
            }
        }
        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.isMine)
            {
                Debug.Log("Enemy: photon view not mine");
                //return;
            }
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            string name = other.name;
            if (!other.name.Contains("Bullet"))
            {
                return;
            }

            
            Debug.Log("Enemy collider was triggered");
            if (other.gameObject.GetComponent<ProjectileManager>() != null)
            {
                if (HitEffect.GetComponent<ParticleSystem>() != null)
                {
                    Debug.Log("simulating hit effect");
                    activateHitEffect(other.transform.position);
                    /*
                    this.gameObject.GetComponent<PhotonView>().RPC("activateHitEffect",
                                                                    PhotonTargets.All,
                                                                    other.transform.position);
                                                                    */
                    //HitEffect.transform.position = other.transform.position;
                    //HitEffect.GetComponent<ParticleSystem>().Play
                    //HitEffect.GetComponent<ParticleSystem>().Simulate(1, true,true);
                    //HitEffect.GetComponent<ParticleSystem>().
                }
                
                
                //Health -= other.GetComponent<ProjectileManager>().getEffectMultiplier() *
                //           other.GetComponent<ProjectileManager>().getPlayerPower();
            }
            
            
        }
        
        #region IPunObservable implementation

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
            
            if (stream.isWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
                stream.SendNext(power);
                //stream.SendNext(true);
                //stream.SendNext(playerCount);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
                this.power = (float)stream.ReceiveNext();
                //this.synced = (bool)stream.ReceiveNext();
                //this.playerCount = (int)stream.ReceiveNext();
            }
            
        }

        #endregion
        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
            
            if (stream.isWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(this.IsFiring);
                stream.SendNext(this.Health);
                stream.SendNext(power);
                //stream.SendNext(true);
                //stream.SendNext(playerCount);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
                this.power = (float)stream.ReceiveNext();
                //this.synced = (bool)stream.ReceiveNext();
                //this.playerCount = (int)stream.ReceiveNext();
            }
            
        }

        #endregion

        [PunRPC]
        public void execute(float EffectMultiplier, float playerPower, float critChance, float critDamage)
        {

            Debug.Log("executing enemy CBT");
            float temp = Random.Range(0, 100);
            float tmp = EffectMultiplier * playerPower;
            if (temp <= critChance)
            {
                tmp *= critDamage;
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

        }

        
        public void initHurricane()
        {
            this.gameObject.GetComponent<Animator>().SetTrigger("Attack");
            Debug.Log("instantiating hurricane");
            Vector3 tmp = this.transform.position;
            tmp.z -= 30;
            tmp.y = 0;
            GameObject tmpHurricane = Instantiate(hurricane, tmp, Quaternion.identity);
            
        }

        
        public void activateHitEffect(Vector3 hitPosition)
        {
            Debug.Log("instantiating hit effect");
            Instantiate(HitEffect, hitPosition, Quaternion.identity);
        }
        [PunRPC]
        public void exitArena()
        {
            if (gameMGRObj.GetComponent<GameManager>() != null)
            {
                PhotonNetwork.room.IsOpen = false;
                gameObject.GetComponent<Animator>().SetTrigger("Die");
                gameMGRObj.GetComponent<GameManager>().activateWinSequence();
            }
            //gameMGRObj.GetComponent<GameManager>().LeaveRoom();
        }
        //[PunRPC]
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
            } else
            {
                tempCBT.GetComponent<Animator>().SetTrigger("Hit");
            }
            Destroy(tempCBT.gameObject, 2);
        }

        [PunRPC]
        public void addAetherToDict(int playerIndex, int playerID)
        {            
            Debug.Log("player: " + playerID + ". and index: " + playerIndex + " were saved to enemy (sceneObj)");
            scenePlayers.Add(playerIndex, playerID);
        }
        [PunRPC]
        public void updateAetherDict( int playerIndexLeft)
        {
            Debug.Log("updating players dictionary");
            scenePlayers.Remove(playerIndexLeft);
            for (int j = playerIndexLeft + 1; j < 5; j++)
            {
                if (scenePlayers.ContainsKey(j))
                {
                    PhotonView.Find(scenePlayers[j]).RPC("updatePlayersPos", PhotonTargets.AllBufferedViaServer, j-1);
                    scenePlayers.Add(j-1,scenePlayers[j]);
                    scenePlayers.Remove(j);
                    
                } else
                {
                                         
                    
                }
            }
        }
    }

}

