using UnityEngine;

namespace MiniMap
{
    public class MiniMapPlayer : MonoBehaviour
    {
        public float moveSpeed;

        private void Update()
        {
            var target = new Vector3(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0,
                Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
            transform.Translate(target);
        }
    }
}