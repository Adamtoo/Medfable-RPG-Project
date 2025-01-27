using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float horizontalInput;
    [SerializeField]
    private float speed = 20;
    [SerializeField]
    private GameObject projectilePrefab;
    private float xRange = 15;

    // Update is called once per frame
    void Update()
    {
        //Allows the player to move left and right using their keys
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * Time.deltaTime * horizontalInput * speed);
        
        //Checks to see if the player is within the game boundaries and repositions them
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -xRange, xRange), transform.position.y, transform.position.z);
        
        //Allow player to shoot projectile when pressing spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(projectilePrefab, transform.position, projectilePrefab.transform.rotation);
        }
    }
}
