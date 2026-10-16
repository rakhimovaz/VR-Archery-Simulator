
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MikeNspired.XRIStarterKit
{
    public class HandReference : MonoBehaviour
    {
        [field: SerializeField]
        public HandAnimator Hand { get; private set; }

        [field: SerializeField]
        public LeftRight LeftRight { get; private set; }

        [field: SerializeField]
        public HandReference OtherHand { get; private set; }

        [field: SerializeField]
        public NearFarInteractor NearFarInteractor { get; private set; }

        private Transform handModelAttach;
        private Transform attachTransform;

        private Vector3 originalLocalPos;
        private Quaternion originalLocalRot;

        private HandPoser currentHandPoser;

        private void OnValidate()
        {
            if (Hand == null)
                Hand = GetComponentInChildren<HandAnimator>();

            if (NearFarInteractor == null)
                NearFarInteractor = GetComponentInChildren<NearFarInteractor>();
        }

        private void Awake()
        {
            if (NearFarInteractor == null)
                NearFarInteractor = GetComponentInChildren<NearFarInteractor>();

            if (NearFarInteractor == null)
            {
                Debug.LogError(
                    "HandReference: NearFarInteractor is missing on " + name,
                    this
                );

                enabled = false;
                return;
            }

            attachTransform = NearFarInteractor.attachTransform;

            if (attachTransform == null)
            {
                Debug.LogError(
                    "HandReference: Attach Transform is missing on " + name,
                    this
                );

                enabled = false;
                return;
            }

            originalLocalPos = attachTransform.localPosition;
            originalLocalRot = attachTransform.localRotation;

            handModelAttach = new GameObject(
                "HandModelAttach_" + name
            ).transform;

            // Use the hand model if available.
            // Otherwise, use the controller transform.
            Transform parentTransform =
                Hand != null ? Hand.transform : transform;

            handModelAttach.SetParent(parentTransform, false);
            handModelAttach.localPosition = Vector3.zero;
            handModelAttach.localRotation = Quaternion.identity;

            NearFarInteractor.selectEntered.AddListener(OnGrab);
            NearFarInteractor.selectExited.AddListener(OnRelease);
        }

        private void OnDestroy()
        {
            if (NearFarInteractor != null)
            {
                NearFarInteractor.selectEntered.RemoveListener(OnGrab);
                NearFarInteractor.selectExited.RemoveListener(OnRelease);
            }

            if (handModelAttach != null)
                Destroy(handModelAttach.gameObject);
        }

        private void OnRelease(SelectExitEventArgs args)
        {
            ResetAttachTransform();
            currentHandPoser = null;
        }

        private void OnGrab(SelectEnterEventArgs args)
        {
            if (NearFarInteractor == null ||
                attachTransform == null ||
                handModelAttach == null)
                return;

            // Skip far interactions.
            if (NearFarInteractor.interactionAttachController != null &&
                NearFarInteractor.interactionAttachController.hasOffset)
                return;

            // Without a hand animator, keep the normal
            // controller grab behavior.
            if (Hand == null)
                return;

            FindHandPoser(args);

            if (currentHandPoser == null)
                return;

            Transform interactableAttach =
                LeftRight == LeftRight.Left
                    ? currentHandPoser.leftHandAttach
                    : currentHandPoser.rightHandAttach;

            if (interactableAttach == null)
                return;

            Vector3 finalPosition =
                interactableAttach.localPosition * -1;

            Quaternion finalRotation =
                Quaternion.Inverse(interactableAttach.localRotation);

            finalPosition = RotatePointAroundPivot(
                finalPosition,
                Vector3.zero,
                finalRotation.eulerAngles
            );

            handModelAttach.localPosition = finalPosition;
            handModelAttach.localRotation = finalRotation;

            attachTransform.position = handModelAttach.position;
            attachTransform.rotation = handModelAttach.rotation;
        }

        private void FindHandPoser(SelectEnterEventArgs args)
        {
            currentHandPoser =
                args.interactableObject.transform
                    .GetComponent<XRHandPoser>();

            if (currentHandPoser == null)
            {
                currentHandPoser =
                    args.interactableObject.transform
                        .GetComponentInChildren<XRHandPoser>();
            }
        }

        public void ResetAttachTransform()
        {
            if (attachTransform == null)
                return;

            attachTransform.localPosition = originalLocalPos;
            attachTransform.localRotation = originalLocalRot;
        }

        private static Vector3 RotatePointAroundPivot(
            Vector3 point,
            Vector3 pivot,
            Vector3 angles)
        {
            Vector3 direction = point - pivot;
            direction = Quaternion.Euler(angles) * direction;
            return direction + pivot;
        }
    }
}
