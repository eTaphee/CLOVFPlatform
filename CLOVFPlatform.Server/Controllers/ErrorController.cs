using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CLOVFPlatform.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorController : ControllerBase
    {
        public ErrorController()
        {
            
        }

        [HttpGet]
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
                    Detail = null,
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
        /// 요청 경로
        /// </summary>
        public string? Path { get; set; }
    }
}

