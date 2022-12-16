using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{

    public List<string> poolId;
    public List<string> poolTitle;
    public List<string> poolType;
    public List<string> poolDescription;
    public List<string> entryAssociationId;

    public UserData(JSONReader jsonReader)
    {
        poolId = jsonReader.GetPoolID();
        poolTitle = jsonReader.GetPoolTitle();
        poolType = jsonReader.GetPoolType();
        poolDescription = jsonReader.GetPoolDescription();
        entryAssociationId = jsonReader.GetAssociationId();
    }

}
