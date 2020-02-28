using SERIAL_COMM.State.Enums;
using System;
using System.Runtime.Serialization;

namespace SERIAL_COMM.State
{
    public class StateException : Exception
    {
        public SMWorkflowState ExceptionState { get; }

        public StateException()
        {
        }

        public StateException(string message, SMWorkflowState state) : base(message)
        {
            ExceptionState = state;
        }

        public StateException(string message) : base(message)
        {
        }

        public StateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public StateException(string message, Exception innerException, SMWorkflowState state)
            : base(message, innerException)
        {
            ExceptionState = state;
        }

        protected StateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
