using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiProcessingModel.Controllers
{
    [RoutePrefix("humber.ca/api")]
    public class SampleController : ApiController
    {
        [Route("noauth")]
        [HttpGet]
        [AllowAnonymous]
        [ExceptionFilter]
        public IHttpActionResult Get()
        {
            throw new Exception();
            return Ok("Anonymous!");
        }

        [Route("auth")]
        [HttpGet]
        [CustomAuthorization]
        public IHttpActionResult GetWithAuth()
        {
            return Ok("You're authorized .... You Rock!");
        }


    }
}
