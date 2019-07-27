using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipelinePatternPractice.Infrastructure
{
    public static class GenericPipelineExtensions
    {
        public static TOutput Step2<TInput, TOutput, TInputOuter, TOutputOuter>(this TInput inputType,
            GenericPipeline<TInputOuter, TOutputOuter> pipelineBuilder,
            Func<TInput, TOutput> step)
        {
            var pipelineStep = pipelineBuilder.GenerateStep<TInput, TOutput>();
            pipelineStep.StepAction = step;
            return default(TOutput);
        }
    }
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

        List<object> _pipelineSteps = new List<object>();

        public GenericPipeline(Func<TPipeIn, GenericPipeline<TPipeIn, TPipeOut>, TPipeOut> steps)
        {
            steps.Invoke(default(TPipeIn), this);
        }
        public GenericPipelineStep<TStepIn, TStepOut> GenerateStep<TStepIn, TStepOut>()
        {
            var pipelineStep = new GenericPipelineStep<TStepIn, TStepOut>();
            var stepIndex = _pipelineSteps.Count;

            Task.Run(() =>
            {
                IPipelineStep<TStepOut> nextPipelineStep = null;

                foreach (var input in pipelineStep.Buffer.GetConsumingEnumerable())
                {
                    bool isLastStep = stepIndex == _pipelineSteps.Count - 1;
                    TStepOut output;
                    try
                    {
                        output = pipelineStep.StepAction(input.Input);
                    }
                    catch (Exception e)
                    {
                        input.TaskCompletionSource.SetException(e);
                        continue;
                    }
                    if (isLastStep)
                    {
                        input.TaskCompletionSource.SetResult((TPipeOut)(object)output);
                    }
                    else
                    {
                        nextPipelineStep = nextPipelineStep ?? (isLastStep ? null : _pipelineSteps[stepIndex + 1] as IPipelineStep<TStepOut>);
                        nextPipelineStep.Buffer.Add(new Item<TStepOut>() { Input = output, TaskCompletionSource = input.TaskCompletionSource });
                    }
                }
            });

            _pipelineSteps.Add(pipelineStep);
            return pipelineStep;
        }

        public Task<TPipeOut> Execute(TPipeIn input)
        {
            var first = _pipelineSteps[0] as IPipelineStep<TPipeIn>;
            TaskCompletionSource<TPipeOut> tsk = new TaskCompletionSource<TPipeOut>();
            first.Buffer.Add(/*input*/new Item<TPipeIn>()
            {
                Input = input,
                TaskCompletionSource = tsk
            });
            return tsk.Task;
        }
    }
}
