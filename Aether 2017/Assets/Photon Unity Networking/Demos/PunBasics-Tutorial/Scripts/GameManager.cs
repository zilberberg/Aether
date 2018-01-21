// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to handle typical game management requirements
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace ExitGames.Demos.DemoAnimator
{
    /// <summary>
    /// Game manager.
    /// Connects and watch Photon Status, Instantiate Player
    /// Deals with quiting the room and the game
    /// Deals with level loading (outside the in room synchronization)
    /// </summary>
    public class GameManager : Photon.MonoBehaviour {

		#region Public Variables

		static public GameManager Instance;

        [Tooltip("The prefab to use for representing the player")]
		public GameObject playerPrefab;
        public GameObject EnemyPrefab;
        public Text Button1;
        public Text Button2;
        public Text Button3;
        public Text Button4;
        public Text Button5;
        public GameObject skillManager;
        public Canvas canvasWin;
        public Canvas canvasPanel;
        public Canvas DPSPanel;
        public Canvas tankPanel;
        public Canvas healerPanel;
        public Camera mainCam;
        public Canvas canvasUI;
        //public GameObject meteor;
        public GameObject skillProjctilePrefab;
        public GameObject skillBoostCapsulePrefab;

        public GameObject PlayerPos1;
        public GameObject PlayerPos2;
        public GameObject PlayerPos3;
        public GameObject PlayerPos4;
        public GameObject PlayerPos5;
        #endregion

        #region Private Variables

        private GameObject instance;
        private PlayerManager player;
        private bool isSelectingTarget;
        private SkillManager skillMGR;
        private PlayerSkill currentSkill;
        private List<string> buttonsToSkills;
        private GameObject clickedPlayer;
        private GameObject enemy;
        private bool[] indexArray = new bool[5];
        private int[] viewIDArray = new int[5];
        private Quaternion[] playerQArray = new Quaternion[5];
        private Vector3[] v3Array = new Vector3[5];
        public Dictionary<int, Vector3> playersPosDict = new Dictionary<int, Vector3>();
        public Dictionary<int, Quaternion> playersRotDict = new Dictionary<int, Quaternion>();
        private GameObject[] PlayerPosArr = new GameObject[5];
        private bool foundSpawn = false;
        private Hashtable hash;
        [SerializeField] private Text warningText;
        private bool warnText = false;
        private float warnTextStatTime = 0;
        private float warnTextEndTime = 3;
        [SerializeField] private Text targetText;


        //private Hashtable playerPosHash = new Hashtable();
        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
		{
			Instance = this;
            
			// in case we started this demo with the wrong scene being active, simply load the menu scene
			if (!PhotonNetwork.connected)
			{
				SceneManager.LoadScene("Map");

				return;
			}

            

			if (playerPrefab == null) { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.
				
				Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
			} else {


                if (PlayerManager.LocalPlayerInstance == null)
                {                   
                    // Adjusting players locations and rotations. 
                    Debug.Log("We are Instantiating LocalPlayer from " + SceneManagerHelper.ActiveSceneName);
                    int i = PhotonNetwork.playerList.Count<PhotonPlayer>();
                    Debug.Log("Nr of players in room: " + PhotonNetwork.playerList.Count<PhotonPlayer>());
                    Vector3 v3 = new Vector3(0f, -15f, 50f);
                    Quaternion playerQ = Quaternion.Euler(-30f, 0, 0);
                    for (int j = 1; j <= 5; j++)
                    {
                        if (j == 1)
                        {
                            v3.Set(0f, -15f, 50f);
                            playerQ = Quaternion.Euler(-30f, 0, 0);
                            playersPosDict.Add(j, v3);
                            playersRotDict.Add(j, playerQ);
                        }
                        else if (j == 2)
                        {
                            v3.Set(30f, -7f, 70f);
                            playerQ = Quaternion.Euler(-20f, -70f, 0);
                            playersPosDict.Add(j, v3);
                            playersRotDict.Add(j, playerQ);
                        }
                        else if (j == 3)
                        {
                            v3.Set(20f, 0f, 100f);
                            playerQ = Quaternion.Euler(-10f, -140f, 0f);
                            playersPosDict.Add(j, v3);
                            playersRotDict.Add(j, playerQ);
                        }
                        else if (j == 4)
                        {
                            v3.Set(-20f, 0f, 100f);
                            playerQ = Quaternion.Euler(-10f, 140f, 0f);
                            playersPosDict.Add(j, v3);
                            playersRotDict.Add(j, playerQ);
                        }
                        else if (j == 5)
                        {
                            v3.Set(-30f, 7f, 70f);
                            playerQ = Quaternion.Euler(-20f, 70f, 0);
                            playersPosDict.Add(j, v3);
                            playersRotDict.Add(j, playerQ);
                        }
                    }
                    // Instantiating First player, getting stats
                    if (i == 1)
                    {
                        player = PhotonNetwork.Instantiate(
                                        this.playerPrefab.name,
                                        playersPosDict[i],
                                        playersRotDict[i],
                                        0, null).GetComponent<PlayerManager>();
                        player.gameObject.GetComponent<PhotonView>().RPC("setPlayerStats", PhotonTargets.AllBuffered,
                                                                         i,
                                                                         PlayerPrefs.GetString("Class"),
                                                                         PlayerPrefs.GetFloat("gr"),
                                                                         PlayerPrefs.GetFloat("em"),
                                                                         PlayerPrefs.GetFloat("wf"),
                                                                         PlayerPrefs.GetFloat("sf"),
                                                                         PlayerPrefs.GetInt("lvl"));
                        // Inserting players Index via SetCustomProperties Function.
                        hash = new Hashtable();
                        hash.Add("index", i);
                        PhotonNetwork.player.SetCustomProperties(hash);
                    } else
                    {
                        // Adjusting Location if not first player
                        int tmp = 1;                        
                        for (int u = 1; u <= i-1; u++)
                        {
                            foreach (PhotonPlayer pl in PhotonNetwork.playerList)
                            {
                                int tmpInt = pl.ID;
                                Debug.Log("Found player at index: " + (pl.CustomProperties["index"]));
                                if (!pl.IsLocal)
                                {
                                    if ((int)(pl.CustomProperties["index"]) == tmp)
                                    {
                                        Debug.Log("Adjusting index to:" + (tmp + 1));
                                        tmp++;
                                    }
                                }
                            }
                        }
                        // Instantiating not first Player location and rotation.
                        Debug.Log("tmp was set to: " + tmp);
                        if (tmp != 0)
                        {
                            player = PhotonNetwork.Instantiate(
                                        this.playerPrefab.name,
                                        playersPosDict[tmp],
                                        playersRotDict[tmp],
                                        0, null).GetComponent<PlayerManager>();
                            player.gameObject.GetComponent<PhotonView>().RPC("setPlayerStats", PhotonTargets.AllBuffered,
                                                                             tmp,
                                                                             PlayerPrefs.GetString("Class"),
                                                                             PlayerPrefs.GetFloat("gr"),
                                                                             PlayerPrefs.GetFloat("em"),
                                                                             PlayerPrefs.GetFloat("wf"),
                                                                             PlayerPrefs.GetFloat("sf"),
                                                                             PlayerPrefs.GetInt("lvl"));
                            hash = new Hashtable();
                            hash.Add("index", tmp);
                            PhotonNetwork.player.SetCustomProperties(hash);
                        }                        
                    }

                    // Instantiating Enemy for first player
                    if (PhotonNetwork.playerList.Count<PhotonPlayer>() == 1)
                    {                        
                        enemy = PhotonNetwork.InstantiateSceneObject(this.EnemyPrefab.name, new Vector3(0f, 8f, 80f), Quaternion.identity, 0,null);
                    }
                    else
                    {
                        Debug.Log("Finding enemy game object");
                        enemy = GameObject.Find("Enemy");
                    }
                    
                    Debug.Log(PlayerPrefs.GetInt("userID"));
                    Debug.Log(PlayerPrefs.GetString("Class"));
                    // Adjusting skills
                    if ((skillMGR == null) && (skillManager.GetComponent<SkillManager>() != null)){

                        skillMGR = skillManager.GetComponent<SkillManager>();
                        this.buttonsToSkills = this.skillMGR.getOurSkillNames();                                                
                        Button1.GetComponent<Text>().text = buttonsToSkills[0];                        
                        Button2.GetComponent<Text>().text = buttonsToSkills[1];                        
                        Button3.GetComponent<Text>().text = buttonsToSkills[2];                        
                        Button4.GetComponent<Text>().text = buttonsToSkills[3];                        
                        Button5.GetComponent<Text>().text = buttonsToSkills[4];
                        Debug.LogWarning("Achieved Skill Manager");
                    } else
                    {
                        Debug.LogWarning("Missing component");
                    }
                    // Setting visable canvases
                    canvasWin.enabled = false;
                    canvasPanel.enabled = true;
                    DPSPanel.enabled = false;
                    tankPanel.enabled = false;
                    healerPanel.enabled = false;
                    if(PlayerPrefs.GetString("Class") == "DPS")
                    {
                        DPSPanel.enabled = true;
                    } else if(PlayerPrefs.GetString("Class") == "healer")
                    {
                        healerPanel.enabled = true;
                    } else if(PlayerPrefs.GetString("Class") == "tank")
                    {                        
                        tankPanel.enabled = true;
                    }
                    
                }
                else{

					Debug.Log("Ignoring scene load for "+ SceneManagerHelper.ActiveSceneName);
				}
			}
		}

        public void setIndexArrayBool(int indexPlayer, int viewID, Vector3 playersPos, Quaternion playersRot)
        {
            Debug.Log("player index: "+indexPlayer);
            indexArray[indexPlayer - 1] = true;
            viewIDArray[indexPlayer - 1] = viewID;
            v3Array[indexPlayer - 1] = playersPos;
            playerQArray[indexPlayer - 1] = playersRot;

        }
        
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
		{
            
            // clears the warning text
            if (warnText && Time.time > warnTextStatTime + warnTextEndTime)
            {
                warningText.text = "";
                warnText = false;
                warnTextStatTime = Time.time;
            }
            // Getting target by touch
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();
                
                if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.GetComponent<PlayerManager>() != null ||
                                                     hit.collider.gameObject.GetComponent<EnemyManager>() != null))
                {
                    if (hit.collider.gameObject.GetComponent<PlayerManager>() != null)
                    {
                        isSelectingTarget = true;
                        clickedPlayer = hit.collider.gameObject;
                        Debug.Log(clickedPlayer.name + "is targeted");
                        targetText.text = "Target: " + clickedPlayer.name;
                    }
                    else if (hit.collider.gameObject.GetComponent<EnemyManager>() != null)
                    {
                        isSelectingTarget = true;
                        clickedPlayer = hit.collider.gameObject;
                        targetText.text = "Target: " + clickedPlayer.name;
                        Debug.Log(clickedPlayer.name + "is targeted");
                    }
                } else
                {
                    Debug.LogWarning("missing component");
                }
                

            }
			// "back" button of phone equals "Escape". quit app if that's pressed
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitApplication();
			}
		}

        public void activateWinSequence()
        {
            canvasUI.enabled = false;
            canvasWin.enabled = true;
        }

        

        #endregion

        #region Photon Messages

        /// <summary>
        /// Called when a Photon Player got connected. We need to then load a bigger scene.
        /// </summary>
        /// <param name="other">Other.</param>
        /// 

        public void OnPhotonPlayerConnected( PhotonPlayer other  )
		{
			Debug.Log( "OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting

			if ( PhotonNetwork.isMasterClient ) 
			{
				Debug.Log( "OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient ); // called before OnPhotonPlayerDisconnected

				LoadArena();
			}
		}
        
		/// <summary>
		/// Called when a Photon Player got disconnected. We need to load a smaller scene.
		/// </summary>
		/// <param name="other">Other.</param>
        /// 
        /*
		public void OnPhotonPlayerDisconnected( PhotonPlayer other  )
		{
			Debug.Log( "OnPhotonPlayerDisconnected() " + other.NickName ); // seen when other disconnects

			if ( PhotonNetwork.isMasterClient ) 
			{
				Debug.Log( "OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient ); // called before OnPhotonPlayerDisconnected
				
				LoadArena();
			}
		}
        */
		/// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		public virtual void OnLeftRoom()
		{
            PhotonNetwork.LoadLevel(4);
			//SceneManager.LoadScene("Map");
		}

		#endregion

		#region Public Methods
        
		public void LeaveRoom()
		{
            
            if (mainCam.GetComponent<testCamera>() != null)
            {
                // Destroying cam texture, to avoid crashes
                StartCoroutine(mainCam.GetComponent<testCamera>().killCamTexture());                
            }
			PhotonNetwork.LeaveRoom();
            Debug.LogWarning("leaving room");
            SceneManager.LoadScene(4);
            
        }
       


        public void QuitApplication()
		{
			Application.Quit();
		}

		#endregion

		#region Private Methods

		void LoadArena()
		{
			if ( ! PhotonNetwork.isMasterClient ) 
			{
				Debug.LogError( "PhotonNetwork : Trying to Load a level but we are not the master Client" );
			}

			Debug.Log( "PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount ); 

			//PhotonNetwork.LoadLevel("PunBasics-Room for "+PhotonNetwork.room.PlayerCount);    // vintage
		}

		#endregion

        public void SkillSelected(int buttonSkillIndex)
        {
            Debug.LogWarning("Skill nr: " + buttonSkillIndex + " was selected");
            //buttonsToSkills.ElementAt(0);
            string skillName = this.buttonsToSkills[buttonSkillIndex];
            this.currentSkill = skillMGR.getSkill(skillName);

            // Activating AOE (over area) type of skills
            if (currentSkill.getAOE())
            {
                Debug.LogWarning("AOE Skill was selected");
                if (currentSkill.getSkillType() == SkillType.HEALING)
                {
                    if (Time.time > currentSkill.getCooldown() + currentSkill.getCooldownStart())
                    {
                        // Trigger heal skill
                        triggerHeal(currentSkill.getSkillName(),
                                player.getStrength(),
                                currentSkill.getEffectMultiplier(),
                                player.getCritChance(),
                                player.getCritDamage(),
                                currentSkill.getAOE());
                        currentSkill.setCooldownStart(Time.time);
                    }
                } else if (currentSkill.getSkillType() == SkillType.BOOSTDAMAGE ||
                           currentSkill.getSkillType() == SkillType.BOOSTDEF ||
                           currentSkill.getSkillType() == SkillType.BOOSTENG ||
                           currentSkill.getSkillType() == SkillType.BOOSTHP ||
                           currentSkill.getSkillType() == SkillType.BOOSTSPD)
                {
                    // Triggering boost skill
                    if (Time.time > currentSkill.getCooldown() + currentSkill.getCooldownStart())
                    {
                        triggerBoost(currentSkill.getSkillName(),
                                player.getStrength(),
                                currentSkill.getEffectMultiplier(),
                                currentSkill.getSkillType(),
                                currentSkill.getSkillEffectDuration(),
                                player.getCritChance(),
                                player.getCritDamage());
                        currentSkill.setCooldownStart(Time.time);
                    }
                }
                               
            } else if (!currentSkill.getAOE())
            {
                Debug.LogWarning("not AOE Skill was selected");
                if (isSelectingTarget)
                {
                    if (clickedPlayer.GetComponent<EnemyManager>() != null)
                    {
                        if (!clickedPlayer.GetComponent<PhotonView>().isMine)
                        {
                            //clickedPlayer.GetComponent<PhotonView>().
                        }
                        // Activating damage skill
                        if (currentSkill.getSkillType() == SkillType.DAMAGING)
                        {
                            if (Time.time > currentSkill.getCooldown() + currentSkill.getCooldownStart())
                            {
                                triggerShot(currentSkill.getSkillName(),
                                            currentSkill.getEffectMultiplier(),
                                            currentSkill.getSkillType(),
                                            currentSkill.getSkillEffectDuration());
                                currentSkill.setCooldownStart(Time.time);
                            }

                        }
                    }
                }
                else
                {
                    warningText.text = "Error: No one was targeted!";
                    warnTextStatTime = Time.time;
                    warnText = true;
                }
            }
        }

        private void triggerHeal(string skillName, float playerPower,
                                 float effectMultiplier, float critChance,
                                 float critDMG, bool isAOE)
        {
            Debug.Log("triggering heal");
            if (isAOE)
            {
                foreach (PhotonPlayer pl in PhotonNetwork.playerList)
                {                    
                    Debug.Log("try healing player's ID: " + pl.ID);
                    if (PhotonView.Find(pl.ID * 1000 + 1) != null)
                    {
                        if (PhotonView.Find(pl.ID * 1000 + 1).gameObject.GetComponent<PlayerManager>() != null)
                        {
                            PhotonView.Find(pl.ID * 1000 + 1).gameObject.GetComponent<PhotonView>().RPC("triggerHeal",
                                                                                                        PhotonTargets.All,
                                                                                                        playerPower,
                                                                                                        critChance,
                                                                                                        critDMG,
                                                                                                        skillName,
                                                                                                        effectMultiplier);
                        }
                    }
                }
            } else
            {

            }
        }

        // TODO: send player RPC triggerBoost
        public void triggerBoost(string skillname, 
                                 float playerPower, 
                                 float effectMultiplier, 
                                 SkillType skillType, 
                                 float skillEffectDuration,
                                 float critChance,
                                 float critDamage)
        {
            foreach (PhotonPlayer pl in PhotonNetwork.playerList)
            {
                Debug.Log("try boosting player's ID: " + pl.ID);
                if (PhotonView.Find(pl.ID * 1000 + 1) != null)
                {
                    if (PhotonView.Find(pl.ID * 1000 + 1).gameObject.GetComponent<PlayerManager>() != null)
                    {
                        PhotonView.Find(pl.ID * 1000 + 1).gameObject.GetComponent<PhotonView>().RPC("triggerBoost",
                                                                                                    PhotonTargets.AllBuffered,
                                                                                                    playerPower,
                                                                                                    critChance,
                                                                                                    critDamage,
                                                                                                    skillname,
                                                                                                    effectMultiplier,
                                                                                                    skillType,
                                                                                                    skillEffectDuration);
                    }
                }
            }
            /*
            player.gameObject.GetComponent<PhotonView>().RPC("triggerBoost", PhotonTargets.All,
                                                                            skillname,
                                                                            effectMultiplier,
                                                                            skillType,
                                                                            skillEffectDuration);
                                                                            */
            /*
            GameObject boostProjectile;
            boostProjectile = PhotonNetwork.Instantiate("BoostCapsule", position, Quaternion.identity, 0, null);
            if (boostProjectile.GetComponent<ProjectileManager>() != null)
            {
                boostProjectile.GetComponent<ProjectileManager>().setProjectileValues(skillname, playerPower, effectMultiplier, skillType, skillEffectDuration, critEffect, critDamage);                
            }
            */
        }
        // GameManager triggershot calls player triggershot through RPC to avoid instantiating photon network object
        public void triggerShot(string currentSkillName, 
                                float effectMultiplier, 
                                SkillType skilltype, 
                                float skillEffectDuration)
        {
            player.gameObject.GetComponent<PhotonView>().RPC("triggerShot", PhotonTargets.All,
                                                                            currentSkillName,
                                                                            effectMultiplier,
                                                                            skilltype,
                                                                            skillEffectDuration);
                                                                            
        }

        public void instantiateEnemyExecute()
        {
            enemy = Instantiate(EnemyPrefab, new Vector3(0f, 8f, 80f), Quaternion.identity) as GameObject;
        }

        public Vector3 getNewPos(int playerIndex)
        {
            Debug.Log("Extracting new players position");
            return playersPosDict[playerIndex];
        }

        public Quaternion getNewRot(int playerIndex)
        {
            Debug.Log("Extracting new players rotation");
            return playersRotDict[playerIndex];
        }
    }

}