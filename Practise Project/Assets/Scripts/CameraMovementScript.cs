using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class CameraMovementScript : MonoBehaviour
    {

        public int speed;
        public int rotSpeed;
        private GameObject player;
        //private Vector3 rotSpeedRight;
        //private Vector3 rotSpeedLeft;
        // Use this for initialization
        void Start()
        {
            player = (GameObject)this.gameObject;
            //Vector3 rotSpeedRight = new Vector3(0, rotSpeed, 0);
            //Vector3 rotSpeedLeft = new Vector3(0, rotSpeed * (-1), 0);
        }

        // Update is called once per frame
        void Update()
        {
            /*
             * if (Input.GetKey(KeyCode.UpArrow))
             *    player.transform.position += player.transform.forward * speed * Time.deltaTime;
             * if (Input.GetKey(KeyCode.DownArrow))
             *    player.transform.position -= player.transform.forward * speed * Time.deltaTime;
             * if (Input.GetKey(KeyCode.RightArrow))
             *    player.transform.position += player.transform.right * speed * Time.deltaTime;
             * if (Input.GetKey(KeyCode.LeftArrow))
             *    player.transform.position -= player.transform.right * speed * Time.deltaTime;
             */
            if (Input.GetKey(KeyCode.W))
                player.transform.position += player.transform.forward * speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.S))
                player.transform.position -= player.transform.forward * speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.D))
                player.transform.position += player.transform.right * speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.A))
                player.transform.position -= player.transform.right * speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.E))
            {
                player.transform.Rotate(Vector3.up * rotSpeed / 10);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                player.transform.Rotate(Vector3.up * rotSpeed * -1 / 10);
            }
        }
    }
}
