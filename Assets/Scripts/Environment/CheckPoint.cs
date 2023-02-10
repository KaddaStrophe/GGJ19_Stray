using UnityEngine;
using UnityEngine.VFX;

public class CheckPoint : MonoBehaviour {
    bool _lerpActive;
    GameObject player;
    [SerializeField]
    PlayerController playerController;
    [SerializeField]
    float _lerpTime;
    Transform home;
    GameObject checkpointVFX;
    bool _VFXActivate;

    protected void Awake() {
        _VFXActivate = false;
        checkpointVFX = transform.GetChild(1).gameObject;
        home = GameObject.Find("Home").transform;
    }

    protected void OnEnable() {
        if(!playerController) {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    protected void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            player = collision.gameObject;
            _lerpActive = true;
            _VFXActivate = true;
            playerController.currentCheckpoint = gameObject.transform;
        }
    }
    protected void Update() {
        if (_VFXActivate) {
            checkpointVFX.GetComponent<VisualEffect>().enabled = true;
        }

        if (_lerpActive) {
            LerpHome(_lerpTime);
        }
    }
    void LerpHome(float newLerpTime) {

        _lerpActive = false;
        float lerpTime = newLerpTime;
        float currentLerpTime = 0f;
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime) {
            currentLerpTime = lerpTime;
        }

        float perc = currentLerpTime / lerpTime;
        //home.position = Vector3.Lerp(home.position, player.transform.position, perc);
        home.position = Vector3.Lerp(home.position, transform.position, Time.time * 5);

    }
}
