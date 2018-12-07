using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace SVC_CustomerManagement.Utilities
{
    public class TraceExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            //Trace.TraceError(context.ExceptionContext.Exception.ToString());
            //Elmah.ErrorSignal.FromCurrentContext().Raise(new HttpException(500, "Response: /" + context.Exception.Message));
        }
    }
}