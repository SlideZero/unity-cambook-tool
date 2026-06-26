using UnityEngine;

[CreateAssetMenu(fileName = "CamBookData", menuName = "CamBook/DataTemplate")]
public class DataValues : ScriptableObject
{
    public Vector3[] bmPositions = new Vector3[5];
    public Quaternion[] bmRotations = new Quaternion[5];
}
