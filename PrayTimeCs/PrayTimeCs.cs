using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrayTimeCs
{
    // PrayTimeCs class
    public class PrayTimeCs
    {
        #region Constants & Structures
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
            public float? DegValue { get; set; }
            public float? MinuteValue { get; set; }
            public string DescribedValue { get; set; }
        }
        public class Method
        {
            public string Name { get; set; }
            public List<Param> Params { get; set; }
        }
        public class ParamsComparer : IEqualityComparer<Param>
        {
            public bool Equals(Param x, Param y)
            {
                return x.ParamName == y.ParamName;
            }

            public int GetHashCode(Param obj)
            {
                return obj.GetHashCode();
            }
        }
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
        private Dictionary<String, float> _offset = new Dictionary<string, float>
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
        private float _lat, _lng, _elv;
        private float _timeZone, _jDate;
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
                Param par = _settings.Where(p => p.ParamName == i.ParamName).FirstOrDefault();
                if (i.MinuteValue != null)
                {
                    par.DegValue = null;
                    par.DescribedValue = string.Empty;
                    par.MinuteValue = i.MinuteValue;
                }
                else if (i.DegValue != null)
                {
                    par.DegValue = i.DegValue;
                    par.DescribedValue = string.Empty;
                    par.MinuteValue = null;
                }
                else if (i.DescribedValue != null)
                {
                    par.DegValue = null;
                    par.DescribedValue = i.DescribedValue;
                    par.MinuteValue = null;
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
                            DegValue = 19.5f
                        },
                        new Param
                        {
                            ParamName = TimeNames.Isha,
                            DegValue = 17.5f
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
                            DegValue = 19.5f
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
                                DegValue = 17.7f
                            },
                            new Param
                            {
                                ParamName = TimeNames.Isha,
                                DegValue = 14
                            },
                            new Param
                            {
                                ParamName = TimeNames.Maghrib,
                                DegValue = 4.5f
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
            
        }

        public void Adjust(List<Param> param)
        {

        }

        public void Tune(Dictionary<String, float> offset)
        {

        }

        public Method GetMethod()
        {
            return _calcMethod;
        }

        public List<Param> GetSetting()
        {
            return _settings;
        }

        public Dictionary<String, float> GetOffset()
        {
            return _offset;
        }

        public Dictionary<String, Method> GetDefaults()
        {
            return _methods;
        }

        public List<String> GetTimes()
        {
            List<String> resultingTimes = new List<string>();
            return resultingTimes;
        }
        #endregion
    }
}
