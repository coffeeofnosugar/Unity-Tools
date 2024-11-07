using UnityEngine;
using UnityEngine.InputSystem;

namespace Tools.EventBus.Sample
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private int health;
        [SerializeField] private int mana;
        
        EventBinding<TestEvent> testEventBinding;
        EventBinding<PlayerEvent> playerEventBinding;

        private void OnEnable()
        {
            testEventBinding = new EventBinding<TestEvent>(HandleTestEvent);
            EventBus<TestEvent>.Register(testEventBinding);
            
            playerEventBinding = new EventBinding<PlayerEvent>(HandlePlayerEvent);
            EventBus<PlayerEvent>.Register(playerEventBinding);
        }

        private void OnDisable()
        {
            EventBus<TestEvent>.Deregister(testEventBinding);
            EventBus<PlayerEvent>.Deregister(playerEventBinding);
        }

        private void Update()
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                EventBus<TestEvent>.Raise(new TestEvent());
            }

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                EventBus<PlayerEvent>.Raise(new PlayerEvent()
                {
                    health = this.health,
                    mana = this.mana
                });
            }
        }

        void HandleTestEvent()
        {
            Debug.Log("Test event received!");
        }

        void HandlePlayerEvent(PlayerEvent playerEvent)
        {
            Debug.Log($"Player event received! Health: {playerEvent.health}, Mana: {playerEvent.mana}");
        }
    }
}