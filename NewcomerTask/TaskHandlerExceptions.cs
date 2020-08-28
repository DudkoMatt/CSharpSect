using System;
using System.Runtime.Serialization;

namespace NewcomerTask
{
    namespace TaskHandlerExceptions
    {
        [Serializable]
        public class TaskAlreadyExistsException : Exception
        {
            public TaskAlreadyExistsException(string message) 
                : base(message) 
            { }
            
            protected TaskAlreadyExistsException(SerializationInfo info, StreamingContext ctx) 
                : base(info, ctx)
            { }
        }
        
        [Serializable]
        public class TaskIdNotFoundException : Exception
        {
            public TaskIdNotFoundException(string message) 
                : base(message) 
            { }
            
            protected TaskIdNotFoundException(SerializationInfo info, StreamingContext ctx) 
                : base(info, ctx)
            { }
        }
        
        [Serializable]
        public class InvalidSubTaskDeadlineException : Exception
        {
            public InvalidSubTaskDeadlineException(string message) 
                : base(message) 
            { }
            
            protected InvalidSubTaskDeadlineException(SerializationInfo info, StreamingContext ctx) 
                : base(info, ctx)
            { }
        }
        
        [Serializable]
        public class GroupAlreadyExistsException : Exception
        {
            public GroupAlreadyExistsException(string message) 
                : base(message) 
            { }
            
            protected GroupAlreadyExistsException(SerializationInfo info, StreamingContext ctx) 
                : base(info, ctx)
            { }
        }
        
        [Serializable]
        public class GroupNotFoundException : Exception
        {
            public GroupNotFoundException(string message) 
                : base(message) 
            { }
            
            protected GroupNotFoundException(SerializationInfo info, StreamingContext ctx) 
                : base(info, ctx)
            { }
        }
    }
}