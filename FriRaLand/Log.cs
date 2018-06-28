using log4net;
using System;
using System.Reflection;

namespace RakuLand {
    class Log {
        public static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
