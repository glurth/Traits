using System.Collections.Generic;
using UnityEngine;
using EyE.Collections.UnityAssetTables;
namespace EyE.Traits
{
    [CreateAssetMenu(fileName = "AllMeasurementUnits", menuName = "GameVersionData/All Measurement Units")]
    public class AllMeasurementUnits : ImmutableTable<MeasurementUnit>, ISerializationCallbackReceiver
    {

        // Private static backing fields
        private static MeasurementUnit _percent;
        private static MeasurementUnit _kilogram;
        private static MeasurementUnit _pound;
        private static MeasurementUnit _ton;

        private static MeasurementUnit _meter;
        private static MeasurementUnit _kilometer;
        private static MeasurementUnit _centimeter;
        private static MeasurementUnit _millimeter;
        private static MeasurementUnit _mile;
        private static MeasurementUnit _foot;
        private static MeasurementUnit _inch;

        private static MeasurementUnit _liter;
        private static MeasurementUnit _milliliter;
        private static MeasurementUnit _cubicMeter;
        private static MeasurementUnit _cubicCentimeter;

        private static MeasurementUnit _second;
        private static MeasurementUnit _minute;
        private static MeasurementUnit _hour;
        private static MeasurementUnit _day;
        private static MeasurementUnit _week;
        private static MeasurementUnit _year;

        private static MeasurementUnit _kilowatt;
        private static MeasurementUnit _watt;
        private static MeasurementUnit _joule;
        private static MeasurementUnit _electronvolt;

        private static MeasurementUnit _newton;
        private static MeasurementUnit _newtonMeter;
        private static MeasurementUnit _poundFoot;

        private static MeasurementUnit _celsius;
        private static MeasurementUnit _fahrenheit;
        private static MeasurementUnit _kelvin;

        private static MeasurementUnit _meterPerSecond;
        private static MeasurementUnit _kilometerPerHour;
        private static MeasurementUnit _milesPerHour;

        private static MeasurementUnit _decibel;

        private static MeasurementUnit _volt;
        private static MeasurementUnit _ampere;
        private static MeasurementUnit _ohm;

        private static MeasurementUnit _hitPoints;
        private static MeasurementUnit _gold;
        private static MeasurementUnit _dollar;
        private static MeasurementUnit _trueFalse;

        public static MeasurementUnit Percent => _percent ??= GetByName("Percent");

        public static MeasurementUnit Kilogram => _kilogram ??= GetByName("Kilogram");
        public static MeasurementUnit Pound => _pound ??= GetByName("Pound");
        public static MeasurementUnit Ton => _ton ??= GetByName("Ton");

        public static MeasurementUnit Meter => _meter ??= GetByName("Meter");
        public static MeasurementUnit Kilometer => _kilometer ??= GetByName("Kilometer");
        public static MeasurementUnit Centimeter => _centimeter ??= GetByName("Centimeter");
        public static MeasurementUnit Millimeter => _millimeter ??= GetByName("Millimeter");
        public static MeasurementUnit Mile => _mile ??= GetByName("Mile");
        public static MeasurementUnit Foot => _foot ??= GetByName("Foot");
        public static MeasurementUnit Inch => _inch ??= GetByName("Inch");

        public static MeasurementUnit Liter => _liter ??= GetByName("Liter");
        public static MeasurementUnit Milliliter => _milliliter ??= GetByName("Milliliter");
        public static MeasurementUnit CubicMeter => _cubicMeter ??= GetByName("Cubic Meter");
        public static MeasurementUnit CubicCentimeter => _cubicCentimeter ??= GetByName("Cubic Centimeter");

        public static MeasurementUnit Second => _second ??= GetByName("Second");
        public static MeasurementUnit Minute => _minute ??= GetByName("Minute");
        public static MeasurementUnit Hour => _hour ??= GetByName("Hour");
        public static MeasurementUnit Day => _day ??= GetByName("Day");
        public static MeasurementUnit Week => _week ??= GetByName("Week");
        public static MeasurementUnit Year => _year ??= GetByName("Year");

        public static MeasurementUnit Kilowatt => _kilowatt ??= GetByName("Kilowatt");
        public static MeasurementUnit Watt => _watt ??= GetByName("Watt");
        public static MeasurementUnit Joule => _joule ??= GetByName("Joule");
        public static MeasurementUnit Electronvolt => _electronvolt ??= GetByName("Electronvolt");

        public static MeasurementUnit Newton => _newton ??= GetByName("Newton");
        public static MeasurementUnit NewtonMeter => _newtonMeter ??= GetByName("Newton-Meter");
        public static MeasurementUnit PoundFoot => _poundFoot ??= GetByName("Pound-Foot");

        public static MeasurementUnit Celsius => _celsius ??= GetByName("Celsius");
        public static MeasurementUnit Fahrenheit => _fahrenheit ??= GetByName("Fahrenheit");
        public static MeasurementUnit Kelvin => _kelvin ??= GetByName("Kelvin");

        public static MeasurementUnit MeterPerSecond => _meterPerSecond ??= GetByName("Meter per second");
        public static MeasurementUnit KilometerPerHour => _kilometerPerHour ??= GetByName("Kilometer per hour");
        public static MeasurementUnit MilesPerHour => _milesPerHour ??= GetByName("Miles per hour");

        public static MeasurementUnit Decibel => _decibel ??= GetByName("Decibel");

        public static MeasurementUnit Volt => _volt ??= GetByName("Volt");
        public static MeasurementUnit Ampere => _ampere ??= GetByName("Ampere");
        public static MeasurementUnit Ohm => _ohm ??= GetByName("Ohm");

        public static MeasurementUnit HitPoints => _hitPoints ??= GetByName("Hit Points");
        public static MeasurementUnit Gold => _gold ??= GetByName("Gold");
        public static MeasurementUnit Dollar => _dollar ??= GetByName("Dollar");
        public static MeasurementUnit TrueFalse => _trueFalse ??= GetByName("True/False");

        override public List<MeasurementUnit> GetDefaultTableElements()
        {
            return new List<MeasurementUnit>()
        {
            // Percentage
            new MeasurementUnit("Percent", "%"),

            // Mass
            new MeasurementUnit("Kilogram", "kg"),
            new MeasurementUnit("Pound", "lb"),
            new MeasurementUnit("Ton", "t"),

            // Length
            new MeasurementUnit("Meter", "m"),
            new MeasurementUnit("Kilometer", "km"),
            new MeasurementUnit("Centimeter", "cm"),
            new MeasurementUnit("Millimeter", "mm"),
            new MeasurementUnit("Mile", "mi"),
            new MeasurementUnit("Foot", "ft"),
            new MeasurementUnit("Inch", "in"),

            // Volume
            new MeasurementUnit("Liter", "L"),
            new MeasurementUnit("Milliliter", "mL"),
            new MeasurementUnit("Cubic Meter", "cu-m"),
            new MeasurementUnit("Cubic Centimeter", "cu-cm"),

            // Time
            new MeasurementUnit("Second", "s"),
            new MeasurementUnit("Minute", "min"),
            new MeasurementUnit("Hour", "hr"),
            new MeasurementUnit("Day", "d"),
            new MeasurementUnit("Week", "wk"),
            new MeasurementUnit("Year", "yr"),

            // Energy and Power
            new MeasurementUnit("Kilowatt", "kW"),
            new MeasurementUnit("Watt", "W"),
            new MeasurementUnit("Joule", "J"),

            // Force and Torque
            new MeasurementUnit("Newton", "N"),
            new MeasurementUnit("Newton-Meter", "Nm"),
            new MeasurementUnit("Pound-Foot", "lb-ft"),

            // Temperature
            new MeasurementUnit("Celsius", "°C"),
            new MeasurementUnit("Fahrenheit", "°F"),
            new MeasurementUnit("Kelvin", "K"),

            // Speed
            new MeasurementUnit("Meter per second", "m/s"),
            new MeasurementUnit("Kilometer per hour", "km/h"),
            new MeasurementUnit("Miles per hour", "mph"),

            // Sound
            new MeasurementUnit("Decibel", "dB"),

            // Electrical
            new MeasurementUnit("Volt", "V"),
            new MeasurementUnit("Ampere", "A"),
            new MeasurementUnit("Ohm", "Ω"),

            // Miscellaneous
            new MeasurementUnit("Hit Points", "HP"),
            new MeasurementUnit("Gold", "G"),
            new MeasurementUnit("Dollar", "$"),
            new MeasurementUnit("True/False", "bit")
        };
        }

    }

}