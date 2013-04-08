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
        private string _calcMethod = "MWL";
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
        private int[] _offset; 
        #endregion
        
        #region Initializers & Public Methods
        public PrayTimeCs(string methodName)
        {
            _calcMethod = methodName;
            InitializeMethods();
        }
        public void InitializeMethods()
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
        #endregion
    }
}
