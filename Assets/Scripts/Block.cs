using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public bool m_Initialised;
    public BlockResource blockResource; 

	public void Start ()
    {
        if (!m_Initialised)
        {
            m_Initialised = true;
            blockResource = Resources.Load("BuildingBlocks/DefaultBlockResource", typeof(BlockResource)) as BlockResource;
        }
	}

    public void Update()
    {
        
    }

}
