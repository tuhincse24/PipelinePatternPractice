using PipelinePatternPractice.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PipelinePatternPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            //GenericPipeline();
            var pipeline = CreateGenericPipeline();

            var tsk = Task.Run(async () =>
            {
                Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));
                Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));
                Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));
                Console.WriteLine(await pipeline.Execute("The pipeline patter is the best patter"));
                Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));
            });
            tsk.Wait();


            Console.Read();
        }

        private static GenericPipeline<string, bool> CreateGenericPipeline()
        {
            var pipeline = new GenericPipeline<string, bool>((inputFirst, builder) =>
                inputFirst.NextStep(builder, input => FindMostCommon(input))
                    .NextStep(builder, input => CountChars(input))
                    .NextStep(builder, input => IsOdd(input)));
            return pipeline;
        }

        private static string FindMostCommon(string input)
        {
            return input.Split(' ')
                .GroupBy(word => word)
                .OrderBy(group => group.Count())
                .Last()
                .Key;
        }

        private static int CountChars(string mostCommon)
        {
            return mostCommon.Length;
        }

        private static bool IsOdd(int number)
        {
            var res = number % 2 == 1;
            Console.WriteLine(res.ToString());
            return res;
        }
    }
}
