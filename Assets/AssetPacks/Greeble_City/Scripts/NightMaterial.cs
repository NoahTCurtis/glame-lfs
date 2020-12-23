// Replaces object layout material with night version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NightMaterial : MonoBehaviour
{
    public Material Night_Material;
    void Start()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform child_sub in child)
            {
                Renderer rend = child_sub.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = Night_Material;
                }
            }
        }
    }
}
