using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15;


    [SerializeField] GameObject particle;

    public int minSwipeRecognition = 10;

    private bool isTraveling;
    private Vector3 travelDirection;

    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;

    private Vector3 nextCollisionPosition;

    private Color solveColor;

    private void Start()
    {
        solveColor = Random.ColorHSV(.5f, 1); // Açık renklerden random alır
        GetComponent<MeshRenderer>().material.color = solveColor;
    }

    private void FixedUpdate()
    {
        // Hareket etmesi gerektiğinde topların hızını ayarlar
        if (isTraveling) {
            rb.velocity = travelDirection * speed;
        }

        // Zemini boyar
        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up/2), .05f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();

            if (ground && !ground.isColored)
            {
                ground.Colored(solveColor);
                ground.Animated(particle,solveColor);
            }

            i++;
        }

        // Hadefe-duvara ulaşıldı mı kontrol et
        if (nextCollisionPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isTraveling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }

        if (isTraveling)
            return;

        // Swipe mekaniği
        if (Input.touchCount > 0 && Input.GetTouch(0).phase ==TouchPhase.Moved)
        {
            // Parmağın son konumu
            swipePosCurrentFrame = new Vector2(-Input.GetTouch(0).deltaPosition.x, -Input.GetTouch(0).deltaPosition.y);

            if (swipePosLastFrame != Vector2.zero)
            {

                // Kaydırma yönünün hesaplar
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                if (currentSwipe.sqrMagnitude < minSwipeRecognition) // Minimum parmak hareteti
                    return;

                currentSwipe.Normalize(); //Mesafeyi değil, yalnızca yönü elde etmek için normalleştirdik (topların hızını taklit edecek)
                // Yukarı-Aşağı swipe
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back); 
                }   

                // Sağa-Sola  swipe
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
            }


            swipePosLastFrame = swipePosCurrentFrame;
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
    }

    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        // Hangi nesneyle çarpışacağını kontrol etme
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isTraveling = true;
    }
}
