
using System;
using UnityEngine;

public static class Models 
{
    [Serializable]
    public class CameraSettings
    {
        [Header("Camera settings")]
        public float SensitivityX;
        public float SensitivityY;
        public bool InvertedX;
        public bool InvertedY;
        public float YClampMin = -60f;
        public float YClampMax = 60f;
        public float MovementSmoothTime = 0.1f;

        [Header("Character")]
        public float CharacterRotationSmoothdamp = 0.1f;
    }

    [Serializable]
    public class CharacterSettings
    {
        public float CharacterRotationSmoothdamp = 0.6f;

        [Header("Movement speeds")]
        public float WalkingSpeed = 1f;
        public float WalkingBackwardSpeed = 1f;
        public float WalkingStrafeSpeed = 1f;
        public float RunningSpeed = 1f;
        public float RunningBackwardSpeed = 1f;
        public float RunningStrafeSpeed = 1f;
        public float SprintingSpeed = 1f;
    }

    [Serializable]
    public class CharacterStats
    {
        public float Stamina;
        public float MaxStamina;
        public float StaminaDrain;
        public float StaminaRestore;
        public float MinStaminaForSprint;
        public float StaminaRestoreDelay;
        public float StaminaRestoreCurrentDelay;
    }
}
