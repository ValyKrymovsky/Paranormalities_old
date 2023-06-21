using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using MyBox;

enum currentFoot
{
    left,
    right
}

public class P_SoundSystem : MonoBehaviour
{
    private P_Movement p_movement;
    private CharacterController ch_controller;
    private Coroutine walkCoroutine;
    private Coroutine sprintCoroutine;
    private Coroutine sneakCoroutine;

    [Separator("Audio", true)]
    [SerializeField] 
    private AudioSource source;
    [SerializeField]
    private float minPitch = .8f;
    [SerializeField]
    private float maxPitch = 1.2f;
    [SerializeField, Range(0, 10)]
    private float volume = 1f;
    [SerializeField]
    private SurfaceIdentity ground;
    private List<AudioClip> walkClips;
    private List<AudioClip> sprintClips;
    private List<AudioClip> currentFootstepsList = new List<AudioClip>();
    private List<AudioClip> playedFootsteps;
    private movementState currentMoveAction;
    [SerializeField]
    private GameObject leftFootJoint;
    [SerializeField]
    private GameObject rightFootJoint;
    
    [Separator("Noise", true)]
    [SerializeField]
    private float noise;
    [SerializeField]
    private float defaultNoise = 1;
    [SerializeField]
    private float sprintMultiplier = 1.75f;
    [SerializeField]
    private float sneakMultiplier = .45f;
    [SerializeField]
    private currentFoot currentFoot;

    [Separator("Noise Multipliers", true)]
    [SerializeField]
    private float stoneMultiplier;
    [SerializeField]
    private float grassMultiplier;
    [SerializeField]
    private float woodMultiplier;

    private Dictionary<SurfaceIdentity, float> groundNoiseMultiplier = new Dictionary<SurfaceIdentity, float>();

    private void Awake() {
        p_movement = GetComponent<P_Movement>();
        ch_controller = GetComponent<CharacterController>();
        playedFootsteps = new List<AudioClip>();
        groundNoiseMultiplier.Add(SurfaceIdentity.stone, 1f);
        groundNoiseMultiplier.Add(SurfaceIdentity.grass, .75f);
        groundNoiseMultiplier.Add(SurfaceIdentity.wood, 1.3f);
    }

    /// <summary>
    /// </summary>
    /// <returns>noise value</returns>
    public float GetNoise()
    {
        return noise;
    }

    /// <summary>
    /// Sets noise value.
    /// </summary>
    /// <param name="_value"></param>
    public void SetNoise(float _value)
    {
        noise = _value;
    }

    /// <summary>
    /// Plays sound based on current selected clip.
    /// </summary>
    public void PlaySound()
    {
        if (!source.isPlaying || source.time >(source.clip.length * .5f))
        {
            AudioClip clip = SelectClip();
            if (clip != null)
            {
                float randomPitch = Random.Range(minPitch, maxPitch);
                source.pitch = randomPitch;
                source.volume = volume / 10;
                source.clip = clip;
                source.Play();
                if (ground == SurfaceIdentity.stone)
                {
                    if (GetMovementAction() == movementState.sprint)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(0).Value * sprintMultiplier;
                    }
                    else if (GetMovementAction() == movementState.sneak)
                    {
                        noise = 0;
                    }
                    else if (GetMovementAction() == movementState.walk)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(0).Value;
                    }
                }
                else if (ground == SurfaceIdentity.grass)
                {
                    if (GetMovementAction() == movementState.sprint)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(1).Value * sprintMultiplier;
                    }
                    else if (GetMovementAction() == movementState.sneak)
                    {
                        noise = 0;
                    }
                    else if (GetMovementAction() == movementState.walk)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(1).Value;
                    }
                }
                else if (ground == SurfaceIdentity.wood)
                {
                    if (GetMovementAction() == movementState.sprint)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(2).Value * sprintMultiplier;
                    }
                    else if (GetMovementAction() == movementState.sneak)
                    {
                        noise = 0;
                    }
                    else if (GetMovementAction() == movementState.walk)
                    {
                        noise = defaultNoise * groundNoiseMultiplier.ElementAt(2).Value;
                    }
                }

                // if (GetMovementAction() == movementState.walk || GetMovementAction() == movementState.sprint)
                // {
                //     SpawnNoiseObject(NoiseObjectGlobalClass.noiseObject, gameObject, GameObject.Find("Noise Objects"), gameObject.transform.position, noise);
                // }  
            }
        }
    }

    /// <summary>
    /// Selects clip from current surfaceMaterial footstep list based on current player move action
    /// </summary>
    /// <returns>random AudioCLip</returns>
    private AudioClip SelectClip()
    {
        AudioClip selectedClip = null;

        if (GetAudioList() != null)
        {
            if (!currentFootstepsList.Equals(GetAudioList()))
            {
                playedFootsteps.Clear();
                currentFootstepsList = GetAudioList();
            }

            if (playedFootsteps.Count >= GetAudioList().Count )
            {
                playedFootsteps.Clear();
            }

            selectedClip = GetAudioList()[Random.Range(0, GetAudioList().Count)];

            while (playedFootsteps.Contains(selectedClip))
            {
                selectedClip = GetAudioList()[Random.Range(0, GetAudioList().Count)];

                if (!playedFootsteps.Contains(selectedClip))
                {
                    break;
                }
            }

            playedFootsteps.Add(selectedClip);

            return selectedClip;
        }
        else
        {
            return selectedClip;
        }
    }

    /// <summary>
    /// Gets Surface Identity from left foot joint.
    /// </summary>
    /// <returns>SurfaceIndentity ground</returns>
    private SurfaceIdentity GetLeftSurfaceIdentity()
    {
        Ray ray = new Ray(leftFootJoint.transform.position, transform.up * -1);
        RaycastHit hitInfo = new RaycastHit();

        if (Physics.Raycast(ray, out hitInfo, .2f))
        {
            if (hitInfo.collider.TryGetComponent(out SurfaceMaterialData surfaceMaterialData))
            {
                ground = surfaceMaterialData.GetSurfaceIndentity();
                SetAudioList(surfaceMaterialData);
                currentFoot = currentFoot.left;
                return ground;
            }
            return 0;
        }
        return 0;
    }

    /// <summary>
    /// Gets Surface Identity from right foot joint.
    /// </summary>
    /// <returns>SurfaceIndentity ground</returns>
    private SurfaceIdentity GetRightSurfaceIdentity()
    {
        Ray ray = new Ray(rightFootJoint.transform.position, transform.up * -1);
        RaycastHit hitInfo = new RaycastHit();

        if (Physics.Raycast(ray, out hitInfo, .2f))
        {
            if (hitInfo.collider.TryGetComponent(out SurfaceMaterialData surfaceMaterialData))
            {
                ground = surfaceMaterialData.GetSurfaceIndentity();
                SetAudioList(surfaceMaterialData);
                currentFoot = currentFoot.right;
                return ground;
            }
            return 0;
        }
        return 0;
    }

    /// <summary>
    /// </summary>
    /// <returns>Current player move action</returns>
    private movementState GetMovementAction()
    {
        return p_movement.GetMovementAction();
    }

    /// <summary>
    /// </summary>
    /// <returns>Footstep list based on current player move action</returns>
    private List<AudioClip> GetAudioList()
    {
        currentMoveAction = GetMovementAction();

        if (currentMoveAction == movementState.walk || currentMoveAction == movementState.sneak)
        {
            return walkClips;
        }
        else if (currentMoveAction == movementState.sprint)
        {
            return sprintClips;
        }

        return null;
    }

    /// <summary>
    /// Sets walkClips and sprintClips based on surface material walk and sprint clip lists.
    /// </summary>
    /// <param name="_surfaceMaterialData"></param>
    private void SetAudioList(SurfaceMaterialData _surfaceMaterialData)
    {
        walkClips = _surfaceMaterialData.GetWalkClips();
        sprintClips = _surfaceMaterialData.GetSprintClips();
    }

    /// <summary>
    /// Spawns noise object on givel location with given noise value.
    /// </summary>
    /// <param name="_noiseObject"></param>
    /// <param name="_instantiator"></param>
    /// <param name="_parent"></param>
    /// <param name="_position"></param>
    /// <param name="_noise"></param>
    public static void SpawnNoiseObject(GameObject _noiseObject, GameObject _instantiator, GameObject _parent, Vector3 _position, float _noise)
    {
        GameObject noiseObject = Instantiate(_noiseObject, _position, _instantiator.transform.rotation, _parent.transform) as GameObject;
        // GameObject noiseObject = PrefabUtility.InstantiatePrefab(_noiseObject);
        // GameObject noiseObject = new GameObject("noiseObject");
        // noiseObject.transform.position = _position;
        // noiseObject.transform.SetParent(_parent.transform);
        NoiseSystem noiseSys = noiseObject.GetComponent<NoiseSystem>();

        noiseSys.SetNoise(_noise);
        noiseSys.SetParent(_instantiator);
        noiseSys.tag = "Noise";
    }
}
