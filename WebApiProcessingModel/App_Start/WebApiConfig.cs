using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;

namespace WebApiProcessingModel
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // 1 --- HTTP message handlers are the first stage 
            //       in the processing pipeline after the request leaves the service host.
            config.MessageHandlers.Add(new CustomMsgHandler());

            // add custom Authorization 
            config.Filters.Add(new CustomAuthorization());

            // Web API routes
            config.MapHttpAttributeRoutes();

            // add action filter
            config.Filters.Add(new CustomActionFilter());

            // Trace Writer
            config.Services.Replace(typeof(System.Web.Http.Tracing.ITraceWriter), new TraceWriter());

        }
    }

    internal class TraceWriter : ITraceWriter
    {
        public void Trace(HttpRequestMessage request, string category, System.Web.Http.Tracing.TraceLevel level, Action<TraceRecord> traceAction)
        {
            Debug.WriteLine($"TraceWriter Begin: request: {request}, category: {category}, level: {level}");
            TraceRecord traceRecord = new TraceRecord(request, category, level);
            traceAction(traceRecord);
            Debug.WriteLine($"TraceWriter End: {traceRecord.Exception}, {traceRecord.Message}");
        }
    }

    internal class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Debug.WriteLine(actionExecutedContext.Exception.Message);
            base.OnException(actionExecutedContext);
        }

    }

    internal class CustomActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            Debug.WriteLine("CustomActionFilter:OnActionExecuting");
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            Debug.WriteLine("CustomActionFilter:OnActionExecuted");
            base.OnActionExecuted(actionExecutedContext);
        }
    }

    internal class CustomAuthorization : AuthorizeAttribute
    {
        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            // Custom authentication logic defined here...
            return true;
        }
    }

    internal class CustomMsgHandler : MessageProcessingHandler
    {
        public CustomMsgHandler()
        {
        }

        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Debug.WriteLine("'{0}' to '{1}' entered into with '{2}' headers",
                 request.Method,
                 request.RequestUri,
                 request.Headers);
            return request;
        }

        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            Debug.WriteLine("'{0}' to '{1}' completed with a '{2}' status",
                response.RequestMessage.Method,
                response.RequestMessage.RequestUri,
                response.StatusCode);
            return response;
        }
    }
}
