using DaveTheMonitor.Core.Animation;
using Microsoft.Xna.Framework;

namespace DaveTheMonitor.Core.Helpers
{
    /// <summary>
    /// Contains helpers for interpolating between values.
    /// </summary>
    public static class Interpolation
    {
        /// <summary>
        /// Linearly interpolates between 3 values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="pos2">The position of the second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="amount">The amount to interpolate.</param>
        /// <returns>The resulting value.</returns>
        public static Color Lerp(Color value1, Color value2, float pos2, Color value3, float amount)
        {
            if (amount < pos2)
                return Color.Lerp(value1, value2, amount / pos2);
            else
                return Color.Lerp(value2, value3, (amount - pos2) / (1 - pos2));
        }

        /// <summary>
        /// Linearly interpolates between 4 values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="pos2">The position of the second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="pos3">The position of the third value.</param>
        /// <param name="value4">The fourth value.</param>
        /// <param name="amount">The amount to interpolate.</param>
        /// <returns>The resulting value.</returns>
        public static Color Lerp(Color value1, Color value2, float pos2, Color value3, float pos3, Color value4, float amount)
        {
            if (amount < pos2)
                return Color.Lerp(value1, value2, amount / pos2);
            else if (amount < pos3)
                return Color.Lerp(value2, value3, (amount - pos2) / (pos3 - pos2));
            else
                return Color.Lerp(value3, value4, (amount - pos3) / (1 - pos3));
        }

        /// <summary>
        /// Linearly interpolates between 2 keyframes.
        /// </summary>
        /// <param name="keyframes">The keyframes to interpolate.</param>
        /// <param name="time">The current time in the animation.</param>
        /// <returns>The interpolated value.</returns>
        public static Color Lerp(KeyframeCollection<Color> keyframes, float time)
        {
            keyframes.GetKeyframes(time, out Keyframe<Color> keyframe1, out Keyframe<Color> keyframe2);
            float amount = (time - keyframe1.Time) / (keyframe2.Time - keyframe1.Time);
            return Color.Lerp(keyframe1.Value, keyframe2.Value, amount);
        }

        /// <summary>
        /// Linearly interpolates between 3 values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="pos2">The position of the second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="amount">The amount to interpolate.</param>
        /// <returns>The resulting value.</returns>
        public static Vector2 Lerp(Vector2 value1, Vector2 value2, float pos2, Vector2 value3, float amount)
        {
            if (amount < pos2)
                return Vector2.Lerp(value1, value2, amount / pos2);
            else
                return Vector2.Lerp(value2, value3, (amount - pos2) / (1 - pos2));
        }

        /// <summary>
        /// Linearly interpolates between 4 values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="pos2">The position of the second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="pos3">The position of the third value.</param>
        /// <param name="value4">The fourth value.</param>
        /// <param name="amount">The amount to interpolate.</param>
        /// <returns>The resulting value.</returns>
        public static Vector2 Lerp(Vector2 value1, Vector2 value2, float pos2, Vector2 value3, float pos3, Vector2 value4, float amount)
        {
            if (amount < pos2)
                return Vector2.Lerp(value1, value2, amount / pos2);
            else if (amount < pos3)
                return Vector2.Lerp(value2, value3, (amount - pos2) / (pos3 - pos2));
            else
                return Vector2.Lerp(value3, value4, (amount - pos3) / (1 - pos3));
        }

        /// <summary>
        /// Linearly interpolates between 2 keyframes.
        /// </summary>
        /// <param name="keyframes">The keyframes to interpolate.</param>
        /// <param name="time">The current time in the animation.</param>
        /// <returns>The interpolated value.</returns>
        public static Vector2 Lerp(KeyframeCollection<Vector2> keyframes, float time)
        {
            keyframes.GetKeyframes(time, out Keyframe<Vector2> keyframe1, out Keyframe<Vector2> keyframe2);
            float amount = (time - keyframe1.Time) / (keyframe2.Time - keyframe1.Time);
            return Vector2.Lerp(keyframe1.Value, keyframe2.Value, amount);
        }

        /// <summary>
        /// Linearly interpolates between 3 values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="pos2">The position of the second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="amount">The amount to interpolate.</param>
        /// <returns>The resulting value.</returns>
        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float pos2, Vector3 value3, float amount)
        {
            if (amount < pos2)
                return Vector3.Lerp(value1, value2, amount / pos2);
            else
                return Vector3.Lerp(value2, value3, (amount - pos2) / (1 - pos2));
        }

        /// <summary>
        /// Linearly interpolates between 4 values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="pos2">The position of the second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="pos3">The position of the third value.</param>
        /// <param name="value4">The fourth value.</param>
        /// <param name="amount">The amount to interpolate.</param>
        /// <returns>The resulting value.</returns>
        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float pos2, Vector3 value3, float pos3, Vector3 value4, float amount)
        {
            if (amount < pos2)
                return Vector3.Lerp(value1, value2, amount / pos2);
            else if (amount < pos3)
                return Vector3.Lerp(value2, value3, (amount - pos2) / (pos3 - pos2));
            else
                return Vector3.Lerp(value3, value4, (amount - pos3) / (1 - pos3));
        }

        /// <summary>
        /// Linearly interpolates between 2 keyframes.
        /// </summary>
        /// <param name="keyframes">The keyframes to interpolate.</param>
        /// <param name="time">The current time in the animation.</param>
        /// <returns>The interpolated value.</returns>
        public static Vector3 Lerp(KeyframeCollection<Vector3> keyframes, float time)
        {
            keyframes.GetKeyframes(time, out Keyframe<Vector3> keyframe1, out Keyframe<Vector3> keyframe2);
            float amount = (time - keyframe1.Time) / (keyframe2.Time - keyframe1.Time);
            return Vector3.Lerp(keyframe1.Value, keyframe2.Value, amount);
        }

        /// <summary>
        /// Linearly interpolates between 3 values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="pos2">The position of the second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="amount">The amount to interpolate.</param>
        /// <returns>The resulting value.</returns>
        public static Vector4 Lerp(Vector4 value1, Vector4 value2, float pos2, Vector4 value3, float amount)
        {
            if (amount < pos2)
                return Vector4.Lerp(value1, value2, amount / pos2);
            else
                return Vector4.Lerp(value2, value3, (amount - pos2) / (1 - pos2));
        }

        /// <summary>
        /// Linearly interpolates between 4 values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="pos2">The position of the second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="pos3">The position of the third value.</param>
        /// <param name="value4">The fourth value.</param>
        /// <param name="amount">The amount to interpolate.</param>
        /// <returns>The resulting value.</returns>
        public static Vector4 Lerp(Vector4 value1, Vector4 value2, float pos2, Vector4 value3, float pos3, Vector4 value4, float amount)
        {
            if (amount < pos2)
                return Vector4.Lerp(value1, value2, amount / pos2);
            else if (amount < pos3)
                return Vector4.Lerp(value2, value3, (amount - pos2) / (pos3 - pos2));
            else
                return Vector4.Lerp(value3, value4, (amount - pos3) / (1 - pos3));
        }

        /// <summary>
        /// Linearly interpolates between 2 keyframes.
        /// </summary>
        /// <param name="keyframes">The keyframes to interpolate.</param>
        /// <param name="time">The current time in the animation.</param>
        /// <returns>The interpolated value.</returns>
        public static Vector4 Lerp(KeyframeCollection<Vector4> keyframes, float time)
        {
            keyframes.GetKeyframes(time, out Keyframe<Vector4> keyframe1, out Keyframe<Vector4> keyframe2);
            float amount = (time - keyframe1.Time) / (keyframe2.Time - keyframe1.Time);
            return Vector4.Lerp(keyframe1.Value, keyframe2.Value, amount);
        }

        /// <summary>
        /// Linearly interpolates between 3 values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="pos2">The position of the second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="amount">The amount to interpolate.</param>
        /// <returns>The resulting value.</returns>
        public static float Lerp(float value1, float value2, float pos2, float value3, float amount)
        {
            if (amount < pos2)
                return MathHelper.Lerp(value1, value2, amount / pos2);
            else
                return MathHelper.Lerp(value2, value3, (amount - pos2) / (1 - pos2));
        }

        /// <summary>
        /// Linearly interpolates between 4 values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="pos2">The position of the second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="pos3">The position of the third value.</param>
        /// <param name="value4">The fourth value.</param>
        /// <param name="amount">The amount to interpolate.</param>
        /// <returns>The resulting value.</returns>
        public static float Lerp(float value1, float value2, float pos2, float value3, float pos3, float value4, float amount)
        {
            if (amount < pos2)
                return MathHelper.Lerp(value1, value2, amount / pos2);
            else if (amount < pos3)
                return MathHelper.Lerp(value2, value3, (amount - pos2) / (pos3 - pos2));
            else
                return MathHelper.Lerp(value3, value4, (amount - pos3) / (1 - pos3));
        }

        /// <summary>
        /// Linearly interpolates between 2 keyframes.
        /// </summary>
        /// <param name="keyframes">The keyframes to interpolate.</param>
        /// <param name="time">The current time in the animation.</param>
        /// <returns>The interpolated value.</returns>
        public static float Lerp(KeyframeCollection<float> keyframes, float time)
        {
            keyframes.GetKeyframes(time, out Keyframe<float> keyframe1, out Keyframe<float> keyframe2);
            float amount = (time - keyframe1.Time) / (keyframe2.Time - keyframe1.Time);
            return MathHelper.Lerp(keyframe1.Value, keyframe2.Value, amount);
        }
    }
}
