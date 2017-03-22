using UnityEngine;
namespace CurvedVRKeyboard {

    public class KeyboardRaycaster: KeyboardComponent {

        //------Raycasting----
        [SerializeField, HideInInspector]
        private Transform raycastingSource;

        [SerializeField, HideInInspector]
        private GameObject target;

        private float rayLength;
        private Ray ray;
        private RaycastHit hit;
        private LayerMask layer;
        //@SENIORVRPROJECT
        private bool wasSteamVRTriggerPressed;
        //@SENIORVRPROJECT
        //---interactedKeys---
        private KeyboardStatus keyboardStatus;
        private KeyboardItem keyItemCurrent;

        [SerializeField, HideInInspector]
        private string clickInputName;

        void Start () {
            keyboardStatus = gameObject.GetComponent<KeyboardStatus>();
            int layerNumber = gameObject.layer;
            layer = 1 << layerNumber;
            //@SENIORVRPROJECT
            wasSteamVRTriggerPressed = false;
            //@SENIORVRPROJECT
        }

        void Update () {
            rayLength = Vector3.Distance(raycastingSource.position, target.transform.position) * 1.1f;
            RayCastKeyboard();
        }

        /// <summary>
        /// Check if camera is pointing at any key. 
        /// If it does changes state of key
        /// </summary>
        private void RayCastKeyboard () {
            //@SENIORVRPROJECT
            bool isSteamVRTriggerPressed = false;
            if (raycastingSource.gameObject != null)
            {
                SteamVR_TrackedController controller = raycastingSource.gameObject.GetComponent<SteamVR_TrackedController>();
                if (controller != null)
                {
                    if (controller.triggerPressed)
                    {
                        isSteamVRTriggerPressed = true;
                    }
                }
            }
            //@SENIORVRPROJECT
            ray = new Ray(raycastingSource.position, raycastingSource.forward);
            if(Physics.Raycast(ray, out hit, rayLength, layer)) { // If any key was hit
                KeyboardItem focusedKeyItem = hit.transform.gameObject.GetComponent<KeyboardItem>();
                if(focusedKeyItem != null) { // Hit may occur on item without script
                    ChangeCurrentKeyItem(focusedKeyItem);
                    keyItemCurrent.Hovering();

                    //@SENIORVRPROJECT
                    bool steamVRTriggerClick = !wasSteamVRTriggerPressed && isSteamVRTriggerPressed;
                    if (steamVRTriggerClick || Input.GetButtonDown(clickInputName))
                    {// If key clicked
                        Debug.Log("Keyboard button pressed");
                        keyItemCurrent.Click();
                        keyboardStatus.HandleClick(keyItemCurrent);
                    }
                    //@SENIORVRPROJECT

                    if (Input.GetButtonDown(clickInputName)) {// If key clicked
                        keyItemCurrent.Click();
                        keyboardStatus.HandleClick(keyItemCurrent);
                    }
                }
            } else if(keyItemCurrent != null) {// If no target hit and lost focus on item
                ChangeCurrentKeyItem(null);
            }
            //@SENIORVRPROJECT
            wasSteamVRTriggerPressed = isSteamVRTriggerPressed;
            //@SENIORVRPROJECT
        }

        private void ChangeCurrentKeyItem ( KeyboardItem key ) {
            if(keyItemCurrent != null) {
                keyItemCurrent.StopHovering();
            }
            keyItemCurrent = key;
        }

        //---Setters---
        public void SetRayLength ( float rayLength ) {
            this.rayLength = rayLength;
        }

        public void SetRaycastingTransform ( Transform raycastingSource ) {
            this.raycastingSource = raycastingSource;
        }

        public void SetClickButton ( string clickInputName ) {
            this.clickInputName = clickInputName;
        }

        public void SetTarget ( GameObject target ) {
            this.target = target;
        }
    }
}