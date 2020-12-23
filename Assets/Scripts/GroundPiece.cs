using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPiece : MonoBehaviour
{
    public bool isColored = false;
    public bool isAnimated = false;

    public void Colored(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
        isColored = true;

        FindObjectOfType<GameManager>().CheckComplete();
    }
    public void Animated(GameObject particle, Color color)
    {
        particle.GetComponent<ParticleSystem>().startColor = color;
        Instantiate(particle, transform.position, Quaternion.identity);
        isAnimated = true;
        FindObjectOfType<GameManager>().CheckComplete();
    }
}
