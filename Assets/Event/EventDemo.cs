using System;
using UnityEngine;

namespace Event
{
    public class EventDemo : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("Awake");
        }

        private void Reset()
        {
            Debug.Log("Reset");
        }

        private void Start()
        {
            Debug.Log("Start");
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable");
        }

        private void OnEnable()
        {
            Debug.Log("OnEnable");
        }

        private void OnMouseDrag()
        {
            Debug.Log("OnMouseDrag");
        }

        private void OnBecameInvisible()
        {
            Debug.Log("OnBecameInvisible");
        }

        private void OnBecameVisible()
        {
            Debug.Log("OnBecameVisible");
        }

        private void OnMouseDown()
        {
            Debug.Log("OnMouseDown");
        }

        private void OnMouseEnter()
        {
            Debug.Log("OnMouseEnter");
        }

        private void OnMouseExit()
        {
            Debug.Log("OnMouseExit");
        }

        private void OnMouseOver()
        {
            Debug.Log("OnMouseOver");
        }

        private void OnMouseUp()
        {
            Debug.Log("OnMouseUp");
        }
    }
}
