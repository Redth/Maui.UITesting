namespace Streamer
{
    public abstract class MethodHandler
    {
        public abstract Type[] ArgumentTypes { get; }

        public abstract Type ReturnType { get; }

        public abstract string Name { get; }

        public abstract Task<object?> Handle(object[] args);
    }

    public class MethodHandler<TReturn> : MethodHandler
    {
        public MethodHandler(string name, Func<Task<TReturn>> handler)
        {
            Name = name;
            Handler = handler;
        }

        public override string Name { get; }

        protected readonly Func<Task<TReturn>> Handler;

        public override Type[] ArgumentTypes => new Type[0];

        public override Type ReturnType => typeof(TReturn);

        public override async Task<object?> Handle(object[] args)
            => await Handler();
    }

    public class MethodHandler<TReturn, TArg1> : MethodHandler
    {
        public MethodHandler(string name, Func<TArg1, Task<TReturn>> handler)
        {
            Name = name;
            Handler = handler;
        }

        public override string Name { get; }

        protected readonly Func<TArg1, Task<TReturn>> Handler;

        public override Type[] ArgumentTypes => new[] { typeof(TArg1) };

        public override Type ReturnType => typeof(TReturn);

        public override async Task<object?> Handle(object[] args)
        {
            if (args?[0] is TArg1 a1)
                return await Handler(a1);

            throw new ArgumentNullException();
        }
    }

    public class MethodHandler<TReturn, TArg1, TArg2> : MethodHandler
    {
        public MethodHandler(string name, Func<TArg1, TArg2, Task<TReturn>> handler)
        {
            Name = name;
            Handler = handler;
        }

        public override string Name { get; }

        protected readonly Func<TArg1, TArg2, Task<TReturn>> Handler;

        public override Type[] ArgumentTypes => new[] { typeof(TArg1), typeof(TArg2) };

        public override Type ReturnType => typeof(TReturn);

        public override async Task<object?> Handle(object[] args)
        {
            if (args?[0] is TArg1 a1
                && args?[1] is TArg2 a2)
                return await Handler(a1, a2);

            throw new ArgumentNullException();
        }
    }

    public class MethodHandler<TReturn, TArg1, TArg2, TArg3> : MethodHandler
    {
        public MethodHandler(string name, Func<TArg1, TArg2, TArg3, Task<TReturn?>> handler)
        {
            Name = name;
            Handler = handler;
        }

        public override string Name { get; }

        protected readonly Func<TArg1, TArg2, TArg3, Task<TReturn?>> Handler;

        public override Type[] ArgumentTypes => new[] {
            typeof(TArg1),
            typeof(TArg2),
            typeof(TArg3)
        };

        public override Type ReturnType => typeof(TReturn);

        public override async Task<object?> Handle(object[] args)
        {
            if (args?[0] is TArg1 a1
                && args?[1] is TArg2 a2
                && args?[2] is TArg3 a3)
                return await Handler(a1, a2, a3);

            throw new ArgumentNullException();
        }
    }

    public class MethodHandler<TReturn, TArg1, TArg2, TArg3, TArg4> : MethodHandler
    {
        public MethodHandler(string name, Func<TArg1, TArg2, TArg3, TArg4, Task<TReturn?>> handler)
        {
            Name = name;
            Handler = handler;
        }

        public override string Name { get; }

        protected readonly Func<TArg1, TArg2, TArg3, TArg4, Task<TReturn?>> Handler;

        public override Type[] ArgumentTypes => new[] {
            typeof(TArg1),
            typeof(TArg2),
            typeof(TArg3),
            typeof(TArg4)
        };

        public override Type ReturnType => typeof(TReturn);

        public override async Task<object?> Handle(object[] args)
        {
            if (args?[0] is TArg1 a1
                && args?[1] is TArg2 a2
                && args?[2] is TArg3 a3
                && args?[3] is TArg4 a4)
                return await Handler(a1, a2, a3, a4);

            throw new ArgumentNullException();
        }
    }

    public class MethodHandler<TReturn, TArg1, TArg2, TArg3, TArg4, TArg5> : MethodHandler
    {
        public MethodHandler(string name, Func<TArg1, TArg2, TArg3, TArg4, TArg5, Task<TReturn?>> handler)
        {
            Name = name;
            Handler = handler;
        }

        public override string Name { get; }

        protected readonly Func<TArg1, TArg2, TArg3, TArg4, TArg5, Task<TReturn?>> Handler;

        public override Type[] ArgumentTypes => new[] {
            typeof(TArg1),
            typeof(TArg2),
            typeof(TArg3),
            typeof(TArg4),
            typeof(TArg5)
        };

        public override Type ReturnType => typeof(TReturn);

        public override async Task<object?> Handle(object[] args)
        {
            if (args?[0] is TArg1 a1
                && args?[1] is TArg2 a2
                && args?[2] is TArg3 a3
                && args?[3] is TArg4 a4
                && args?[4] is TArg5 a5)
                return await Handler(a1, a2, a3, a4, a5);

            throw new ArgumentNullException();
        }
    }
}