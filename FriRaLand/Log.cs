using log4net;
using System;
using System.Reflection;

namespace FriRaLand {
    class Log {
        public static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
