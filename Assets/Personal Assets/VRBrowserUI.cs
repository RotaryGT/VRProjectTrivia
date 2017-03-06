﻿#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
#define ZF_OSX
#endif

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZenFulcrum.EmbeddedBrowser
{

    /**
     * This class will handle input to a browser based on the mouse position and a mesh collider on the browser.
     * Mouse positions are looked up according to the UVs on the *collider* mesh. Generally, you will want to use
     * the same or a visually similar (including UVs) mesh for the renderer.
     */
    public class VRBrowserUI : MonoBehaviour, IBrowserUI
    {
        /**
         * Creates a new UI handler.
         * We will attach to {parent}, which must have the mesh we are interacting with.
         * In most cases, this will also be the same object that has the Browser we will be fed to. (a la browser.UIHandler)
         */
        public static VRBrowserUI Create(MeshCollider meshCollider, Transform worldPointer, VRCursor cursorRenderer)
        {
            var ui = meshCollider.gameObject.GetComponent<VRBrowserUI>();
            if (!ui) ui = meshCollider.gameObject.AddComponent<VRBrowserUI>();
            ui.meshCollider = meshCollider;
            ui.worldPointer = worldPointer;
            ui.cursorRenderer = cursorRenderer;

            return ui;
        }

        public void Awake()
        {
            BrowserCursor = new BrowserCursor();
            BrowserCursor.cursorChange += CursorUpdated;

            InputSettings = new BrowserInputSettings();
        }
        public void Start()
        {
            VRCursor.SetUpBrowserInput(GetComponent<Browser>(), GetComponent<MeshCollider>());
        }

        public GameObject vrCursorObject;

        protected MeshCollider meshCollider;
        protected SteamVR_TrackedController controller;
        protected Transform worldPointer;
        protected VRCursor cursorRenderer;

        /**
         * How far can we reach to touch a browser?
         *
         * HideInInspector:
         * Showing it in the inspector would imply that changing the value would be used, but in most practical cases
         * with FPSBrowserUI, the value will be overridden by the FPSCursorRenderer.
         */
        [HideInInspector]
        public float maxDistance = float.PositiveInfinity;

        /** Fills up with key events as they happen. */
        protected List<Event> keyEvents = new List<Event>();

        /** Swaps with keyEvents on InputUpdate and is returned in the main getter. */
        protected List<Event> keyEventsLast = new List<Event>();

        /** Returns the user's interacting ray, usually the mouse pointer in some form. */
        protected virtual Ray LookRay
        {
            get
            {
                if (vrCursorObject != null)
                {
                    Ray raycast = new Ray(vrCursorObject.transform.position, vrCursorObject.transform.forward);
                    return raycast;
                }
                else
                {
                    return Camera.main.ScreenPointToRay(Input.mousePosition);
                }
            }
        }

        /** List of keys Unity won't give us events for. So we have to poll. */
        static readonly KeyCode[] keysToCheck = {
#if ZF_OSX
		//On windows you get GUI events for ctrl, super, alt. On mac...you don't!
		KeyCode.LeftCommand,
		KeyCode.RightCommand,
		KeyCode.LeftControl,
		KeyCode.RightControl,
		KeyCode.LeftAlt,
		KeyCode.RightAlt,
		//KeyCode.CapsLock, unity refuses to inform us of this, so there's not much we can do
#endif
		//Unity consistently doesn't send events for shift across all platforms.
		KeyCode.LeftShift,
        KeyCode.RightShift,
    };

        public virtual void InputUpdate()
        {
            //Note: keyEvents gets filled in OnGUI as things happen. InputUpdate get called just before it's read.
            //To get the right events to the right place at the right time, swap the "double buffer" of key events.
            var tmp = keyEvents;
            keyEvents = keyEventsLast;
            keyEventsLast = tmp;
            keyEvents.Clear();


            //Trace mouse from the main camera
            var mouseRay = LookRay;
            RaycastHit hit;
            Physics.Raycast(mouseRay, out hit, maxDistance);

            if (hit.transform != meshCollider.transform)
            {
                //not looking at it.
                MousePosition = new Vector3(0, 0);
                MouseButtons = 0;
                MouseScroll = new Vector2(0, 0);

                MouseHasFocus = false;
                KeyboardHasFocus = false;

                LookOff();
                return;
            }
            LookOn();
            MouseHasFocus = true;
            KeyboardHasFocus = true;

            //convert ray hit to useful mouse position on page
            var localPoint = hit.textureCoord;
            MousePosition = localPoint;

            var buttons = (MouseButton)0;
            if (Input.GetMouseButton(0)) buttons |= MouseButton.Left;
            if (Input.GetMouseButton(1)) buttons |= MouseButton.Right;
            if (Input.GetMouseButton(2)) buttons |= MouseButton.Middle;
            MouseButtons = buttons;

            if (vrCursorObject != null)
            {
                controller = vrCursorObject.GetComponent<SteamVR_TrackedController>();
                if (controller != null)
                {
                    if (controller.triggerPressed)
                    {
                        Debug.Log("Pressed Trigger");
                        buttons |= MouseButton.Left;
                    }
                }
            }
            MouseScroll = Input.mouseScrollDelta;


            //Unity doesn't include events for some keys, so fake it by checking each frame.
            for (int i = 0; i < keysToCheck.Length; i++)
            {
                if (Input.GetKeyDown(keysToCheck[i]))
                {
                    //Prepend down, postpend up. We don't know which happened first, but pressing
                    //modifiers usually precedes other key presses and releasing tends to follow.
                    keyEventsLast.Insert(0, new Event() { type = EventType.KeyDown, keyCode = keysToCheck[i] });
                }
                else if (Input.GetKeyUp(keysToCheck[i]))
                {
                    keyEventsLast.Add(new Event() { type = EventType.KeyUp, keyCode = keysToCheck[i] });
                }
            }
        }

        public void OnGUI()
        {
            var ev = Event.current;
            if (ev.type != EventType.KeyDown && ev.type != EventType.KeyUp) return;

            //		if (ev.character != 0) Debug.Log("ev >>> " + ev.character);
            //		else if (ev.type == EventType.KeyUp) Debug.Log("ev ^^^ " + ev.keyCode);
            //		else if (ev.type == EventType.KeyDown) Debug.Log("ev vvv " + ev.keyCode);

            keyEvents.Add(new Event(ev));
        }

        protected bool mouseWasOver = false;
        protected void LookOn()
        {
            if (BrowserCursor != null)
            {
                Debug.Log("Look On");
                CursorUpdated();
            }
            mouseWasOver = true;
        }

        protected void LookOff()
        {
            if (BrowserCursor != null && mouseWasOver)
            {
                Debug.Log("Look Off");
                SetCursor(null);
            }
            mouseWasOver = false;
        }

        protected void CursorUpdated()
        {
            SetCursor(BrowserCursor);
        }

        /**
         * Sets the current mouse cursor.
         * If the cursor is null we are not looking at this browser.
         *
         * This base implementation changes the mouse cursor, but you could change an in-game reticle, etc.
         */
        protected virtual void SetCursor(BrowserCursor newCursor)
        {
            //note that HandleKeyInputBrowserCursor can change while we don't have focus.
            //In such a case, don't do anything
            if (!MouseHasFocus && newCursor != null) return;

            if (newCursor == null)
            {
                Cursor.visible = true;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            else
            {
                if (newCursor.Texture != null)
                {
                    Cursor.visible = true;
                    Cursor.SetCursor(newCursor.Texture, newCursor.Hotspot, CursorMode.Auto);
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                }
            }
        }

        public bool MouseHasFocus { get; protected set; }
        public Vector2 MousePosition { get; protected set; }
        public MouseButton MouseButtons { get; protected set; }
        public Vector2 MouseScroll { get; protected set; }
        public bool KeyboardHasFocus { get; protected set; }
        public List<Event> KeyEvents { get { return keyEventsLast; } }
        public BrowserCursor BrowserCursor { get; protected set; }
        public BrowserInputSettings InputSettings { get; protected set; }

    }

}

