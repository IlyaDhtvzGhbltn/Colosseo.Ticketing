using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Ticketing.Api.v1.Extensions
{
    public static class ControllerExtensions
    {
        public static async void SetStatusCode(this ControllerBase controller, int code, string message) 
        {
            controller.Response.StatusCode = code;
            await controller.Response.WriteAsync(message);
        }

        public static void SetStatusCode(this ControllerBase controller, int code)
        {
            controller.Response.StatusCode = code;
        }
    }
}
