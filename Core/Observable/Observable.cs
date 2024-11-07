using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools
{
    [HideLabel]
    [Serializable]
    public struct Observable<T>
    {
        public Action OnValueChanged;
        public Action<T> OnValueChangedTo;
        public Action<T, T> OnValueChangedFromTo;

        [SerializeField, LabelText("@$property.Parent.NiceName"), HideInPlayMode]
        private T _value;
        [ShowInInspector, LabelText("@$property.Parent.NiceName"), HideInEditorMode]
        public T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(value, _value))
                {
                    var prev = _value;
                    _value = value;
                    OnValueChanged?.Invoke();
                    OnValueChangedTo?.Invoke(_value);
                    OnValueChangedFromTo?.Invoke(prev, _value);
                }
            }
        }
        
        public static implicit operator T(Observable<T> observable) => observable.Value;
        public static implicit operator Observable<T>(T value) => new Observable<T> { Value = value };
    }
}