using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MikeNspired.XRIStarterKit
{
    public class AutoSpawnObjectInHandOnGrab : MonoBehaviour
    {
        [SerializeField] private XRBaseInteractable _xrBaseInteractable;
        [SerializeField] private XRBaseInteractable prefabToSpawn;

        private HandReference currentHand, otherHand;

        private void Start()
        {
            if (_xrBaseInteractable == null)
            {
                Debug.LogError("XR Base Interactable is missing!");
                return;
            }

            _xrBaseInteractable.selectEntered.AddListener(OnGrab);
            _xrBaseInteractable.selectExited.AddListener(OnRelease);
        }

        private void OnRelease(SelectExitEventArgs args)
        {
            currentHand = null;
            otherHand = null;
        }

        private void OnGrab(SelectEnterEventArgs args)
        {
            currentHand = args.interactorObject.transform
                .GetComponentInParent<HandReference>();

            if (currentHand == null)
            {
                Debug.LogWarning("HandReference not found!");
                return;
            }

            otherHand = currentHand.OtherHand;

            if (otherHand == null)
            {
                Debug.LogWarning("OtherHand is not assigned!");
                return;
            }

            TrySpawn();
        }

        public virtual void TrySpawn()
        {
            if (!enabled) return;
            if (currentHand == null || otherHand == null) return;

            if (otherHand.NearFarInteractor == null)
            {
                Debug.LogWarning("NearFarInteractor is missing!");
                return;
            }

            if (otherHand.NearFarInteractor.hasSelection) return;

            if (prefabToSpawn == null)
            {
                Debug.LogWarning("Arrow prefab is missing!");
                return;
            }

            var spawnedObject = Instantiate(prefabToSpawn);

            otherHand.NearFarInteractor.interactionManager.SelectEnter(
                otherHand.NearFarInteractor,
                (IXRSelectInteractable)spawnedObject
            );
        }

        private void OnValidate()
        {
            if (!_xrBaseInteractable)
                _xrBaseInteractable = GetComponent<XRBaseInteractable>();
        }
    }
}