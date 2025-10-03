// SimpleWaypointFollowerUpdate.cs
using System.Linq;
using UnityEngine;

public class SimpleWaypointFollowerUpdate : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Movement")]
    public float baseSpeed = 5f;
    public float speedMultiplier = 1f;
    public bool loop = true;
    public LayerMask characterLayer;
    public float positionIndex;

    private int _index = 0;
    private bool _routeFinished = false;
    private Animator _anim;

    void Awake() => _anim = GetComponentInChildren<Animator>();

    void Update()
    {
        CharacterSidePosUpdate();
        if (waypoints == null || waypoints.Length == 0 || _routeFinished)
        {
            _anim.SetFloat("Blend", 0f);          // idle
            return;
        }

        Vector3 target = waypoints[_index].position;
        Vector3 dir = (target - transform.position).normalized;
        float step = baseSpeed * speedMultiplier * Time.deltaTime;

        // move
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        // look toward target
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.02f);

        // feed real speed to animator
        _anim.SetFloat("Blend", step / Time.deltaTime / 5); // == baseSpeed * speedMultiplier

        // reached waypoint?
        if (transform.position == target)
            AdvanceIndex();
    }

    private void CharacterSidePosUpdate()
    {
        if (Physics.OverlapSphere(transform.position, .5f, characterLayer).Length > 0)
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition,
                Vector3.right * Physics.OverlapSphere(transform.position, .5f, characterLayer).Length * positionIndex, .01f);
        }
        else
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, Vector3.zero, 0.01f);
        }
    }

    private void AdvanceIndex()
    {
        _index++;
        if (_index >= waypoints.Length)
        {
            if (loop)
                _index = 0;
            else
                _routeFinished = true;
        }
    }
}