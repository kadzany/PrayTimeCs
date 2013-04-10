using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrayTimeCs
{
    public enum AsrFactor
    {
        Standard,
        Hanafi
    }
    public static class TimeNames
    {
        public static string Imsak = "Imsak";
        public static string Fajr = "Fajr";
        public static string Sunrise = "Sunrise";
        public static string Dhuhr = "Dhuhr";
        public static string Asr = "Asr";
        public static string Sunset = "Sunset";
        public static string Maghrib = "Maghrib";
        public static string Isha = "Isha";
        public static string Midnight = "Midnight";
    }
    public static class HighLatMethods
    {
        public static string NightMiddle = "NightMiddle";   // middle of night
        public static string AngleBased = "AngleBased";     // angle/60th of night
        public static string OneSeventh = "OneSeventh";     // 1/7th of night
        public static string None = "None";                 // No adjustment
    }
    public class Param
    {
        public string ParamName { get; set; }
        double? _degValue;
        public double? DegValue 
        {
            get { return _degValue;  }
            set 
            {
                _describedValue = null;
                _minuteValue = null;
                _degValue = value; 
            }
        }
        double? _minuteValue;
        public double? MinuteValue
        {
            get { return _minuteValue; }
            set
            {
                _describedValue = null;
                _degValue = null;
                _minuteValue = value;
            }
        }
        string _describedValue;
        public string DescribedValue
        {
            get { return _describedValue; }
            set
            {
                _minuteValue = null;
                _degValue = null;
                _describedValue = value;
            }
        }
    }
    public class Method
    {
        public string Name { get; set; }
        public List<Param> Params { get; set; }
    }
    public class SunPosition
    {
        public double Declination { get; set; }
        public double Equation { get; set; }
    }
    public class DMath
    {
        public double D2R(double d)
        {
            return (d * Math.PI) / 180;
        }
        public double R2D(double r) 
        { 
            return (r * 180.0) / Math.PI; 
        }
        public double Sin(double d)
        { 
            return Math.Sin(D2R(d)); 
        }
        public double Cos(double d) 
        { 
            return Math.Cos(D2R(d)); 
        }
        public double Tan(double d) 
        { 
            return Math.Tan(D2R(d)); 
        }
        public double ArcSin(double d) 
        { 
            return R2D(Math.Asin(d)); 
        }
        public double ArcCos(double d)
        { 
            return R2D(Math.Acos(d)); 
        }
        public double ArcTan(double d) 
        { 
            return R2D(Math.Atan(d)); 
        }
        public double ArcCot(double x) 
        { 
            return R2D(Math.Atan(1 / x)); 
        }
        public double ArcTan2(double y,double  x)
        { 
            return R2D(Math.Atan2(y, x)); 
        }
        public double FixAngle(double a) 
        { 
            return Fix(a, 360); 
        }
        public double FixHour(double a) 
        { 
            return Fix(a, 24); 
        }
        public double Fix(double a,double  b) {
            a = a - b * (Math.Floor(a / b));
            return (a < 0) ? a + b : a;
        }
    }
    public class PrayTimeCs
    {
        #region Constants 
        private DMath _degMath = new DMath();
        private Method _calcMethod;
        private Dictionary<String, Method> _methods;
        private List<Param> _defaultParams = new List<Param>
        {
            new Param
            {
                ParamName = TimeNames.Maghrib,
                MinuteValue = 0
            },
            new Param
            {
                ParamName = TimeNames.Midnight,
                DescribedValue = "Standard"
            }
        };
        private List<Param> _settings = new List<Param>
        {
            new Param
            {
                ParamName = TimeNames.Imsak,
                MinuteValue = 10
            },
            new Param
            {
                ParamName = TimeNames.Dhuhr,
                MinuteValue = 0
            },
            new Param
            {
                ParamName = TimeNames.Asr,
                DescribedValue = "Standard"
            },
            new Param
            {
                ParamName = "HighLats",
                DescribedValue = HighLatMethods.NightMiddle
            }
        };
        private string _timeFormat = "24h";
        private string[] _timeSuffixes = new string[] { "am", "pm" };
        private string _invalidTime = "-----";
        private int _numIterations = 1;
        private Dictionary<String, double> _offset = new Dictionary<string, double>
        {
            { TimeNames.Imsak, 0 },
            { TimeNames.Fajr, 0 },
            { TimeNames.Sunrise, 0 },
            { TimeNames.Dhuhr, 0 },
            { TimeNames.Asr, 0 },
            { TimeNames.Sunset, 0 },
            { TimeNames.Maghrib, 0 },
            { TimeNames.Isha, 0 },
            { TimeNames.Midnight, 0 }
        }; 
        private double _lat, _lng, _elv;
        private double _timeZone, _jDate;
        #endregion
        
        #region Initializers & Public Methods
        public PrayTimeCs(string methodName)
        {            
            InitializeMethods();            
            SetMethodsDefault();
            InitializeSettings(methodName);            
        }
        private void InitializeSettings(string methodName)
        {
            _calcMethod = _methods[methodName];
            foreach (var i in _calcMethod.Params)
            {
                if (_settings.Where(p => p.ParamName == i.ParamName).FirstOrDefault() == null)
                {
                    _settings.Add(i);
                }
            }
        }
        private void SetMethodsDefault()
        {
            List<Param> defParams = _defaultParams;
            foreach (var i in _methods)
            {
                i.Value.Params.AddRange(defParams.Where(p => ! i.Value.Params.Select(c => c.ParamName).Contains(p.ParamName)));
            }
        }
        private void InitializeMethods()
        {
            if (_methods == null)
            {
                _methods = new Dictionary<string, Method>();
            }
            _methods.Add("MWL",
                new Method
                {
                    Name = "Muslim World League",
                    Params = new List<Param>()
                    {
                        new Param
                        {
                            ParamName = TimeNames.Fajr,
                            DegValue = 18
                        },
                        new Param
                        {
                            ParamName = TimeNames.Isha,
                            DegValue = 17
                        }
                    }
                });
            _methods.Add("ISNA",
                new Method
                {
                    Name = "Islamic Society of North America (ISNA)",
                    Params = new List<Param>()
                    {
                        new Param
                        {
                            ParamName = TimeNames.Fajr,
                            DegValue = 15
                        },
                        new Param
                        {
                            ParamName = TimeNames.Isha,
                            DegValue = 15
                        }
                    }
                });
            _methods.Add("Egypt",
                new Method
                {
                    Name = "Egyptian General Authority of Survey",
                    Params = new List<Param>()
                    {
                        new Param
                        {
                            ParamName = TimeNames.Fajr,
                            DegValue = 19.5
                        },
                        new Param
                        {
                            ParamName = TimeNames.Isha,
                            DegValue = 17.5
                        }
                    }
                });
            _methods.Add("Makkah",
                new Method
                {
                    Name = "Umm Al-Qura University, Makkah",
                    Params = new List<Param>()
                    {
                        new Param
                        {
                            ParamName = TimeNames.Fajr,
                            DegValue = 19.5
                        },
                        new Param
                        {
                            ParamName = TimeNames.Isha,
                            MinuteValue = 90
                        }
                    }
                });
            _methods.Add("Karachi",
            new Method
            {
                Name = "University of Islamic Sciences, Karachi",
                Params = new List<Param>()
                    {
                        new Param
                        {
                            ParamName = TimeNames.Fajr,
                            DegValue = 18
                        },
                        new Param
                        {
                            ParamName = TimeNames.Isha,
                            DegValue = 18
                        }
                    }
            });
            _methods.Add("Tehran",
                new Method
                {
                    Name = "Institute of Geophysics, University of Tehran",
                    Params = new List<Param>()
                        {
                            new Param
                            {
                                ParamName = TimeNames.Fajr,
                                DegValue = 17.7
                            },
                            new Param
                            {
                                ParamName = TimeNames.Isha,
                                DegValue = 14
                            },
                            new Param
                            {
                                ParamName = TimeNames.Maghrib,
                                DegValue = 4.5
                            },
                            new Param
                            {
                                ParamName = TimeNames.Midnight,
                                DescribedValue = "Jafari"
                            }
                        }
                });
            _methods.Add("Jafari",
                    new Method
                    {
                        Name = "Shia Ithna-Ashari, Leva Institute, Qum",
                        Params = new List<Param>()
                            {
                                new Param
                                {
                                    ParamName = TimeNames.Fajr,
                                    DegValue = 16
                                },
                                new Param
                                {
                                    ParamName = TimeNames.Isha,
                                    DegValue = 14
                                },
                                new Param
                                {
                                    ParamName = TimeNames.Maghrib,
                                    DegValue = 4
                                },
                                new Param
                                {
                                    ParamName = TimeNames.Midnight,
                                    DescribedValue = "Jafari"
                                }
                            }
                    });
        }
        public void SetMethod(Method method)
        {
            _calcMethod = method;
        }
        public void Adjust(List<Param> param)
        {
            _settings = param;
        }
        public void Tune(Dictionary<String, double> offset)
        {
            _offset = offset;
        }
        public Method GetMethod()
        {
            return _calcMethod;
        }
        public List<Param> GetSetting()
        {
            return _settings;
        }
        public Dictionary<String, double> GetOffset()
        {
            return _offset;
        }
        public Dictionary<String, Method> GetDefaults()
        {
            return _methods;
        }
        public List<String> GetTimes(DateTime date, double lat, double lng, double timezone, int dst)
        {
            List<String> resultingTimes = new List<string>();
            _lat = lat;
            _lng = lng;
            //TODO: timezone functions (DST, GMTOffsett, etc is not yet implemented
            _timeZone = 1 * timezone + ((1 * dst) > 0 ? 1 : 0);
            _jDate = GetJulian(date.Year, date.Month, date.Day) - _lng / (15 * 24);
            return ComputeTimes(resultingTimes);
        }
        private List<string> ComputeTimes(List<string> resultingTimes)
        {
            Dictionary<String, double> times = new Dictionary<string, double>
            {
                { TimeNames.Imsak, 5 },
                { TimeNames.Fajr, 5 },
                { TimeNames.Sunrise, 6 },
                { TimeNames.Dhuhr, 12 },
                { TimeNames.Asr, 13 },
                { TimeNames.Sunset, 18 },
                { TimeNames.Maghrib, 18 },
                { TimeNames.Isha, 18 },
                { TimeNames.Midnight, 0 }
            }; 

            // main iterations
            for (var i = 1; i <= _numIterations; i++)
                times = ComputePrayerTimes(times);

            times = AdjustTimes(times);

            // add midnight time
            times[TimeNames.Midnight] = 
                (GetParamByName(_settings, TimeNames.Midnight).DescribedValue == "Jafari") ?
				times[TimeNames.Sunset] + TimeDiff(times[TimeNames.Sunset], times[TimeNames.Fajr]) / 2 :
                times[TimeNames.Sunset] + TimeDiff(times[TimeNames.Sunset], times[TimeNames.Sunrise]) / 2;

            times = TuneTimes(times);
            return ModifyFormats(times);
        }
        private double TimeDiff(double starTime, double endTime)
        {
            return _degMath.FixHour(endTime - starTime);
        }
        private Dictionary<string,double> TuneTimes(Dictionary<string,double> times)
        {
            Dictionary<string, double> computedTime = new Dictionary<string, double>();
            foreach (var i in times)
            {
                computedTime.Add(i.Key, i.Value + _offset[i.Key] / 60.0);
            }
            return computedTime;
        }
        private List<string> ModifyFormats(Dictionary<string,double> times)
        {
            List<string> modifiedFormat = new List<string>();            
            foreach (var i in times)
            {
                modifiedFormat.Add(GetFormattedTime(i, _timeFormat));
            }
            return modifiedFormat;
        }
        private string GetFormattedTime(KeyValuePair<string, double> time, string format)
        {
            if (time.Value == null){
                return _invalidTime;
            }

            if (format == "Float") return time.Value.ToString();

            //string suffixes = suffixes || _timeSuffixes[0];

            double timeVal = _degMath.FixHour(time.Value + 0.5 / 60.0);  // add 0.5 minutes to round
            
            double hours = Math.Floor(timeVal);
            double minutes = Math.Floor((timeVal - hours) * 60.0);
            string  suffix = (format == "12h") ? _timeSuffixes[hours < 12.0 ? 0 : 1] : "";
            string hour = (format == "24h") ? TwoDigitsFormat(hours).ToString() : ((hours + 12.0 - 1) % 12.0 + 1).ToString();
            return hour + ':' + TwoDigitsFormat(minutes).ToString() + (string.IsNullOrEmpty(suffix) ? " " + suffix : "");
        }
        private string TwoDigitsFormat(double hours)
        {
            return (hours < 10) ? "0" + hours.ToString() : hours.ToString();
        }
        private Dictionary<string,double> AdjustTimes(Dictionary<string,double> times)
        {            
            Dictionary<string,double> computedTimes = new Dictionary<string,double>();
            foreach (var i in times)
            {
                computedTimes.Add(i.Key, i.Value + _timeZone  - _lng / 15.0);
            }

            if (GetParamByName(_settings, "HighLats").DescribedValue != "None")
            {
                computedTimes = AdjustHighLats(computedTimes);
            }

            if (IsMin(GetParamByName(_settings, TimeNames.Imsak)))
                computedTimes[TimeNames.Imsak] = computedTimes[TimeNames.Fajr] - GetAngle(GetParamByName(_settings, TimeNames.Imsak)) / 60;
            if (IsMin(GetParamByName(_settings, TimeNames.Maghrib)))
                computedTimes[TimeNames.Maghrib] = computedTimes[TimeNames.Sunset] + GetAngle(GetParamByName(_settings, TimeNames.Maghrib)) / 60;
            if (IsMin(GetParamByName(_settings, TimeNames.Isha)))
                computedTimes[TimeNames.Isha] = computedTimes[TimeNames.Maghrib] + GetAngle(GetParamByName(_settings, TimeNames.Isha)) / 60;

            computedTimes[TimeNames.Dhuhr] += GetAngle(GetParamByName(_settings, TimeNames.Dhuhr)) / 60;

 	        return computedTimes;
        }
        private static Param GetParamByName(List<Param> setting, string paramName)
        {
            return setting.Where( c => c.ParamName == paramName).FirstOrDefault();
        }
        private bool IsMin(Param param)
        {
            bool isMin = false;
            if (param.MinuteValue != null)
            {
                isMin = true;
            }
            return isMin;
        }
        private Dictionary<string, double> AdjustHighLats(Dictionary<string, double> times)
        {
            double nightTime = TimeDiff(times[TimeNames.Sunset], times[TimeNames.Sunrise]);

            times[TimeNames.Imsak] = AdjustHLTime(times[TimeNames.Imsak], times[TimeNames.Sunrise], GetAngle(GetParamByName(_settings, TimeNames.Imsak)), nightTime, "CCW");
            times[TimeNames.Fajr]  = AdjustHLTime(times[TimeNames.Fajr], times[TimeNames.Sunrise], GetAngle(GetParamByName(_settings, TimeNames.Fajr)), nightTime, "CCW");
            times[TimeNames.Isha] = AdjustHLTime(times[TimeNames.Isha], times[TimeNames.Sunset], GetAngle(GetParamByName(_settings, TimeNames.Isha)), nightTime, "CW");
            times[TimeNames.Maghrib] = AdjustHLTime(times[TimeNames.Maghrib], times[TimeNames.Sunset], GetAngle(GetParamByName(_settings, TimeNames.Maghrib)), nightTime, "CW");
            
            return times;
        }
        private double AdjustHLTime(double time,double baseTime,double angle,double nightTime,string direction)
        {
            double portion = GetNightPortion(angle, nightTime);
            double timeDiff = (direction == "CCW") ? TimeDiff(time, baseTime) : TimeDiff(baseTime, time);
            if (time == null || timeDiff > portion)
            {
                time = baseTime + (direction == "CCW" ? -portion : portion);
            }
            return time;
        }
        private double GetNightPortion(double angle, double nightTime)
        {            
            double portion = 1.0 / 2.0; // MidNight
            if (GetParamByName(_settings, "HighLats").DescribedValue == "AngleBased")
            {
                portion = 1.0 / 60.0 * angle;
            }
            if (GetParamByName(_settings, "HighLats").DescribedValue == "OneSeventh")
            {
                portion = 1.0 / 7.0;
            }
            return portion * nightTime;
        }
        private Dictionary<string,double> ComputePrayerTimes(Dictionary<string,double> times)
        {
            Dictionary<string, double> computedTimes;
            times = DayPortion(times);

            double Imsak = SunAngleTime(GetAngle(GetParamByName(_settings, TimeNames.Imsak)), times[TimeNames.Imsak], "CCW");
            double Fajr = SunAngleTime(GetAngle(GetParamByName(_settings, TimeNames.Fajr)), times[TimeNames.Fajr], "CCW");
            double Sunrise = SunAngleTime(RiseSetAngle(), times[TimeNames.Sunrise], "CCW");
            double Dhuhr = GetMidDay(times[TimeNames.Dhuhr]);
            double Asr = AsrTime(AsrFactor(GetParamByName(_settings, TimeNames.Asr).DescribedValue), times[TimeNames.Asr]);
            double Sunset = SunAngleTime(RiseSetAngle(), times[TimeNames.Sunset], "CW");
            double Maghrib = SunAngleTime(GetAngle(GetParamByName(_settings, TimeNames.Maghrib)), times[TimeNames.Maghrib], "CW");
            double Isha = SunAngleTime(GetAngle(GetParamByName(_settings, TimeNames.Isha)), times[TimeNames.Isha], "CW");

            return computedTimes = new Dictionary<string, double>
            {
                { TimeNames.Imsak, Imsak },
                { TimeNames.Fajr, Fajr },
                { TimeNames.Sunrise, Sunrise },
                { TimeNames.Dhuhr, Dhuhr },
                { TimeNames.Asr, Asr },
                { TimeNames.Sunset, Sunset },
                { TimeNames.Maghrib, Maghrib },
                { TimeNames.Isha, Isha }
            };
        }
        private double AsrTime(double factor, double time)
        {
            double declination = GetSunPosition(_jDate + time).Declination;
            double angle = -_degMath.ArcCot(factor + _degMath.Tan(Math.Abs(_lat - declination)));
            return SunAngleTime(angle, time, "CW");
        }
        private double AsrFactor(string asrFactor)
        {
            double afVal = 1;
            if(Enum.IsDefined(typeof(AsrFactor), asrFactor))
            {
                afVal = Convert.ToDouble(Enum.Parse(typeof(AsrFactor), asrFactor)) + 1;
            }
            return afVal;
        }
        private double RiseSetAngle()
        {
            var angle = 0.0347 * Math.Sqrt(_elv); // an approximation
            return 0.833 + angle;
        }
        private double SunAngleTime(double angle, double time, string clockDirection)
        {
            double declination = GetSunPosition(_jDate + time).Declination;
            double noon = GetMidDay(time);
            var t = 1.0 / 15.0 * _degMath.ArcCos((-_degMath.Sin(angle) - _degMath.Sin(declination) * _degMath.Sin(_lat)) /
                (_degMath.Cos(declination) * _degMath.Cos(_lat)));
            return noon + (clockDirection == "CCW" ? -t : t);
        }
        private double GetAngle(Param param)
        {
            double? angle = param.DegValue;
            if (param.DegValue == null)
            {
                angle = param.MinuteValue;
            }
            return angle.GetValueOrDefault();
        }
        private double GetMidDay(double time)
        {
            double equation = GetSunPosition(_jDate + time).Equation;
            double noon = _degMath.FixHour(12 - equation);
            return noon;  
        }
        private SunPosition GetSunPosition(double jDate)
        {
            double D = jDate - 2451545.0;
            double g = _degMath.FixAngle(357.529 + 0.98560028 * D);
            double q = _degMath.FixAngle(280.459 + 0.98564736 * D);
            double L = _degMath.FixAngle(q + 1.915 * _degMath.Sin(g) + 0.020 * _degMath.Sin(2 * g));

            double R = 1.00014 - 0.01671 * _degMath.Cos(g) - 0.00014 * _degMath.Cos(2 * g);
            double e = 23.439 - 0.00000036 * D;

            double RA = _degMath.ArcTan2(_degMath.Cos(e) * _degMath.Sin(L), _degMath.Cos(L)) / 15;
            double eqt = q / 15 - _degMath.FixHour(RA);
            double decl = _degMath.ArcSin(_degMath.Sin(e) * _degMath.Sin(L));

            return new SunPosition{ Declination = decl, Equation = eqt };
        }
        private Dictionary<string, double> DayPortion(Dictionary<string, double> times)
        {
            Dictionary<string, double> updatedTimes = new Dictionary<string, double>();
            foreach (var i in times)
            {
                updatedTimes.Add(i.Key, i.Value / 24);
            }

            return updatedTimes;
        }
        private double GetJulian(int year, int month, int day)
        {
            if (month <= 2)
            {
                year -= 1;
                month += 12;
            };
            double A = Math.Floor((double)(year / 100));
            double B = 2 - A + Math.Floor(A / 4);

            double JD = (double)(Math.Floor((double)(365.25 * (year + 4716))) + Math.Floor((double)(30.6001 * (month + 1))) + day + B - (double)1524.5);
            return JD;
        }
        #endregion
    }
}
