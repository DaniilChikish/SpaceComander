using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Mechanics
{
    public class CameraMovementScript : MonoBehaviour
    {
        public int speed;
        public int rotSpeed;
        private GameObject player;
        private float holding;
        Transform mapCam;
        //private Vector3 rotSpeedRight;
        //private Vector3 rotSpeedLeft;
        // Use this for initialization
        void Start()
        {
            player = this.gameObject;
            mapCam = this.transform.Find("MapCam");
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
            Vector3 newPosition = player.transform.position;
            if (Input.GetKey(KeyCode.W))
            {
                newPosition += player.transform.forward * speed * (Time.deltaTime + holding / 4);
                if (holding<5) holding += Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                newPosition += -player.transform.forward * speed * (Time.deltaTime + holding / 4);
                if (holding < 5) holding += Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                newPosition += player.transform.right * speed * (Time.deltaTime + holding / 4);
                if (holding < 5) holding += Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                newPosition += -player.transform.right * speed * (Time.deltaTime + holding / 4);
                if (holding < 5) holding += Time.deltaTime;
            }
            if (newPosition.magnitude < 2500)
                player.transform.position = newPosition;

            if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
                if (holding > 0.00001f) holding = holding * 0.7f;
                else holding = 0;
            if (Input.GetKey(KeyCode.E))
            {
                player.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
                mapCam.transform.Rotate(-Vector3.forward * -rotSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                player.transform.Rotate(Vector3.up * -rotSpeed * Time.deltaTime);
                mapCam.transform.Rotate(-Vector3.forward * rotSpeed * Time.deltaTime);
            }
        }
    }
}
