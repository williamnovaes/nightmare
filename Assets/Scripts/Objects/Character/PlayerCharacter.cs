using UnityEngine;
using System.Collections;

namespace Nightmare
{
    [RequireComponent(typeof(CharacterController2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerCharacter : MonoBehaviour
    {
        static protected PlayerCharacter s_PlayerInstance;
        static public PlayerCharacter PlayerInstance { get { return s_PlayerInstance; } }

        public InventoryController inventoryController
        {
            get { return m_inventoryController; }
        }

        public SpriteRenderer spriteRenderer;
        public Damageable damagable;
        public Damager meleeDamager;
        public Transform facingLeftBulletSpawnPoint;
        public Transform facingRightBulletSpawnPoint;
        public BulletPool bulletPool;
        public Transform cameraFollowTarget;

        public float maxSpeed = 10f;
        public float groundAcceleration = 100f;
        public float groundDeceleration = 100f;
        [Range(0f, 1f)] public float pushingSpeedProportion;

        [Range(0f, 1f)] public float airboneAccelProportion;
        [Range(0f, 1f)] public float airboneDecelProportion;

        public float gravity = 50f;
        public float jumpSpeed = 20f;
        public float jumpAbortSpeedReduction = 100f;

        [Range(k_MinHurtJumpAngle, k_MaxHurtJumpAngle)] public float hurtJumpAngle = 45f;
        public float hurtJumpSpeed = 5f;
        public float flickeringDuration = 0.1f;

        public float meleeAttackDashSpeed = 5f;
        public bool dashWhileAirbone = false;

        public RandomAudioPlayer footstepAudioPlayer;
        public RandomAudioPlayer landingAudioPlayer;
        public RandomAudioPlayer hurtAudioPlayer;
        public RandomAudioPlayer meleeAttackAudioPlayer;
        public RandomAudioPlayer rangedAttackAudioPlayer;

        public float shotsPerSecond = 1f;
        public float bulletSpeed = 5f;
        public float holdingGunTimeoutDuration = 10f;
        public bool rightBulletSpawnPointAnimated = true;

        public float cameraHorizontalFacingOffset;
        public float cameraHorizontalSpeedOffset;
        public float cameraVerticalInputOffset;
        public float maxHorizontalDeltaDampTime;
        public float maxVerticalDeltaDampTime;
        public float verticalCameraOffsetDelay;

        public bool spriteOriginallyFacesLeft;

        protected CharacterController2D m_CharacterController2D;
        protected Animator m_Animator;
        protected CapsuleCollider2D m_Capsule;
        protected Transform m_Transform;
        protected Vector2 m_MoveVector;
        protected List<Pushable> m_CurrentPushables = new List<Pushable>(4);
        protected Pushable m_CurrentPushable;
        protected float m_TanHurtJumpAngle;
        protected WaitForSeconds m_FliackeringWait;
        protected Coroutine m_FlickerCoroutine;
        protected Transform m_CurrentBulletSpawnPoint;
        protected float m_ShotSpawnGap;
        protected WaitForSeconds m_ShotSpawnWait;
        protected Coroutine m_ShootingCoroutine;
        protected float m_NextShotTime;
        protected bool m_IsFiring;
        protected float m_ShotTimer;
        protected float m_HoldingGunTimeRamaining;
        protected TileBase m_CurrentSurface;
        protected float m_CamFollowHorizontalSpeed;
        protected float m_CamFollowVerticalSpeed;
        protected float m_VerticaCameraOffsetTimer;
        protected InventoryController m_InventoryController;

        protected Checkpoint m_LastCheckpoint = null;
        protected Vector2 m_StartingPosition = Vector2.zero;
        protected bool m_StartingFacingLeft = false;

        protected bool m_InPause = false;


    }
}
