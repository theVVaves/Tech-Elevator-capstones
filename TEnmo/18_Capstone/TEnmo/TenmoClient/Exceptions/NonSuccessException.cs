﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Exceptions
{
    public class NonSuccessException : Exception
    {
        private const string NON_SUCCESS_MESSAGE = "Error occurred - received non-success response: ";
        public NonSuccessException() : base() { }
        public NonSuccessException(string message) : base(message) { }
        public NonSuccessException(string message, Exception innerException) : base(message, innerException) { }
        public NonSuccessException(int statusCode) : base(NON_SUCCESS_MESSAGE + statusCode) { }
        public NonSuccessException(int statusCode, Exception innerException) : base(NON_SUCCESS_MESSAGE + statusCode, innerException) { }
    }
}
