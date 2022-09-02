using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameobjectID : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public ushort goID;

    public ushort GoID   // property
    {
        get { return goID; }   // get method
        set { goID = value; }  // set method
    }
    //If you need to access the ID, use this
}
