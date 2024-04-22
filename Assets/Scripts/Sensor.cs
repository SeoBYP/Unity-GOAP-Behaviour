using System;
using UnityEngine;
using Utility;

namespace GOAP
{
    [RequireComponent(typeof(SphereCollider))]
    public class Sensor : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 5f;
        [SerializeField] private float timerInterval = 1f;

        public event Action OnTargetChanged = delegate { };

        private SphereCollider detectionRange;
        private GameObject target;
        private Vector3 lastKnownPosition;
        private CountdownTimer _timer;
        public Vector3 TargetPosition => target ? target.transform.position : Vector3.zero;
        public bool IsTargetInRange => TargetPosition != Vector3.zero;

        private void Awake()
        {
            detectionRange = GetComponent<SphereCollider>();
            detectionRange.isTrigger = true;
            detectionRange.radius = detectionRadius;
        }

        private void Start()
        {
            _timer = new CountdownTimer(timerInterval);
            _timer.OnTimerStop += () =>
            {
                UpdateTargetPosition(target.OrNull());
                _timer.Start();
            };
            _timer.Start();
        }



        void Update()
        {
            _timer.Tick(Time.deltaTime);
        }


        void UpdateTargetPosition(GameObject target = null)
        {
            this.target = target;
            if (IsTargetInRange && (lastKnownPosition != TargetPosition || lastKnownPosition != Vector3.zero))
            {
                lastKnownPosition = TargetPosition;
                OnTargetChanged.Invoke();
            }
        }


        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            UpdateTargetPosition(other.gameObject);
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            UpdateTargetPosition();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = IsTargetInRange ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}