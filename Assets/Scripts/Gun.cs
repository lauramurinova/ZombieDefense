using System.Collections;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour, IHandGrabUseDelegate
{
        [Header("Input")] [SerializeField] private Transform _trigger;
        [SerializeField] private Transform _barrel;
        [SerializeField] private AnimationCurve _triggerRotationCurve;
        [SerializeField] private SnapAxis _axis = SnapAxis.X;
        [SerializeField] [Range(0f, 1f)] private float _releaseThresold = 0.3f;
        [SerializeField] [Range(0f, 1f)] private float _fireThresold = 0.9f;
        [SerializeField] private float _triggerSpeed = 3f;
        [SerializeField] private AnimationCurve _strengthCurve = AnimationCurve.EaseInOut(0f,0f,1f,1f);
        
        [Header("Settings")]
        [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
        [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
        [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;
        
        [Header("Prefab Refrences")]
        public GameObject bulletPrefab;
        public GameObject muzzleFlashPrefab;

        private bool _wasFired = false;
        private float _dampedUseStrength = 0;
        private float _lastUseTime;

        private void UpdateTriggerRotation(float progress)
        {
            float value = _triggerRotationCurve.Evaluate(progress);
            Vector3 angles = _trigger.localEulerAngles;
            if ((_axis & SnapAxis.X) != 0)
            {
                angles.x = value;
            }
            if ((_axis & SnapAxis.Y) != 0)
            {
                angles.y = value;
            }
            if ((_axis & SnapAxis.Z) != 0)
            {
                angles.z = value;
            }
            _trigger.localEulerAngles = angles;
        }

        private void Fire()
        {
            if (muzzleFlashPrefab)
            {
                GameObject tempFlash;
                tempFlash = Instantiate(muzzleFlashPrefab, _barrel.position, _barrel.rotation);

                Destroy(tempFlash, destroyTimer);
            }

            if (!bulletPrefab)
            { return; }

            Instantiate(bulletPrefab, _barrel.position, _barrel.rotation).GetComponent<Rigidbody>().AddForce(_barrel.forward * shotPower, ForceMode.Acceleration);
        }

        public void BeginUse()
        {
            _dampedUseStrength = 0f;
            _lastUseTime = Time.realtimeSinceStartup;
        }

        public void EndUse()
        {
        }

        public float ComputeUseStrength(float strength)
        {
            float delta = Time.realtimeSinceStartup - _lastUseTime;
            _lastUseTime = Time.realtimeSinceStartup;
            if (strength > _dampedUseStrength)
            {
                _dampedUseStrength = Mathf.Lerp(_dampedUseStrength, strength, _triggerSpeed * delta);
            }
            else
            {
                _dampedUseStrength = strength;
            }
            float progress = _strengthCurve.Evaluate(_dampedUseStrength);
            UpdateTriggerProgress(progress);
            return progress;
        }

        private void UpdateTriggerProgress(float progress)
        {
            UpdateTriggerRotation(progress);

            if (progress >= _fireThresold && !_wasFired)
            {
                _wasFired = true;
                Fire();
            }
            else if (progress <= _releaseThresold)
            {
                _wasFired = false;
            }
        }
}
