using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SurfaceMaterialData : MonoBehaviour
{
    public SurfaceMaterial surfaceMaterial;

    public SurfaceIdentity GetSurfaceIndentity()
    {
        return surfaceMaterial.identity;
    }

    public List<AudioClip> GetWalkClips()
    {
        if (surfaceMaterial.useWalkClips)
        {
            return surfaceMaterial.walkClips;
        }
        return null;
    }

    public List<AudioClip> GetSprintClips()
    {
        if (surfaceMaterial.useSprintClips)
        {
            return surfaceMaterial.sprintClips;
        }
        return null;
    }

}
