using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace logging.api.test.Interface
{
    public class CustomLogger : ICustomLogger
    {
        private readonly ILogger<CustomLogger> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        public CustomLogger(ILogger<CustomLogger> logger, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        {
            _logger = logger;
            _recyclableMemoryStreamManager = recyclableMemoryStreamManager;
        }

        public async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            var responseBody = _recyclableMemoryStreamManager.GetStream();

            context.Response.Body = responseBody;

            //await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            LogHttpResponse(context, body);

            await responseBody.CopyToAsync(originalBodyStream);
        }

        public async Task LogRequest(HttpContext context)
        {

            context.Request.EnableBuffering();

            var requestStream = _recyclableMemoryStreamManager.GetStream();

            await context.Request.Body.CopyToAsync(requestStream);

            var requestBody = ReadStreamInChunks(requestStream);

            LogHttpRequest(context, requestBody);

            context.Request.Body.Position = 0;

        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            var textWriter = new StringWriter();
            var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }

        private void LogHttpRequest(HttpContext httpContext, string body)
        {
            using (var scope = _logger.BeginScope("{@id_channel} {@http_request_path} {@http_request_remote_address} {@http_request_method}",
                httpContext.Request.Headers.Where(d => d.Key.Equals("id_channel")).Select(d => d.Value).FirstOrDefault().ToString(),
                $"{httpContext.Request.Path.Value}{httpContext.Request.QueryString}",
                httpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                httpContext.Request.Method)
                )
            {
                LogRequestHeader(httpContext);

                LogRequestBody(body);
            }
        }

        private void LogRequestHeader(HttpContext httpcontext)
        {
            using (var scope = _logger.BeginScope("{@log_type}", "REQUEST_HEADER"))
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                foreach (var header in httpcontext.Request.Headers)
                {
                    dic.Add(header.Key, header.Value);
                }

                var message_json = _logger.BeginScope("{@messagejson_authorization}",
                    $"{dic.Where(c => c.Key.Equals("Authorization")).Select(c => c.Value).FirstOrDefault()}");

                _logger.LogInformation($"{JsonConvert.SerializeObject(dic)}");

            }

        }

        private void LogRequestBody(string body)
        {
            using (var scope = _logger.BeginScope("{@log_type}", "REQUEST_BODY"))
            {
                _logger.LogInformation($"{body}");
            }
        }

        private void LogHttpResponse(HttpContext httpContext, string body)
        {
            using (var scope = _logger.BeginScope("{@id_channel} {@http_request_path} {@http_request_remote_address} {@http_request_method} @{http_request_status_code}" +
                "@{http_request_status_phrase}",
                httpContext.Request.Headers.Where(d => d.Key.Equals("id_channel")).Select(d => d.Value).FirstOrDefault().ToString(),
                $"{httpContext.Request.Path.Value}{httpContext.Request.QueryString}",
                httpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                httpContext.Request.Method,
                httpContext.Response.StatusCode,
                ReasonPhrases.GetReasonPhrase(httpContext.Response.StatusCode)
                ))
            {
                LogResponseHeader(httpContext);

                LogResponseBody(body);
            }
        }

        private void LogResponseHeader(HttpContext httpcontext)
        {
            using (var scope = _logger.BeginScope("{@log_type}", "RESPONSE_HEADER"))
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                foreach (var header in httpcontext.Response.Headers)
                {
                    dic.Add(header.Key, header.Value);
                }

                _logger.LogInformation($"{JsonConvert.SerializeObject(dic)}");
            }
        }

        private void LogResponseBody(string body)
        {
            using (var scope = _logger.BeginScope("{@log_type}", "RESPONSE_BODY"))
            {
                _logger.LogInformation($"{body}");
            }
        }
    }
}
