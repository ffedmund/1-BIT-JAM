using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    public float speed = 5;
    public float lifeTime = 1;
    public Vector3 direction;
    public bool inited;

    public void SetUp(Vector3 direction)
    {
        this.direction = new Vector3(direction.x,0,0);
        ShockWave opposite = Instantiate(gameObject,transform.position, Quaternion.identity).GetComponent<ShockWave>();
        opposite.direction = -this.direction;
        opposite.inited = true;
        inited = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(!inited)return;
        transform.position += direction.normalized * speed * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
            Destroy(gameObject);
    }
}
