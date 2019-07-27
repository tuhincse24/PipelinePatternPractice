using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PipelinePatternPractice.Infrastructure
{
    public class GenericPipeline<TPipeIn, TPipeOut>
    {
        public interface IPipelineStep<TStepIn>
        {
            BlockingCollection<Item<TStepIn>> Buffer { get; set; }
        }

        public class GenericPipelineStep<TStepIn, TStepOut> : IPipelineStep<TStepIn>
        {
            public BlockingCollection<Item<TStepIn>> Buffer { get; set; } = new BlockingCollection<Item<TStepIn>>();
            public Func<TStepIn, TStepOut> StepAction { get; set; }
        }

        public class Item<T>
        {
            public T Input { get; set; }
            public TaskCompletionSource<TPipeOut> TaskCompletionSource { get; set; }
        }
    }
}
