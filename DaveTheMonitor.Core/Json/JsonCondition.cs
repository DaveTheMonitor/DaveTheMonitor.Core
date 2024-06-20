using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A condition that can be deserialized from a <see cref="JsonElement"/>.
    /// </summary>
    public abstract class JsonCondition
    {
        private static Dictionary<string, Type> _conditionTypes = new Dictionary<string, Type>();

        /// <summary>
        /// Creates a new <see cref="JsonCondition"/> from a <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static JsonCondition FromJson(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException("JsonCondition must be an object.");
            }

            if (!element.TryGetProperty("Type", out JsonElement conditionType))
            {
                throw new InvalidOperationException("JsonCondition must specify a type.");
            }

            if (conditionType.ValueKind != JsonValueKind.String)
            {
                throw new InvalidOperationException("JsonCondition Type must be a string.");
            }

            string typeId = conditionType.GetString();
            if (!_conditionTypes.TryGetValue(typeId, out Type type))
            {
#if DEBUG
                CorePlugin.Log("Invalid JsonCondition type");
#endif
                return new FalseCondition();
            }

            JsonCondition condition = (JsonCondition)Activator.CreateInstance(type);
            condition.ReadFromJson(element);
            return condition;
        }

        /// <summary>
        /// Reads this condition from the <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The <see cref="JsonElement"/> to read.</param>
        protected virtual void ReadFromJson(JsonElement element)
        {

        }

        /// <summary>
        /// Adds <typeparamref name="T"/> as a <see cref="JsonCondition"/>. <typeparamref name="T"/> must inherit from <see cref="JsonCondition"/>.
        /// </summary>
        /// <typeparam name="T">The type to add.</typeparam>
        /// <param name="typeId">The ID of the condition.</param>
        public static void RegisterConditionType<T>(string typeId) where T : JsonCondition
        {
            if (_conditionTypes.ContainsKey(typeId))
            {
                throw new InvalidOperationException($"JsonCondition with Id {typeId} has already been added.");
            }
            _conditionTypes.Add(typeId, typeof(T));
        }

        /// <summary>
        /// Registers all <see cref="JsonCondition"/>s with the <see cref="JsonConditionAttribute"/> from the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to load the types from.</param>
        public static void RegisterConditionTypes(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                JsonConditionAttribute attribute = type.GetCustomAttribute<JsonConditionAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                if (!type.IsAssignableTo(typeof(JsonCondition)))
                {
                    throw new InvalidOperationException($"JsonCondition must inherit {typeof(JsonCondition).FullName}");
                }

                if (_conditionTypes.ContainsKey(attribute.Id))
                {
                    throw new InvalidOperationException($"JsonCondition with Id {attribute.Id} has already been added.");
                }
                _conditionTypes.Add(attribute.Id, type);
            }
        }

        /// <summary>
        /// Evaluates this condition.
        /// </summary>
        /// <param name="actor">The actor triggering this condition.</param>
        /// <returns>The result of the condition.</returns>
        public abstract bool Evaluate(ICoreActor actor);

        /// <summary>
        /// Gets a <see cref="JsonConditionOperator"/> from a property.
        /// </summary>
        /// <param name="element">The element containing the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="defaultOperator">The default operator to use if the property doesn't exist.</param>
        /// <returns>The operator specified by the property, or <paramref name="defaultOperator"/> if the property doesn't exist.</returns>
        protected JsonConditionOperator GetOperator(JsonElement element, string propertyName, JsonConditionOperator defaultOperator)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement operatorElement))
            {
                return defaultOperator;
            }

            if (operatorElement.ValueKind != JsonValueKind.String)
            {
                throw new InvalidOperationException("JsonCondition Operator must be a valid operator: ==, !=, >, >=, <, <=, %.");
            }

            return operatorElement.GetString() switch
            {
                "==" => JsonConditionOperator.Equal,
                "!=" => JsonConditionOperator.NotEqual,
                ">" => JsonConditionOperator.GreaterThan,
                ">=" => JsonConditionOperator.GreaterThanOrEqual,
                "<" => JsonConditionOperator.LessThan,
                "<=" => JsonConditionOperator.LessThanOrEqual,
                "%" => JsonConditionOperator.Modulo,
                _ => throw new InvalidOperationException("JsonCondition Operator must be a valid operator: ==, !=, >, >=, <, <=, %.")
            };
        }

        /// <summary>
        /// Compares <paramref name="left"/> with <paramref name="right"/> using <paramref name="op"/>
        /// </summary>
        /// <typeparam name="T">The type to compare.</typeparam>
        /// <param name="left">The lefthand side of the operator.</param>
        /// <param name="right">The righthand side of the operator.</param>
        /// <param name="op">The operator to use. For <see cref="JsonConditionOperator.Modulo"/>, returns true if the result == 0.</param>
        /// <returns>The result of the comparison.</returns>
        protected static bool Compare<T>(T left, T right, JsonConditionOperator op) where T : System.Numerics.INumber<T>
        {
            return op switch
            {
                JsonConditionOperator.Equal => left == right,
                JsonConditionOperator.NotEqual => left != right,
                JsonConditionOperator.GreaterThan => left > right,
                JsonConditionOperator.GreaterThanOrEqual => left > right,
                JsonConditionOperator.LessThan => left < right,
                JsonConditionOperator.LessThanOrEqual => left <= right,
                JsonConditionOperator.Modulo => left % right == T.Zero,
                _ => throw new ArgumentOutOfRangeException(nameof(op))
            };
        }
    }
}
