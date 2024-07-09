using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Json;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DaveTheMonitor.Core.Helpers
{
    /// <summary>
    /// Contains several methods related to data deserialization.
    /// </summary>
    public static class DeserializationHelper
    {
        /// <summary>
        /// File extensions that may contain Json data.
        /// </summary>
        public static readonly string[] JsonFileExtensions = new string[]
        {
            ".json",
            ".jsonc"
        };
        public static JsonDocumentOptions DocumentOptionsTrailingCommasSkipComments { get; }
        public static JsonSerializerOptions SerializerOptionsTrailingCommasSkipComments { get; }

        static DeserializationHelper()
        {
            DocumentOptionsTrailingCommasSkipComments = new JsonDocumentOptions()
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };
            SerializerOptionsTrailingCommasSkipComments = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
        }

        #region Json

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="string"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="string"/>, otherwise null.</returns>
        public static string GetStringProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.String)
            {
                return null;
            }

            return value.GetString();
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a value in <typeparamref name="T"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a value in <typeparamref name="T"/>, otherwise null.</returns>
        public static T? GetEnumProperty<T>(JsonElement element, string propertyName) where T : struct
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.String || !Enum.TryParse(value.GetString(), true, out T result))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="bool"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="bool"/>, otherwise null.</returns>
        public static bool? GetBoolProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.True && value.ValueKind != JsonValueKind.False)
            {
                return null;
            }

            return value.GetBoolean();
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="byte"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="byte"/>, otherwise null.</returns>
        public static byte? GetByteProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            return value.TryGetByte(out byte result) ? result : null;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is an <see cref="sbyte"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is an <see cref="sbyte"/>, otherwise null.</returns>
        public static sbyte? GetSByteProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            return value.TryGetSByte(out sbyte result) ? result : null;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="short"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="short"/>, otherwise null.</returns>
        public static short? GetInt16Property(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            return value.TryGetInt16(out short result) ? result : null;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="ushort"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="ushort"/>, otherwise null.</returns>
        public static ushort? GetUInt16Property(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            return value.TryGetUInt16(out ushort result) ? result : null;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is an <see cref="int"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is an <see cref="int"/>, otherwise null.</returns>
        public static int? GetInt32Property(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            return value.TryGetInt32(out int result) ? result : null;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="uint"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="uint"/>, otherwise null.</returns>
        public static uint? GetUInt32Property(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            return value.TryGetUInt32(out uint result) ? result : null;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="long"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="long"/>, otherwise null.</returns>
        public static long? GetInt64Property(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            return value.TryGetInt64(out long result) ? result : null;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="ulong"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="ulong"/>, otherwise null.</returns>
        public static ulong? GetUInt64Property(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            return value.TryGetUInt64(out ulong  result) ? result : null;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="float"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="float"/>, otherwise null.</returns>
        public static float? GetSingleProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            return value.TryGetSingle(out float result) ? result : null;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="double"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="double"/>, otherwise null.</returns>
        public static double? GetDoubleProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            return value.TryGetDouble(out double result) ? result : null;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="decimal"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="decimal"/>, otherwise null.</returns>
        public static decimal? GetDecimalProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            return value.TryGetDecimal(out decimal result) ? result : null;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="ModVersion"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="ModVersion"/>, otherwise null.</returns>
        public static ModVersion? GetVersionProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.String || !TryParseModVersion(value.GetString(), out ModVersion version))
            {
                return null;
            }

            return version;
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="Color"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="Color"/>, otherwise null.</returns>
        public static Color? GetColorProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.Array || (value.GetArrayLength() != 3 && value.GetArrayLength() != 4))
            {
                return null;
            }

            return GetColor(value);
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="Vector2"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="Vector2"/>, otherwise null.</returns>
        public static Vector2? GetVector2Property(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() != 2)
            {
                return null;
            }

            return GetVector2(value);
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="Vector3"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="Vector3"/>, otherwise null.</returns>
        public static Vector3? GetVector3Property(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() != 3)
            {
                return null;
            }
            
            return GetVector3(value);
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="Vector4"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="Vector4"/>, otherwise null.</returns>
        public static Vector4? GetVector4Property(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() != 4)
            {
                return null;
            }

            return GetVector4(value);
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="Point"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="Point"/>, otherwise null.</returns>
        public static Point? GetPointProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() != 2)
            {
                return null;
            }

            return GetPoint(value);
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="Point3D"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="Point3D"/>, otherwise null.</returns>
        public static Point3D? GetPoint3DProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() != 3)
            {
                return null;
            }

            return GetPoint3D(value);
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="GlobalPoint3D"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="GlobalPoint3D"/>, otherwise null.</returns>
        public static GlobalPoint3D? GetGlobalPoint3DProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() != 3)
            {
                return null;
            }

            return GetGlobalPoint3D(value);
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="Rectangle"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="Rectangle"/>, otherwise null.</returns>
        public static Rectangle? GetRectangleProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() != 4)
            {
                return null;
            }

            return GetRectangle(value);
        }

        /// <summary>
        /// Gets the value of the specified property if it exists and is a <see cref="BoundingBox"/>, otherwise null.
        /// </summary>
        /// <param name="element">The parent element of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property if it exists and is a <see cref="BoundingBox"/>, otherwise null.</returns>
        public static BoundingBox? GetBoundingBoxProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            Vector3? min = GetVector3Property(value, "Min");
            Vector3? max = GetVector3Property(value, "Max");

            if (!min.HasValue || !max.HasValue)
            {
                return null;
            }

            return new BoundingBox(min.Value, max.Value);
        }

        /// <summary>
        /// Gets a <see cref="Color"/> from a Json element array. The array is expected to contain 3 <see cref="byte"/>s.
        /// </summary>
        /// <param name="element">The element to convert to a color.</param>
        /// <returns>A <see cref="Color"/> from the element.</returns>
        public static Color GetColor(JsonElement element)
        {
            return new Color(element[0].GetByte(), element[1].GetByte(), element[2].GetByte(), element.GetArrayLength() >= 4 ? element[3].GetByte() : 255);
        }

        /// <summary>
        /// Gets a <see cref="Vector2"/> from a Json element array. The array is expected to contain 2 <see cref="float"/>s.
        /// </summary>
        /// <param name="element">The element to convert to a color.</param>
        /// <returns>A <see cref="Vector2"/> from the element.</returns>
        public static Vector2 GetVector2(JsonElement element)
        {
            return new Vector2(element[0].GetSingle(), element[1].GetSingle());
        }

        /// <summary>
        /// Gets a <see cref="Vector3"/> from a Json element array. The array is expected to contain 3 <see cref="float"/>s.
        /// </summary>
        /// <param name="element">The element to convert to a color.</param>
        /// <returns>A <see cref="Vector3"/> from the element.</returns>
        public static Vector3 GetVector3(JsonElement element)
        {
            return new Vector3(element[0].GetSingle(), element[1].GetSingle(), element[2].GetSingle());
        }

        /// <summary>
        /// Gets a <see cref="Vector4"/> from a Json element array. The array is expected to contain 4 <see cref="float"/>s.
        /// </summary>
        /// <param name="element">The element to convert to a color.</param>
        /// <returns>A <see cref="Vector4"/> from the element.</returns>
        public static Vector4 GetVector4(JsonElement element)
        {
            return new Vector4(element[0].GetSingle(), element[1].GetSingle(), element[2].GetSingle(), element[3].GetSingle());
        }

        /// <summary>
        /// Gets a <see cref="Point"/> from a Json element array. The array is expected to contain 2 <see cref="int"/>s.
        /// </summary>
        /// <param name="element">The element to convert to a color.</param>
        /// <returns>A <see cref="Point"/> from the element.</returns>
        public static Point GetPoint(JsonElement element)
        {
            return new Point(element[0].GetInt32(), element[1].GetInt32());
        }

        /// <summary>
        /// Gets a <see cref="Point3D"/> from a Json element array. The array is expected to contain 3 <see cref="int"/>s.
        /// </summary>
        /// <param name="element">The element to convert to a color.</param>
        /// <returns>A <see cref="Point3D"/> from the element.</returns>
        public static Point3D GetPoint3D(JsonElement element)
        {
            return new Point3D(element[0].GetInt32(), element[1].GetInt32(), element[2].GetInt32());
        }

        /// <summary>
        /// Gets a <see cref="GlobalPoint3D"/> from a Json element array. The array is expected to contain 3 <see cref="int"/>s.
        /// </summary>
        /// <param name="element">The element to convert to a color.</param>
        /// <returns>A <see cref="GlobalPoint3D"/> from the element.</returns>
        public static GlobalPoint3D GetGlobalPoint3D(JsonElement element)
        {
            return new GlobalPoint3D(element[0].GetInt32(), element[1].GetInt32(), element[2].GetInt32());
        }

        /// <summary>
        /// Gets a <see cref="Rectangle"/> from a Json element array. The array is expected to contain 4 <see cref="int"/>s.
        /// </summary>
        /// <param name="element">The element to convert to a color.</param>
        /// <returns>A <see cref="Rectangle"/> from the element.</returns>
        public static Rectangle GetRectangle(JsonElement element)
        {
            return new Rectangle(element[0].GetInt32(), element[1].GetInt32(), element[2].GetInt32(), element[3].GetInt32());
        }

        #endregion

        #region TryParse

        public static bool TryParseModVersion(string str, out ModVersion version)
        {
            version = new ModVersion();
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            ModVersion value = new ModVersion();
            string[] split = str.Split('.');
            if (split.Length == 0)
            {
                return false;
            }
            if (split.Length >= 1)
            {
                if (int.TryParse(split[0], out int result))
                {
                    value.Major = result;
                }
                else
                {
                    return false;
                }
            }
            if (split.Length >= 2)
            {
                if (int.TryParse(split[1], out int result))
                {
                    value.Minor = result;
                }
                else
                {
                    return false;
                }
            }
            if (split.Length >= 3)
            {
                if (int.TryParse(split[2], out int result))
                {
                    value.Patch = result;
                }
                else
                {
                    return false;
                }
            }

            version = value;
            return true;
        }

        public static bool TryParseVector2(string s, out Vector2 result)
        {
            return TryParseVector2(s.AsSpan(), null, out result);
        }

        public static bool TryParseVector2(string s, IFormatProvider provider, out Vector2 result)
        {
            return TryParseVector2(s.AsSpan(), provider, out result);
        }

        public static bool TryParseVector2(ReadOnlySpan<char> s, out Vector2 result)
        {
            return TryParseVector2(s, null, out result);
        }

        public static bool TryParseVector2(ReadOnlySpan<char> s, IFormatProvider provider, out Vector2 result)
        {
            result = default(Vector2);

            Span<float> span = stackalloc float[2];
            if (!TryParseArray(s, provider, span, out int length) || length != 2)
            {
                return false;
            }

            result.X = span[0];
            result.Y = span[1];
            return true;
        }

        public static bool TryParseVector3(string s, out Vector3 result)
        {
            return TryParseVector3(s.AsSpan(), null, out result);
        }

        public static bool TryParseVector3(string s, IFormatProvider provider, out Vector3 result)
        {
            return TryParseVector3(s.AsSpan(), provider, out result);
        }

        public static bool TryParseVector3(ReadOnlySpan<char> s, out Vector3 result)
        {
            return TryParseVector3(s, null, out result);
        }

        public static bool TryParseVector3(ReadOnlySpan<char> s, IFormatProvider provider, out Vector3 result)
        {
            result = default(Vector3);

            Span<float> span = stackalloc float[3];
            if (!TryParseArray(s, provider, span, out int length) || length != 3)
            {
                return false;
            }

            result.X = span[0];
            result.Y = span[1];
            result.Z = span[2];
            return true;
        }

        public static bool TryParseVector4(string s, out Vector4 result)
        {
            return TryParseVector4(s.AsSpan(), null, out result);
        }

        public static bool TryParseVector4(string s, IFormatProvider provider, out Vector4 result)
        {
            return TryParseVector4(s.AsSpan(), provider, out result);
        }

        public static bool TryParseVector4(ReadOnlySpan<char> s, out Vector4 result)
        {
            return TryParseVector4(s, null, out result);
        }

        public static bool TryParseVector4(ReadOnlySpan<char> s, IFormatProvider provider, out Vector4 result)
        {
            result = default(Vector4);

            Span<float> span = stackalloc float[4];
            if (!TryParseArray(s, provider, span, out int length) || length != 4)
            {
                return false;
            }

            result.X = span[0];
            result.Y = span[1];
            result.Z = span[2];
            result.W = span[3];
            return true;
        }

        public static bool TryParsePoint(string s, out Point result)
        {
            return TryParsePoint(s.AsSpan(), null, out result);
        }

        public static bool TryParsePoint(string s, IFormatProvider provider, out Point result)
        {
            return TryParsePoint(s.AsSpan(), provider, out result);
        }

        public static bool TryParsePoint(ReadOnlySpan<char> s, out Point result)
        {
            return TryParsePoint(s, null, out result);
        }

        public static bool TryParsePoint(ReadOnlySpan<char> s, IFormatProvider provider, out Point result)
        {
            result = default(Point);

            Span<int> span = stackalloc int[2];
            if (!TryParseArray(s, provider, span, out int length) || length != 2)
            {
                return false;
            }

            result.X = span[0];
            result.Y = span[1];
            return true;
        }

        public static bool TryParsePoint3D(string s, out Point3D result)
        {
            return TryParsePoint3D(s.AsSpan(), null, out result);
        }

        public static bool TryParsePoint3D(string s, IFormatProvider provider, out Point3D result)
        {
            return TryParsePoint3D(s.AsSpan(), provider, out result);
        }

        public static bool TryParsePoint3D(ReadOnlySpan<char> s, out Point3D result)
        {
            return TryParsePoint3D(s, null, out result);
        }

        public static bool TryParsePoint3D(ReadOnlySpan<char> s, IFormatProvider provider, out Point3D result)
        {
            result = default(Point3D);

            Span<int> span = stackalloc int[3];
            if (!TryParseArray(s, provider, span, out int length) || length != 3)
            {
                return false;
            }

            result.X = span[0];
            result.Y = span[1];
            result.Z = span[2];
            return true;
        }

        public static bool TryParseGlobalPoint3D(string s, out GlobalPoint3D result)
        {
            return TryParseGlobalPoint3D(s.AsSpan(), null, out result);
        }

        public static bool TryParseGlobalPoint3D(string s, IFormatProvider provider, out GlobalPoint3D result)
        {
            return TryParseGlobalPoint3D(s.AsSpan(), provider, out result);
        }

        public static bool TryParseGlobalPoint3D(ReadOnlySpan<char> s, out GlobalPoint3D result)
        {
            return TryParseGlobalPoint3D(s, null, out result);
        }

        public static bool TryParseGlobalPoint3D(ReadOnlySpan<char> s, IFormatProvider provider, out GlobalPoint3D result)
        {
            result = default(GlobalPoint3D);

            Span<int> span = stackalloc int[3];
            if (!TryParseArray(s, provider, span, out int length) || length != 3)
            {
                return false;
            }

            result.X = span[0];
            result.Y = span[1];
            result.Z = span[2];
            return true;
        }

        public static bool TryParseArray<T>(string s, IFormatProvider provider, out T[] result) where T : ISpanParsable<T>
        {
            return TryParseArray(s.AsSpan(), provider, out result);
        }

        public static bool TryParseArray<T>(ReadOnlySpan<char> s, IFormatProvider provider, out T[] result) where T : ISpanParsable<T>
        {
            result = null;
            ReadOnlySpan<char> span = s;
            int length = 1;
            foreach (char c in s)
            {
                if (c == ',')
                {
                    length++;
                }
            }

            if (length == 1)
            {
                if (T.TryParse(s, provider, out T r))
                {
                    result = new T[] { r };
                    return true;
                }
                else
                {
                    return false;
                }
            }

            result = new T[length];
            return TryParseArray<T>(s, provider, result, length);
        }

        public static bool TryParseArray<T>(ReadOnlySpan<char> s, IFormatProvider provider, Span<T> result, out int length) where T : ISpanParsable<T>
        {
            length = 1;
            foreach (char c in s)
            {
                if (c == ',')
                {
                    length++;
                }
            }
            if (length > result.Length)
            {
                return false;
            }

            result.Clear();
            if (length == 1)
            {
                if (T.TryParse(s, provider, out T r))
                {
                    result[0] = r;
                    return true;
                }
                else
                {
                    length = 0;
                    return false;
                }
            }

            ReadOnlySpan<char> slice = s;
            for (int i = 0; i < length; i++)
            {
                int index = slice.IndexOf(',');
                if (index == -1)
                {
                    if (T.TryParse(slice, provider, out T f))
                    {
                        result[i] = f;
                    }
                    else
                    {
                        length = 0;
                        return false;
                    }
                }
                else if (T.TryParse(slice.Slice(0, index), provider, out T r))
                {
                    result[i] = r;
                    slice = slice.Slice(index + 1);
                }
                else
                {
                    length = 0;
                    return false;
                }
            }

            return true;
        }

        private static bool TryParseArray<T>(ReadOnlySpan<char> s, IFormatProvider provider, Span<T> result, int length) where T : ISpanParsable<T>
        {
            result = null;
            if (length < result.Length)
            {
                return false;
            }

            result.Clear();
            if (length == 1)
            {
                if (T.TryParse(s, provider, out T r))
                {
                    result[0] = r;
                    return true;
                }
                else
                {
                    length = 0;
                    return false;
                }
            }

            ReadOnlySpan<char> slice = s;
            for (int i = 0; i < length; i++)
            {
                int index = slice.IndexOf(',');
                if (T.TryParse(slice.Slice(0, index), provider, out T r))
                {
                    result[i] = r;
                    slice = slice.Slice(index + 1);
                }
                else
                {
                    length = 0;
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Components

        /// <summary>
        /// Creates a new <see cref="ComponentCollection"/> from a Json object.
        /// </summary>
        /// <param name="json">The Json object string to parse.</param>
        /// <param name="type">The type of component to parse.</param>
        /// <param name="options">The options to parse the string.</param>
        /// <param name="serializerOptions">The serializer options for object deserialization.</param>
        /// <returns>A new <see cref="ComponentCollection"/> from the object.</returns>
        public static ComponentCollection ReadComponents(string json, string type, JsonDocumentOptions options, JsonSerializerOptions serializerOptions)
        {
            return ReadComponents(JsonDocument.Parse(json, options), type, serializerOptions);
        }

        /// <summary>
        /// Creates a new <see cref="ComponentCollection"/> from a Json object.
        /// </summary>
        /// <param name="document">The Json document to parse. The document's root element is expected to contain a version and an object containing components.</param>
        /// <param name="type">The type of component to parse.</param>
        /// <param name="serializerOptions">The serializer options for object deserialization.</param>
        /// <returns>A new <see cref="ComponentCollection"/> from the object.</returns>
        public static ComponentCollection ReadComponents(JsonDocument document, string type, JsonSerializerOptions serializerOptions)
        {
            return ReadComponents(document.RootElement, type, serializerOptions);
        }

        /// <summary>
        /// Creates a new <see cref="ComponentCollection"/> from a Json object.
        /// </summary>
        /// <param name="element">The Json element to parse. The element is expected to contain a version and an object containing components.</param>
        /// <param name="type">The type of component to parse.</param>
        /// <param name="serializerOptions">The serializer options for object deserialization.</param>
        /// <returns>A new <see cref="ComponentCollection"/> from the object.</returns>
        public static ComponentCollection ReadComponents(JsonElement element, string type, JsonSerializerOptions serializerOptions)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidCoreJsonException("Json must be an object.");
            }

            ModVersion version = GetVersionProperty(element, "Version") ?? new ModVersion(0, 0, 0);
            if (version == new ModVersion(0, 0, 0))
            {
                throw new InvalidCoreJsonException("Version must be specified.");
            }

            if (!element.TryGetProperty("Components", out JsonElement componentsElement))
            {
                throw new InvalidCoreJsonException("Json must contain components.");
            }

            ComponentCollection components = ComponentCollection.ReadFromJson(version, componentsElement, type, serializerOptions);
            components.SetComponentDefaults();
            return components;
        }

        #endregion

        #region Files

        /// <summary>
        /// Returns an array of Json files in the specified directory using <see cref="JsonFileExtensions"/>.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <param name="searchOption">The option for searching the directory.</param>
        /// <returns>An array of Json files in the specified directory.</returns>
        public static string[] GetJsonFiles(string path, SearchOption searchOption)
        {
            string[] files = Directory.GetFiles(path, "*", searchOption);
            return files.Where(file =>
            {
                foreach (string ext in JsonFileExtensions)
                {
                    if (file.EndsWith(ext))
                    {
                        return true;
                    }
                }
                return false;
            }).ToArray();
        }

        /// <summary>
        /// Reads all Json files in the specified directory and parses them as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to parse.</typeparam>
        /// <param name="path">The directory to read.</param>
        /// <param name="searchOption">The option for searching the directory.</param>
        /// <returns>An array of <typeparamref name="T"/> from the Json files in the directory.</returns>
        public static T[] ReadAllFromPath<T>(string path, SearchOption searchOption) where T : IJsonType<T>
        {
            if (!Directory.Exists(path))
            {
                return null;
            }

            string[] files = GetJsonFiles(path, searchOption);
            List<T> list = new List<T>();
            foreach (string file in files)
            {
                T obj = T.FromJson(File.ReadAllText(file));
                list.Add(obj);
            }
            return list.ToArray();
        }

        #endregion
    }
}
