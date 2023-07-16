using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable, CreateAssetMenu(fileName = "NewSurfaceMaterial", menuName = "SurfaceMaterial")]
public class SurfaceMaterial : ScriptableObject
{
    public SurfaceIdentity identity;
    public bool useWalkClips;
    public List<AudioClip> walkClips;
    public bool useSprintClips;
    public List<AudioClip> sprintClips;
}
