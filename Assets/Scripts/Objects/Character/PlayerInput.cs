using UnityEngine;
using System.Collections;

namespace Nightmare
{
    public class PlayerInput : InputComponent, IDataPersister
    {
        public static PlayerInput Instance
        {
            get { return s_Instance; }
        }

        protected static PlayerInput s_Instance;

        public bool HaveControl { get { return m_HaveControl; } }

        public InputButton Pause = new InputButton(KeyCode.Escape, XBoxControllerButtons.Menu);
        public InputButton Interact = new InputButton(KeyCode.E, XBoxControllerButtons.Y);
        public InputButton MeleeAttack = new InputButton(KeyCode.K, XBoxControllerButtons.X);
        public InputButton RangedAttack = new InputButton(KeyCode.O, XBoxControllerButtons.B);
        public InputButton Jump = new InputButton(KeyCode.Space, XBoxControllerButtons.A);
        public InputAxis Horizontal = new InputAxis(KeyCode.D, KeyCode.A, XBoxControllerAxes.LeftstickHorizontal);
        public InputAxis Vertical = new InputAxis(KeyCode.W, KeyCode.S, XBoxControllerAxes.LeftstickVertical);
        [HideInInspector]
        public DataSettings dataSettings;

        protected bool m_HaveControl = true;

        protected bool m_DebugMenuIsOpen = false;

        void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
            }
            else
            {
                throw new UnityException("Não podem haver mais de uma instancia de player input. As instancias sao " + s_Instance.name + "and" + name);
            }
        }

        void OnEnable()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
            }
            else
            {
                throw new UnityException("Não podem haver mais de uma instancia de player input. As instancias sao " + s_Instance.name + "and" + name);
            }
            PersistentDataManager.RegisterPersister(this);
        }

        void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);

            s_Instance = null;
        }

        protected override void GetInputs(bool fixedUpdateHappened)
        {
            Pause.Get(fixedUpdateHappened, inputType);
            Interact.Get(fixedUpdateHappened, inputType);
            MeleeAttack.Get(fixedUpdateHappened, inputType);
            RangedAttack.Get(fixedUpdateHappened, inputType);
            Jump.Get(fixedUpdateHappened, inputType);
            Horizontal.Get(inputType);
            Vertical.Get(inputType);

            if (Input.GetKeyDown(KeyCode.F12))
            {
                m_DebugMenuIsOpen = !m_DebugMenuIsOpen;
            }
        }

        public override void GainControl()
        {
            m_HaveControl = true;

            GainControl(Pause);
            GainControl(Interact);
            GainControl(MeleeAttack);
            GainControl(RangedAttack);
            GainControl(Jump);
            GainControl(Horizontal);
            GainControl(Vertical);
        }

        public override void ReleaseControl(bool resetValues = true)
        {
            m_HaveControl = false;

            ReleaseControl(Pause, resetValues);
            ReleaseControl(Interact, resetValues);
            ReleaseControl(MeleeAttack, resetValues);
            ReleaseControl(RangedAttack, resetValues);
            ReleaseControl(Jump, resetValues);
            ReleaseControl(Horizontal, resetValues);
            ReleaseControl(Vertical, resetValues);
        }

        public void DisableMeleeAttacking()
        {
            MeleeAttack.Disable();
        }

        public void EnableMeleeAttacking()
        {
            MeleeAttack.Enable();

        }

        public void DisableRangedAttacking()
        {
            RangedAttack.Disable();
        }

        public void EnableRangedAttacking()
        {
            RangedAttack.Enable();
        }

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData()
        {
            return new Data<bool, bool>(MeleeAttack.Enabled, RangedAttack.Enabled);
        }

        public void LoadData(Data data)
        {
            Data<bool, bool> playerInputData = (Data<bool, bool>)data;
            if (playerInputData.value0)
            {
                MeleeAttack.Enable();
            }
            else
            {
                MeleeAttack.Disable();
            }

            if (playerInputData.value1)
            {
                RangedAttack.Enable();
            }
            else
            {
                RangedAttack.Disable();
            }
        }

        void OnGUI()
        {
            if (m_DebugMenuIsOpen)
            {
                const float height = 100;

                GUILayout.BeginArea(new Rect(30, Screen.height, 200, height));

                GUILayout.BeginVertical("box");
                GUILayout.Label("Press F12 to close");

                bool meleeAttackEnabled = GUILayout.Toggle(MeleeAttack.Enabled, "Enable melee attack");
                bool rangedAttackEnabled = GUILayout.Toggle(MeleeAttack.Enabled, "Enable ranged attack");

                if (meleeAttackEnabled != MeleeAttack.Enabled)
                {
                    if (meleeAttackEnabled)
                    {
                        MeleeAttack.Enable();
                    }
                    else
                    {
                        MeleeAttack.Disable();
                    }
                }

                if (rangedAttackEnabled)
                {
                    if (rangedAttackEnabled)
                    {
                        RangedAttack.Enable();
                    }
                    else
                    {
                        RangedAttack.Disable();
                    }
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
    }
}
