using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MikeNspired.XRIStarterKit
{
    public class PullInteraction : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] private AudioRandomize pullBackAudio, arrowNotchedAudio, launchClip;
        [SerializeField] private Transform start, end, stringPosition;
        [SerializeField] private AutoSpawnObjectInHandOnGrab autoSpawnObjectInHandOnGrab;

        private XRBaseInteractable xrBaseInteractable;
        private XRBaseInteractor currentInteractor;
        private HapticImpulsePlayer currentHapticImpulsePlayer;

        private float pullAmount;
        private bool isSelected = false;
        private bool canPlayPullBackSound = true;

        private Arrow notchedArrow;
        private Collider[] colliders;
        private float lastFramePullBack;

        public Arrow NotchedArrow => notchedArrow;

        private void OnValidate()
        {
            if (!xrBaseInteractable)
                xrBaseInteractable = GetComponent<XRBaseInteractable>();
        }

        private void Start()
        {
            OnValidate();

            colliders = transform.parent.GetComponentsInChildren<Collider>(true);

            if (colliders.Length == 0)
                Debug.LogWarning("No colliders found for the bow.");

            xrBaseInteractable.selectEntered.AddListener(OnSelectEntered);
            xrBaseInteractable.selectExited.AddListener(OnSelectExited);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (notchedArrow) return;

            var attachedRigidbody = other.attachedRigidbody;
            if (attachedRigidbody == null) return;

            if (!attachedRigidbody.TryGetComponent<XRGrabInteractable>(
                    out var arrowInteractable))
                return;

            if (!arrowInteractable.isSelected) return;

            if (!(arrowInteractable.firstInteractorSelecting
                  is XRBaseInteractor arrowHolder))
                return;

            if (!attachedRigidbody.TryGetComponent(out Arrow arrow))
                return;

            HandleArrowInteraction(arrowHolder, arrow, arrowInteractable);
        }

        private void HandleArrowInteraction(
            XRBaseInteractor arrowHolder,
            Arrow arrow,
            XRGrabInteractable arrowInteractable)
        {
            arrowHolder.interactionManager.SelectExit(
                (IXRSelectInteractor)arrowHolder,
                arrowInteractable
            );

            notchedArrow = arrow;

            notchedArrow.transform.SetParent(transform);

            notchedArrow.transform.SetLocalPositionAndRotation(
                start.localPosition,
                start.localRotation
            );

            if (notchedArrow.TryGetComponent<Rigidbody>(
                    out var arrowRigidbody))
            {
                arrowRigidbody.isKinematic = true;
            }

            arrowInteractable.enabled = false;

            arrowNotchedAudio.Play();
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            currentInteractor = args.interactorObject as XRBaseInteractor;

            if (currentInteractor == null) return;

            isSelected = true;

            currentHapticImpulsePlayer =
                currentInteractor.GetComponentInParent<HapticImpulsePlayer>();

            currentHapticImpulsePlayer?.SendHapticImpulse(0.7f, 0.05f);
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            var interactor = args.interactorObject as XRBaseInteractor;

            if (interactor == null) return;

            isSelected = false;
            currentInteractor = null;
            canPlayPullBackSound = true;

            launchClip.Play(pullAmount);

            if (pullAmount > 0 && notchedArrow)
            {
                notchedArrow.transform.SetParent(null);

                notchedArrow.Release(pullAmount, colliders);

                notchedArrow = null;

                if (autoSpawnObjectInHandOnGrab != null)
                {
                    autoSpawnObjectInHandOnGrab.TrySpawn();
                }
            }

            pullAmount = 0f;

            skinnedMeshRenderer.SetBlendShapeWeight(0, 0);
        }

        private void Update()
        {
            if (!isSelected || currentInteractor == null)
                return;

            Vector3 pullPosition = currentInteractor.transform.position;

            pullAmount = CalculatePull(pullPosition);

            skinnedMeshRenderer.SetBlendShapeWeight(
                0,
                pullAmount * 100
            );

            stringPosition.position = Vector3.Lerp(
                start.position,
                end.position,
                pullAmount
            );

            stringPosition.rotation = Quaternion.Lerp(
                start.rotation,
                end.rotation,
                pullAmount
            );

            if (notchedArrow != null)
            {
                notchedArrow.transform.SetPositionAndRotation(
                    stringPosition.position,
                    stringPosition.rotation
                );
            }

            if (pullAmount > 0.3f)
            {
                if (Mathf.Abs(lastFramePullBack - pullAmount) > 0.01f)
                {
                    currentHapticImpulsePlayer?.SendHapticImpulse(
                        pullAmount / 5f,
                        0.05f
                    );
                }

                if (canPlayPullBackSound)
                {
                    canPlayPullBackSound = false;
                    pullBackAudio.Play();
                }
            }
            else
            {
                canPlayPullBackSound = true;
            }

            lastFramePullBack = pullAmount;
        }

        private float CalculatePull(Vector3 pullPosition)
        {
            Vector3 pullDirection = pullPosition - start.position;
            Vector3 targetDirection = end.position - start.position;

            float maxLength = targetDirection.magnitude;

            targetDirection.Normalize();

            float pullValue =
                Vector3.Dot(pullDirection, targetDirection) / maxLength;

            return Mathf.Clamp(pullValue, 0, 1);
        }
    }
}