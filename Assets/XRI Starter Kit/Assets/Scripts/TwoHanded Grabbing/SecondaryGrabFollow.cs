
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MikeNspired.XRIStarterKit
{
    public class SecondaryGrabFollow : MonoBehaviour
    {
        [Header("Interactable References")]
        [SerializeField]
        private XRGrabInteractable mainInteractable;

        [SerializeField]
        protected XRBaseInteractable interactable;

        [Header("Rotation Options")]
        [SerializeField]
        private bool rotateOnY = false, rotateOnZ = false;

        private XRBaseInteractor currentHand;
        private XRBaseInteractor mainGripHand;
        private XRInteractionManager interactionManager;

        private Transform interactableStartingParent;
        private Transform newHandAttachTransform;
        private Transform startingParent;

        private Quaternion mainInteractableHandAttachTransformStartingRotation;
        private Vector3 mainHandAttachStartingRotation;
        private Vector3 mainHandStartingRotation;

        private TransformStruct interactableStartingTransformData;

        private void Awake()
        {
            OnValidate();

            if (interactable == null || mainInteractable == null)
            {
                Debug.LogError(
                    "SecondaryGrabFollow: Interactable references are missing.",
                    this
                );
                enabled = false;
                return;
            }

            interactable.selectEntered.AddListener(OnSecondarySelectEntered);
            interactable.selectExited.AddListener(OnSecondarySelectExited);

            mainInteractable.selectEntered.AddListener(OnMainSelectEntered);
            mainInteractable.selectExited.AddListener(OnMainSelectExited);

            interactableStartingTransformData.SetTransformStruct(
                interactable.transform.localPosition,
                interactable.transform.localRotation,
                interactable.transform.localScale
            );

            interactableStartingParent = interactable.transform.parent;

            newHandAttachTransform = new GameObject(
                mainInteractable.name + " Follow Hand Helper"
            ).transform;

            newHandAttachTransform.SetParent(transform);
        }

        private void OnValidate()
        {
            if (interactable == null)
                interactable = GetComponent<XRGrabInteractable>();

            if (interactionManager == null)
                interactionManager = FindFirstObjectByType<XRInteractionManager>();
        }

        private void OnDestroy()
        {
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(OnSecondarySelectEntered);
                interactable.selectExited.RemoveListener(OnSecondarySelectExited);
            }

            if (mainInteractable != null)
            {
                mainInteractable.selectEntered.RemoveListener(OnMainSelectEntered);
                mainInteractable.selectExited.RemoveListener(OnMainSelectExited);
            }

            if (newHandAttachTransform != null)
            {
                if (mainGripHand != null &&
                    mainGripHand.attachTransform != null &&
                    mainGripHand.attachTransform.parent == newHandAttachTransform)
                {
                    mainGripHand.attachTransform.SetParent(startingParent, true);
                }

                Destroy(newHandAttachTransform.gameObject);
            }
        }

        private void OnSecondarySelectEntered(SelectEnterEventArgs args)
        {
            currentHand = args.interactorObject as XRBaseInteractor;

            if (currentHand == null ||
                mainGripHand == null ||
                mainGripHand.attachTransform == null ||
                mainGripHand.transform.parent == null ||
                newHandAttachTransform == null)
            {
                Debug.LogWarning(
                    "SecondaryGrabFollow: Main hand is not ready.",
                    this
                );

                currentHand = null;
                return;
            }

            SetStartingFromToRotation();

            mainHandStartingRotation =
                mainGripHand.transform.parent.localEulerAngles;

            mainHandAttachStartingRotation =
                mainGripHand.attachTransform.localEulerAngles;
        }

        private void OnSecondarySelectExited(SelectExitEventArgs args)
        {
            ResetMainHandAttachTransform();

            if (interactable != null)
            {
                Transform t = interactable.transform;

                t.SetParent(interactableStartingParent, false);
                t.localPosition = interactableStartingTransformData.position;
                t.localRotation = interactableStartingTransformData.rotation;
                t.localScale = interactableStartingTransformData.scale;
            }

            currentHand = null;
        }

        private void OnMainSelectEntered(SelectEnterEventArgs args)
        {
            mainGripHand = args.interactorObject as XRBaseInteractor;

            if (mainGripHand == null ||
                mainGripHand.attachTransform == null)
            {
                Debug.LogWarning(
                    "SecondaryGrabFollow: Invalid main hand.",
                    this
                );

                mainGripHand = null;
                return;
            }

            startingParent = mainGripHand.attachTransform.parent;

            mainInteractableHandAttachTransformStartingRotation =
                mainGripHand.attachTransform.localRotation;
        }

        private void OnMainSelectExited(SelectExitEventArgs args)
        {
            if (currentHand != null)
            {
                ResetMainHandAttachTransform();

                if (interactionManager != null && interactable != null)
                {
                    interactionManager.SelectExit(
                        currentHand,
                        (IXRSelectInteractable)interactable
                    );
                }

                currentHand = null;
            }

            if (mainGripHand != null)
            {
                HandReference handReference =
                    mainGripHand.GetComponentInParent<HandReference>();

                if (handReference != null && handReference.isActiveAndEnabled)
                {
                    handReference.ResetAttachTransform();
                }

                if (mainGripHand.attachTransform != null)
                {
                    mainGripHand.attachTransform.SetParent(startingParent, true);

                    mainGripHand.attachTransform.localRotation =
                        mainInteractableHandAttachTransformStartingRotation;
                }
            }

            mainGripHand = null;
        }

        private void ResetMainHandAttachTransform()
        {
            if (mainGripHand == null ||
                mainGripHand.attachTransform == null)
                return;

            mainGripHand.attachTransform.SetParent(startingParent, true);

            mainGripHand.attachTransform.localRotation =
                mainInteractableHandAttachTransformStartingRotation;

            if (newHandAttachTransform != null)
                newHandAttachTransform.SetParent(transform, true);
        }

        private void Update()
        {
            if (currentHand != null &&
                mainGripHand != null &&
                mainGripHand.attachTransform != null)
            {
                SetRotation();
            }
        }

        private void SetStartingFromToRotation()
        {
            if (currentHand == null ||
                mainGripHand == null ||
                mainGripHand.attachTransform == null ||
                newHandAttachTransform == null)
                return;

            Transform attach = mainGripHand.attachTransform;

            newHandAttachTransform.position = attach.position;

            if (currentHand.transform.position != attach.position)
            {
                newHandAttachTransform.LookAt(currentHand.transform);
            }

            newHandAttachTransform.SetParent(attach.parent, true);

            attach.SetParent(newHandAttachTransform, true);
        }

        private void SetRotation()
        {
            if (currentHand == null ||
                mainGripHand == null ||
                mainGripHand.attachTransform == null ||
                newHandAttachTransform == null)
                return;

            newHandAttachTransform.LookAt(currentHand.transform);

            Vector3 r = mainHandAttachStartingRotation;

            if (rotateOnZ)
            {
                if (mainGripHand.transform.parent == null)
                    return;

                float rToAdd =
                    mainGripHand.transform.parent.localEulerAngles.z -
                    mainHandStartingRotation.z;

                mainGripHand.attachTransform.localEulerAngles =
                    new Vector3(r.x, r.y, r.z + rToAdd);
            }
            else if (rotateOnY)
            {
                float rToAdd =
                    mainGripHand.transform.localEulerAngles.y -
                    mainHandStartingRotation.y;

                mainGripHand.attachTransform.localEulerAngles = r;

                mainGripHand.attachTransform.Rotate(
                    mainGripHand.attachTransform.up,
                    rToAdd,
                    Space.World
                );
            }
        }
    }
}
