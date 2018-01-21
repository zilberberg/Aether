using UnityEngine;
using System.Collections;
using ExitGames.Demos.DemoAnimator;

public class onCollision : MonoBehaviour {
    [SerializeField]
    public float burst;
	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * burst);


    }
	
	// Update is called once per frame
	void Update () {
	
	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyManager>() != null)
        {
            /*
            other.gameObject.GetComponent<PhotonView>().RPC("execute",
                                                        PhotonTargets.All,
                                                        this.gameObject.GetComponent<ProjectileManager>().getEffectMultiplier(),
                                                        this.gameObject.GetComponent<ProjectileManager>().getPlayerPower(),
                                                        this.gameObject.GetComponent<ProjectileManager>().getCritChance(),
                                                        this.gameObject.GetComponent<ProjectileManager>().getCritDamage());
                                                        */
            other.gameObject.GetComponent<EnemyManager>().execute(this.gameObject.GetComponent<ProjectileManager>().getEffectMultiplier(),
                                                        this.gameObject.GetComponent<ProjectileManager>().getPlayerPower(),
                                                        this.gameObject.GetComponent<ProjectileManager>().getCritChance(),
                                                        this.gameObject.GetComponent<ProjectileManager>().getCritDamage());
            //other.gameObject.GetComponent<PhotonView>().RPC("initCBT", PhotonTargets.All);
        } else if (other.gameObject.GetComponent<PlayerManager>() != null)
        {
            other.gameObject.GetComponent<PhotonView>().RPC("boostPlayer",
                                                            PhotonTargets.AllBufferedViaServer,
                                                            this.gameObject.GetComponent<ProjectileManager>().getEffectMultiplier(),
                                                            this.gameObject.GetComponent<ProjectileManager>().getPlayerPower(),
                                                            this.gameObject.GetComponent<ProjectileManager>().getSkillType(),
                                                            this.gameObject.GetComponent<ProjectileManager>().getSkillEffectDuration(),
                                                            this.gameObject.GetComponent<ProjectileManager>().getCritChance(),
                                                            this.gameObject.GetComponent<ProjectileManager>().getCritDamage());
        }
        
        Destroy(this.gameObject);
    }
}
