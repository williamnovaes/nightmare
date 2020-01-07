using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nightmare
{
    public abstract class InputComponent : MonoBehaviour
    {
        public enum InputType
        {
            MouseAndKeyBoard,
            Controller,
        }

        public enum XBoxControllerButtons
        {
            None,
            A,
            B,
            X,
            Y,
            Leftstick,
            Rightstick,
            View,
            Menu,
            LeftBumper,
            RightBumper,
        }

        public enum XBoxControllerAxes
        {
            None,
            LeftstickHorizontal,
            LeftstickVertical,
            DpadHorizontal,
            DpadVertical,
            RightstickHorizontal,
            RightstickVertical,
            LeftTrigger,
            RightTrigger,
        }

        [Serializable]
        public class InputButton
        {
            public KeyCode key;
            public XBoxControllerButtons controllerButton;
            public bool Down { get; protected set; }
            public bool Held { get; protected set; }
            public bool Up { get; protected set; }
            public bool Enabled
            {
                get { return m_Enabled; }
            }

            [SerializeField]
            protected bool m_Enabled = false;
            protected bool m_GettingInput = false;

            bool m_AfterFixedUpdateDown;
            bool m_AfterFixedUpdateHeld;
            bool m_AfterFixedUpdateUp;

            protected static readonly Dictionary<int, string> k_buttonsToName = new Dictionary<int, string>
            {
                {(int)XBoxControllerButtons.A, "A"},
                {(int)XBoxControllerButtons.B, "B"},
                {(int)XBoxControllerButtons.X, "X"},
                {(int)XBoxControllerButtons.Y, "Y"},
                {(int)XBoxControllerButtons.Leftstick, "Leftstick"},
                {(int)XBoxControllerButtons.Rightstick, "Rightstick"},
                {(int)XBoxControllerButtons.View, "View"},
                {(int)XBoxControllerButtons.Menu, "Menu"},
                {(int)XBoxControllerButtons.LeftBumper, "LeftBumper"},
                {(int)XBoxControllerButtons.RightBumper, "RightBumper"},
            };

            public InputButton(KeyCode key, XBoxControllerButtons controllerButton)
            {
                this.key = key;
                this.controllerButton = controllerButton;
            }

            public void Get(bool fixedUpdateHappened, InputType inputType)
            {
                if (!m_Enabled)
                {
                    Down = false;
                    Held = false;
                    Up = false;
                    return;
                }

                if (!m_GettingInput)
                {
                    return;
                }

                if (inputType == InputType.Controller)
                {
                    if (fixedUpdateHappened)
                    {
                        Down = Input.GetButtonDown(k_buttonsToName[(int)controllerButton]);
                        Held = Input.GetButton(k_buttonsToName[(int)controllerButton]);
                        Up = Input.GetButtonUp(k_buttonsToName[(int)controllerButton]);

                        m_AfterFixedUpdateDown = Down;
                        m_AfterFixedUpdateHeld = Held;
                        m_AfterFixedUpdateUp = Up;
                    }
                    else
                    {
                        Down = Input.GetButtonDown(k_buttonsToName[(int)controllerButton]) || m_AfterFixedUpdateDown;
                        Held = Input.GetButton(k_buttonsToName[(int)controllerButton]) || m_AfterFixedUpdateHeld;
                        Up = Input.GetButtonUp(k_buttonsToName[(int)controllerButton]) || m_AfterFixedUpdateUp;

                        m_AfterFixedUpdateDown |= Down;
                        m_AfterFixedUpdateHeld |= Held;
                        m_AfterFixedUpdateUp |= Up;
                    }
                }
                else if (inputType == InputType.MouseAndKeyBoard)
                {
                    if (fixedUpdateHappened)
                    {
                        Down = Input.GetKeyDown(key);
                        Held = Input.GetKey(key);
                        Up = Input.GetKeyUp(key);

                        m_AfterFixedUpdateDown = Down;
                        m_AfterFixedUpdateHeld = Held;
                        m_AfterFixedUpdateUp = Up;
                    }
                    else
                    {
                        Down = Input.GetKeyDown(key) || m_AfterFixedUpdateDown;
                        Held = Input.GetKey(key) || m_AfterFixedUpdateHeld;
                        Up = Input.GetKeyUp(key) || m_AfterFixedUpdateUp;

                        m_AfterFixedUpdateDown |= Down;
                        m_AfterFixedUpdateHeld |= Held;
                        m_AfterFixedUpdateUp |= Up;
                    }
                }
            }

            public void Enable()
            {
                m_Enabled = true;
            }

            public void Disable()
            {
                m_Enabled = false;
            }

            public void GainControl()
            {
                m_GettingInput = true;
            }

            public IEnumerator ReleaseControl(bool resetValues)
            {
                m_GettingInput = false;

                if (!resetValues)
                {
                    yield break;
                }

                if (Down)
                {
                    Up = true;
                }
                Down = false;
                Held = false;

                m_AfterFixedUpdateDown = false;
                m_AfterFixedUpdateHeld = false;
                m_AfterFixedUpdateHeld = false;

                yield return null;

                Up = false;
            }
        }

        [Serializable]
        public class InputAxis
        {
            public KeyCode positive;
            public KeyCode negative;
            public XBoxControllerAxes controllerAxis;
            public float Value { get; protected set; }
            public bool ReceivingInput { get; protected set; }
            public bool Enabled
            {
                get { return m_Enabled; }
            }

            protected bool m_Enabled = true;
            protected bool m_GettingInput = true;

            protected static readonly Dictionary<int, string> k_AxisToName = new Dictionary<int, string>
            {
                {(int)XBoxControllerAxes.LeftstickHorizontal,"LeftstickHorizontal"},
                {(int)XBoxControllerAxes.LeftstickVertical, "LeftstickVertical"},
                {(int)XBoxControllerAxes.DpadHorizontal, "DPadHorizontal"},
                {(int)XBoxControllerAxes.DpadVertical, "DPadVertical"},
                {(int)XBoxControllerAxes.RightstickHorizontal, "RightstickHorizontal"},
                {(int)XBoxControllerAxes.RightstickVertical, "RightstickVertical"},
                {(int)XBoxControllerAxes.LeftTrigger, "LeftTrigger"},
                {(int)XBoxControllerAxes.RightTrigger, "RightTrigger"},
            };

            public InputAxis(KeyCode positive, KeyCode negative, XBoxControllerAxes controllerAxis)
            {
                this.positive = positive;
                this.negative = negative;
                this.controllerAxis = controllerAxis;
            }

            public void Get(InputType inputType)
            {
                if (!m_Enabled)
                {
                    Value = 0f;
                    return;
                }

                if (m_GettingInput)
                {
                    return;
                }

                bool positiveHeld = false;
                bool negativeHeld = false;

                if (inputType == InputType.Controller)
                {
                    float value = Input.GetAxisRaw(k_AxisToName[(int)controllerAxis]);
                    positiveHeld = value > Single.Epsilon;
                    negativeHeld = value < -Single.Epsilon;
                }
                else if (inputType == InputType.MouseAndKeyBoard)
                {
                    positiveHeld = Input.GetKey(positive);
                    negativeHeld = Input.GetKey(negative);
                }

                if (positiveHeld == negativeHeld)
                {
                    Value = 0f;
                }
                else if (positiveHeld)
                {
                    Value = 1f;
                }
                else
                {
                    Value = -1f;
                }

                ReceivingInput = positiveHeld || negativeHeld;
            }

            public void Enable()
            {
                m_Enabled = true;
            }

            public void Disable()
            {
                m_Enabled = false;
            }

            public void GainControl()
            {
                m_GettingInput = true;
            }

            public void ReleaseControl(bool resetValues)
            {
                m_GettingInput = false;
                if (resetValues)
                {
                    Value = 0f;
                    ReceivingInput = false;
                }
            }
        }

        public InputType inputType = InputType.MouseAndKeyBoard;

        bool m_FixedUpdateHappened;

        void Update()
        {
            GetInputs(m_FixedUpdateHappened || Mathf.Approximately(Time.timeScale, 0));

            m_FixedUpdateHappened = false;
        }

        void FixedUpdate()
        {
            m_FixedUpdateHappened = true;
        }

        protected abstract void GetInputs(bool fixedUpdateHappened);

        public abstract void GainControl();

        public abstract void ReleaseControl(bool resetValues = true);

        protected void GainControl(InputButton inputButton)
        {
            inputButton.GainControl();
        }

        protected void GainControl(InputAxis inputAxis)
        {
            inputAxis.GainControl();
        }

        protected void ReleaseControl(InputButton inputButton, bool resetValues)
        {
            StartCoroutine(inputButton.ReleaseControl(resetValues));
        }

        protected void ReleaseControl(InputAxis inputAxis, bool resetValues)
        {
            inputAxis.ReleaseControl(resetValues);
        }
    }
}