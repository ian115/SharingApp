using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pool", menuName = "Pool")]
public class Pool : ScriptableObject
{
    public string title;
    public string type;
    public float amount;

}
