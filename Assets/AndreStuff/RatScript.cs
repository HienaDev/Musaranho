using System.Collections;
using AndreStuff;
using UnityEngine;

public class RatScript : MonoBehaviour
{

    [SerializeField] private float waitTimeForRun = 0.5f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float timeBeforeStartDisappearing = 0.5f;
    [SerializeField] private float timeToDisappear = 1f;
    
    private Renderer _renderer;
    private Camera _cam;
    private bool _hasBeenSeen = false;
    private Animator _animator;
    private Rigidbody _rb;
    private bool _shouldRun = false;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _cam = Camera.main;
        _renderer = transform.Find("Rat").GetComponent<SkinnedMeshRenderer>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_shouldRun) _rb.linearVelocity = transform.right * moveSpeed;
        if (_hasBeenSeen) return;
        if (IsSeen()) StartCoroutine(RatGotFoundAnime());
    }
    
    private bool IsSeen()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_cam);
        return GeometryUtility.TestPlanesAABB(planes , transform.GetComponent<Collider>().bounds);
    }

    private IEnumerator RatGotFoundAnime()
    {
        //Debug.Log("RAT SEEN!!");
        SoundManager.Instance.PlaySound(SoundType.RAT_JUMPSCARE, null, 0.85f, 1.2f);
        _hasBeenSeen = true;
        yield return new WaitForSeconds(waitTimeForRun);
        SoundManager.Instance.PlaySound(SoundType.RAT_TIKUS, transform.gameObject, true);
        StartCoroutine(LowerSound((timeToDisappear + timeBeforeStartDisappearing)/1.5f));
        _animator.SetBool("seen", true);
        _shouldRun = true;
        yield return new WaitForSeconds(timeBeforeStartDisappearing);
        Color color = _renderer.material.color;
        while (true)
        {
            color.a -= (1f / timeToDisappear) * Time.deltaTime;
            _renderer.material.color = color;
            if (color.a < 0f) break;
            yield return null;
        }
        Destroy(gameObject);
    }

    private IEnumerator LowerSound(float time)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        while (true)
        {
            audioSource.volume -= (1f / time) * Time.deltaTime;
            yield return null;
        }
    }
    
    
}
