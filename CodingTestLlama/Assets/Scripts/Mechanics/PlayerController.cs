﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.Assertions;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        [Space]
        [Space]
        [Header("Player Feedback : Particles")]
        [SerializeField] private ParticleSystem _dustParticleSystem = null;
        [SerializeField] private int _nrJumpParticles = 15;
        [SerializeField] private int _nrLandParticles = 25;
        [Space]
        [SerializeField] private ParticleSystem _runningParticleSystem = null;
        private float _runParticlesEmissionTimeMultiplier = 0;
        [SerializeField] private float _rotationYWhenMovingRightRunParticles = -90;
        [SerializeField] private float _rotationYWhenMovingLeftRunParticles = 90;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            Assert.IsNotNull(_dustParticleSystem);
            Assert.IsNotNull(_runningParticleSystem);
            if (_runningParticleSystem)
            { _runParticlesEmissionTimeMultiplier = _runningParticleSystem.emission.rateOverTimeMultiplier; }
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                if (_runningParticleSystem)
                {
                    ParticleSystem.EmissionModule emissionModule = _runningParticleSystem.emission;
                    if (Mathf.Abs(move.x) > float.Epsilon && jumpState != JumpState.InFlight)
                    {
                        emissionModule.rateOverTimeMultiplier = _runParticlesEmissionTimeMultiplier;
                        Vector3 rotationEuler = _runningParticleSystem.transform.localRotation.eulerAngles;
                        float rotationYWhenRight = _rotationYWhenMovingRightRunParticles;
                        float rotationYWhenLeft = _rotationYWhenMovingLeftRunParticles;
                        _runningParticleSystem.transform.localRotation
                            = Quaternion.Euler(
                                rotationEuler.x,
                                (move.x > 0) ? rotationYWhenRight : rotationYWhenLeft,
                                rotationEuler.z);
                    }
                    else
                    {
                        emissionModule.rateOverTimeMultiplier = 0;
                    }
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    if (_dustParticleSystem)
                    { _dustParticleSystem.Emit(_nrJumpParticles); }
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                        if (_dustParticleSystem)
                        { _dustParticleSystem.Emit(_nrLandParticles); }
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}