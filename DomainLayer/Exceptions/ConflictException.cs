using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace DomainLayer.Exceptions
{
    public class ConflictException : BaseHttpException
    {
        private readonly static int statusCode = StatusCodes.Status409Conflict;

        public ConflictException(object customError) : base(customError, statusCode)
        {
        }

        public ConflictException(IEnumerable<ValidationError> errors) : base(errors, statusCode)
        {
        }

        public ConflictException(Exception ex) : base(ex, statusCode)
        {
        }

        public ConflictException(string message = "Entity conflict", string errorCode = null, string refLink = null) : base(message, statusCode, errorCode, refLink)
        {
        }
    }
}
