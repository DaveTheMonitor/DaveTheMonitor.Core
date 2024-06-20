using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// Contains extensions to help with interpolating specific types of keyframes from a <see cref="KeyframeCollection{T}"/>.
    /// </summary>
    public static class InterpolationExtensions
    {

        #region Float

        public static float Interpolate(this KeyframeCollection<float> keyframes, float time, EasingType easing)
        {
            return easing switch
            {
                EasingType.Step => StepInterpolation(keyframes, time),
                EasingType.Linear => Lerp(keyframes, time),
                _ => throw new ArgumentOutOfRangeException(nameof(easing))
            };
        }

        public static float Lerp(this KeyframeCollection<float> keyframes, float time)
        {
            keyframes.GetKeyframes(time, out Keyframe<float> left, out Keyframe<float> right);
            if (left.Time == right.Time) return left.Value;

            float amount = (time - left.Time) / (right.Time - left.Time);
            return MathHelper.Lerp(left.Value, right.Value, amount);
        }

        #endregion

        #region Vector2

        public static Vector2 Interpolate(this KeyframeCollection<Vector2> keyframes, float time, EasingType easing)
        {
            return easing switch
            {
                EasingType.Step => StepInterpolation(keyframes, time),
                EasingType.Linear => Lerp(keyframes, time),
                _ => throw new ArgumentOutOfRangeException(nameof(easing))
            };
        }

        public static Vector2 Lerp(this KeyframeCollection<Vector2> keyframes, float time)
        {
            keyframes.GetKeyframes(time, out Keyframe<Vector2> left, out Keyframe<Vector2> right);
            if (left.Time == right.Time) return left.Value;

            float amount = (time - left.Time) / (right.Time - left.Time);
            return Vector2.Lerp(left.Value, right.Value, amount);
        }

        #endregion

        #region Vector3

        public static Vector3 Interpolate(this KeyframeCollection<Vector3> keyframes, float time, EasingType easing)
        {
            return easing switch
            {
                EasingType.Step => StepInterpolation(keyframes, time),
                EasingType.Linear => Lerp(keyframes, time),
                _ => throw new ArgumentOutOfRangeException(nameof(easing))
            };
        }

        public static Vector3 Lerp(this KeyframeCollection<Vector3> keyframes, float time)
        {
            keyframes.GetKeyframes(time, out Keyframe<Vector3> left, out Keyframe<Vector3> right);
            if (left.Time == right.Time) return left.Value;

            float amount = (time - left.Time) / (right.Time - left.Time);
            return Vector3.Lerp(left.Value, right.Value, amount);
        }

        #endregion

        #region Vector4

        public static Vector4 Interpolate(this KeyframeCollection<Vector4> keyframes, float time, EasingType easing)
        {
            return easing switch
            {
                EasingType.Step => StepInterpolation(keyframes, time),
                EasingType.Linear => Lerp(keyframes, time),
                _ => throw new ArgumentOutOfRangeException(nameof(easing))
            };
        }

        public static Vector4 Lerp(this KeyframeCollection<Vector4> keyframes, float time)
        {
            keyframes.GetKeyframes(time, out Keyframe<Vector4> left, out Keyframe<Vector4> right);
            if (left.Time == right.Time) return left.Value;

            float amount = (time - left.Time) / (right.Time - left.Time);
            return Vector4.Lerp(left.Value, right.Value, amount);
        }

        #endregion

        #region Quaternion

        public static Quaternion Interpolate(this KeyframeCollection<Quaternion> keyframes, float time, EasingType easing)
        {
            return easing switch
            {
                EasingType.Step => StepInterpolation(keyframes, time),
                EasingType.Linear => Lerp(keyframes, time),
                _ => throw new ArgumentOutOfRangeException(nameof(easing))
            };
        }

        public static Quaternion Lerp(this KeyframeCollection<Quaternion> keyframes, float time)
        {
            keyframes.GetKeyframes(time, out Keyframe<Quaternion> left, out Keyframe<Quaternion> right);
            if (left.Time == right.Time) return left.Value;

            float amount = (time - left.Time) / (right.Time - left.Time);
            return Quaternion.Slerp(left.Value, right.Value, amount);
        }

        #endregion

        #region Color

        public static Color Interpolate(this KeyframeCollection<Color> keyframes, float time, EasingType easing)
        {
            return easing switch
            {
                EasingType.Step => StepInterpolation(keyframes, time),
                EasingType.Linear => Lerp(keyframes, time),
                _ => throw new ArgumentOutOfRangeException(nameof(easing))
            };
        }

        public static Color Lerp(this KeyframeCollection<Color> keyframes, float time)
        {
            keyframes.GetKeyframes(time, out Keyframe<Color> left, out Keyframe<Color> right);
            if (left.Time == right.Time) return left.Value;

            float amount = (time - left.Time) / (right.Time - left.Time);
            return Color.Lerp(left.Value, right.Value, amount);
        }

        #endregion

        private static T StepInterpolation<T>(KeyframeCollection<T> keyframes, float time)
        {
            return keyframes.GetValue(time);
        }

        public static T Interpolate<T>(this KeyframeCollection<T> keyframes, float time, Func<T, T, float, T> interpolator)
        {
            keyframes.GetKeyframes(time, out Keyframe<T> left, out Keyframe<T> right);
            if (left.Time == right.Time) return left.Value;

            float amount = (time - left.Time) / (right.Time - left.Time);
            return interpolator(left.Value, right.Value, amount);
        }
    }
}
