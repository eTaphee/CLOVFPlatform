using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CLOVFPlatform.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;

        public ErrorController(IWebHostEnvironment environment)
        {
            this.environment = environment;   
        }

        [HttpGet, HttpPost, HttpPatch, HttpPut, HttpDelete]
        [Produces("application/json")]
        public ActionResult<ErrorMessage> Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature == null)
            {
                return BadRequest(new ErrorMessage
                {
                    Status = 400,
                    Message = "Bad Request",
                    Detail = "Invalid API Access",
                    Path = HttpContext.Request.Path
                }); ;
            }

            var error = exceptionHandlerPathFeature!.Error;
            if (error is CLOVFPlatformException ex)
            {
                return StatusCode(ex.StatusCode, new ErrorMessage
                {
                    Status = ex.StatusCode,
                    Message = ex.Message,
                    Detail = ex.Detail,
                    StackTrace = environment.IsProduction() ? null : error.StackTrace,
                    Path = exceptionHandlerPathFeature.Path
                });
            }
            else
            {
                // 처리되지 않은 예외
                return StatusCode(500, new ErrorMessage
                {
                    Status = 500,
                    Message = error.GetType().Name,
                    Detail =  environment.IsProduction() ? null : error.Message,
                    StackTrace = environment.IsProduction() ? null : error.StackTrace,
                    Path = exceptionHandlerPathFeature.Path
                });
            }
        }
    }

    /// <summary>
    /// 에러 메세지
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// 상태 코드
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 메시지
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 상세 내용
        /// </summary>
        public string? Detail { get; set; }

        /// <summary>
        /// 스택 트레이스(비 프로덕션 환경)
        /// </summary>
        public string? StackTrace { get; set; }

        /// <summary>
        /// 요청 경로
        /// </summary>
        public string? Path { get; set; }
    }
}

