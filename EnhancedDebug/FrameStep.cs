using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedDebug
{
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.FixedUpdate))]
    internal class SkipPlayerInputWhileSuspended
    {
        private static bool Prefix()
        {
            // Returning false causes the original method to be skipped
            return !FrameStepController.Suspended;
        }
    }

    public class FrameStepController : MonoBehaviour
    {
        private Coroutine coroutine;

        private bool frameStepActive;
        public bool FrameStepActive
        {
            get => frameStepActive;
            private set
            {
                frameStepActive = value;
                if (frameStepActive)
                    StartLoop();
            }
        }

        private bool suspensionPending = false;
        private bool resumePending = false;

        public static bool Suspended { get; private set; } = false;

        private void StartLoop()
        {
            if (coroutine == null)
                coroutine = StartCoroutine(Coroutine());
        }

        private IEnumerator Coroutine()
        {
            while (FrameStepActive)
            {
                yield return new WaitForFixedUpdate();

                LateFixedUpdate();
            }

            coroutine = null;
        }

        private void Suspend()
        {
            SuspensionManager.SuspendAll();
            suspensionPending = false;
            Suspended = true;
        }

        private void Resume()
        {
            SuspensionManager.ResumeAll();
            Suspended = false;
            resumePending = false;
        }

        private void Start()
        {
            FrameStepActive = true;
        }


        private string lastInput;

        private void Update()
        {
            if (Input.GetKeyDown(Controls.StepPause))
            {
                if (Suspended)
                    resumePending = true;
                else if (DebugMenuB.DebugControlsEnabled)
                    suspensionPending = true;
            }

            // Step single frame
            if (Suspended && Input.GetKeyDown(Controls.Step))
            {
                resumePending = true;
                suspensionPending = true;
            }
        }

        private void LateFixedUpdate()
        {
            if (Suspended && resumePending)
            {
                Resume();
            }
            else if (!Suspended && suspensionPending)
            {
                CollectInputsFromFrame();
                Suspend();
            }
        }

        private void OnGUI()
        {
            if (lastInput != null)
            {
                GUI.Box(new Rect(10, 10, 400, 20), "");
                GUI.Label(new Rect(10, 10, 400, 20), lastInput);
            }
        }

        private readonly List<string> inputs = new List<string>();
        private void CollectInputsFromFrame()
        {
            inputs.Clear();

            GetButton(Core.Input.Up, "Up");
            GetButton(Core.Input.Down, "Down");
            GetButton(Core.Input.Left, "Left");
            GetButton(Core.Input.Right, "Right");

            GetButton(Core.Input.Jump, "Jump");
            GetButton(Core.Input.SoulFlame, "SoulFlame");
            GetButton(Core.Input.SpiritFlame, "SpiritFlame");
            GetButton(Core.Input.Bash, "Bash");

            GetButton(Core.Input.Glide, "Glide");
            GetButton(Core.Input.ChargeJump, "ChargeJump");
            GetButton(Core.Input.LeftShoulder, "Grenade");
            GetButton(Core.Input.RightShoulder, "Dash");

            lastInput = string.Join(" ", inputs.ToArray());
        }

        private void GetButton(Core.Input.InputButtonProcessor button, string name)
        {
            if (button.OnPressed) inputs.Add(name + "*");
            else if (button.IsPressed) inputs.Add(name);
        }
    }
}
