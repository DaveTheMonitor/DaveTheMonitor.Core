using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Items
{
    [Component("Core.ItemSwingTime", "SwingTime", "Item")]
    public sealed class ItemSwingTimeComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemSwingTime";
        public float Time => _time.Value;
        public float Pause => _pause.Value;
        public float ExtendedPause => _extendedPause.Value;
        public float RetractTime => _retractTime.Value;
        public bool RetractSmooth => _retractSmooth.Value;
        private float? _time;
        private float? _pause;
        private float? _extendedPause;
        private float? _retractTime;
        private bool? _retractSmooth;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _time = DeserializationHelper.GetSingleProperty(element, "Time");
            _pause = DeserializationHelper.GetSingleProperty(element, "Pause");
            _extendedPause = DeserializationHelper.GetSingleProperty(element, "ExtendedPause");
            _retractTime = DeserializationHelper.GetSingleProperty(element, "RetractTime");
            _retractSmooth = DeserializationHelper.GetBoolProperty(element, "RetractSmooth");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemSwingTimeComponent)replacement;
            if (component._time.HasValue) _time = component._time;
            if (component._pause.HasValue) _pause = component._pause;
            if (component._extendedPause.HasValue) _extendedPause = component._extendedPause;
            if (component._retractTime.HasValue) _retractTime = component._retractTime;
            if (component._retractSmooth.HasValue) _retractSmooth = component._retractSmooth;
        }

        public void ReplaceXmlData(ref ItemSwingTimeDataXML data)
        {
            if (_time.HasValue) data.Time = Time;
            if (_pause.HasValue) data.Pause = Pause;
            if (_extendedPause.HasValue) data.ExtendedPause = ExtendedPause;
            if (_retractTime.HasValue) data.RetractTime = RetractTime;
            if (_retractSmooth.HasValue) data.RetractSmooth = RetractSmooth;
        }

        public override void SetDefaults()
        {
            _time ??= 0.27f;
            _pause ??= 0;
            _extendedPause ??= 0;
            _retractTime ??= -1;
            _retractSmooth ??= false;
        }

        public static ItemSwingTimeComponent FromXML(ItemSwingTimeDataXML data)
        {
            var component = new ItemSwingTimeComponent
            {
                _time = data.Time,
                _pause = data.Pause,
                _extendedPause = data.ExtendedPause,
                _retractTime = data.RetractTime,
                _retractSmooth = data.RetractSmooth
            };

            return component;
        }
    }
}
