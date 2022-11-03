using System;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Represebts the current vital statistics of some game entity.
    /// </summary>
    public class Health : MonoBehaviour
    {
        /// <summary>
        /// The maximum hit points for the entity.
        /// </summary>
        public int maxHP = 1;

        /// <summary>
        /// Indicates if the entity should be considered 'alive'.
        /// </summary>
        public bool IsAlive => currentHP > 0;

        int currentHP;

        [SerializeField] private float _invincibilityDuration = 0.25f;
        private float _remainingInvincibilityDuration = 0;

        [SerializeField] private HealthBar _healthBar = null;

        private void Start()
        {
            UpdateHealthBar();
        }

        private void Update()
        {
            if (_remainingInvincibilityDuration > 0)
            {
                _remainingInvincibilityDuration -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Increment the HP of the entity.
        /// </summary>
        public void Increment()
        {
            currentHP = Mathf.Clamp(currentHP + 1, 0, maxHP);

            UpdateHealthBar();
        }

        /// <summary>
        /// Decrement the HP of the entity. Will trigger a HealthIsZero event when
        /// current HP reaches 0.
        /// </summary>
        public void Decrement()
        {
            //if (_remainingInvincibilityDuration > 0)
            //{ return; }

            currentHP = Mathf.Clamp(currentHP - 1, 0, maxHP);
            if (currentHP == 0)
            {
                var ev = Schedule<HealthIsZero>();
                ev.health = this;
            }

            _remainingInvincibilityDuration = _invincibilityDuration;
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            if (_healthBar)
            { _healthBar.HealthPercentage = ((float)currentHP) / maxHP; }
        }

        /// <summary>
        /// Decrement the HP of the entitiy until HP reaches 0.
        /// </summary>
        public void Die()
        {
            while (currentHP > 0)
            {
                _remainingInvincibilityDuration = -1;
                Decrement(); 
            }
        }

        public void HealToMax()
        {
            while (currentHP < maxHP)
            { Increment(); }
        }

        public void LoseHealth()
        {
            Decrement();
        }

        void Awake()
        {
            currentHP = maxHP;
        }
    }
}
