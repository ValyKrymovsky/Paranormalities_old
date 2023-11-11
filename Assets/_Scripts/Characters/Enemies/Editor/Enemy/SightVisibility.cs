using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(E_Teacher))]
public class SightVisibility : Editor
{
    /*
    private void OnSceneGUI()
    {
        E_Teacher teacher = (E_Teacher)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(teacher.transform.position, Vector3.up, Vector3.forward, 360, teacher.sightDistance);

        Vector3 viewAngle01 = DirectionFromAngle(teacher.transform.eulerAngles.y, -teacher.sightAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(teacher.transform.eulerAngles.y, teacher.sightAngle / 2);

        Handles.color = Color.yellow;

        Handles.DrawLine(teacher.transform.position, teacher.transform.position + viewAngle01 * teacher.sightDistance);
        Handles.DrawLine(teacher.transform.position, teacher.transform.position + viewAngle02 * teacher.sightDistance);

        if (teacher.playerInSight)
        {
            Handles.color = Color.green;
            Handles.DrawLine(teacher.transform.position, teacher.FindPlayer().position);
        }
    }

    private Vector3 DirectionFromAngle(float _eulerY, float _anglesInDegrees)
    {
        _anglesInDegrees += _eulerY;

        return new Vector3(Mathf.Sin(_anglesInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(_anglesInDegrees * Mathf.Deg2Rad));
    }
    */
}
