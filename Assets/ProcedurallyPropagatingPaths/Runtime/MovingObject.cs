using System.Collections;
using UnityEngine;

namespace PPP
{
    public class MovingObject : MonoBehaviour
    {
        public float Lifetime { get; private set; }
        public float DestroyDuration { get; private set; }
        public ObjectPath Path { get; private set; }
        public float Speed { get; private set; }
        public void Initialize(float lifetime, float destroyDuration, ObjectPath path, float speed)
        {
            Lifetime = lifetime;
            DestroyDuration = destroyDuration;
            Path = path;
            Speed = speed;
        }
        private IEnumerator Start()
        {
            AnimateFromInitialPoint(Lifetime);
            yield return new WaitForSeconds(Lifetime - DestroyDuration);
            DestroySelf(DestroyDuration);
        }
        public void AnimateFromInitialPoint(float lifetime)
        {
            StartCoroutine(AnimateFromInitialPointRoutine(lifetime));
        }

        private IEnumerator AnimateFromInitialPointRoutine(float lifetime)
        {
            //Make the particle follow the path by updating it's position and rotation
            if (TryGetComponent(out ParticleSystem ps))
            {
                ps.Play();
            }
            transform.position = Path.AnchorPoints[0].GetPosition();
            transform.forward = Path.AnchorPoints[0].GetDirection();
            float timer = 0;
            while (timer < lifetime)
            {
                float lerpValue = timer * Speed;
                transform.position = Path.GetObjectPositionAndForward(lerpValue)[0];
                transform.forward = Path.GetObjectPositionAndForward(lerpValue)[1];
                timer += Time.deltaTime;
                yield return null;
            }
        }

        public void DestroySelf(float duration)
        {
            StartCoroutine(DestroySelfRoutine(duration));
        }

        private IEnumerator DestroySelfRoutine(float duration)
        {
            //Scales down to Vector3.zero and destroys itself
            float timer = 0;
            Vector3 initialScale = transform.localScale;
            while (timer < duration)
            {
                transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }

    }
}
