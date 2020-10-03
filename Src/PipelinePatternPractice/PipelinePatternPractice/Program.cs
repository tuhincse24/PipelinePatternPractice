using PipelinePatternPractice.Infrastructure;
using System;
using System.Collections.Generic;
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

            //var tsk = Task.Run(async () =>
            //{
            //    Console.WriteLine(await pipeline.Execute("The pipeline pattern1 is the best pattern1"));
            //    Console.WriteLine(await pipeline.Execute("The pipeline pattern2 is the best pattern2"));
            //    Console.WriteLine(await pipeline.Execute("The pipeline pattern3 is the best pattern3"));
            //    Console.WriteLine(await pipeline.Execute("The pipeline patter4 is the best patter4"));
            //    Console.WriteLine(await pipeline.Execute("The pipeline pattern5 is The best pattern5 pattern5 The"));
            //});

            //tsk.Wait();

            var taskList = new List<Task>();
            for(int i = 0; i < 10000; i++)
            {
                var task = pipeline.Execute($"The pipeline pattern{i} is the best patter{i}");
                taskList.Add(task);
            }
            Task.Run(async () =>
            {
                await Task.WhenAll(taskList);
            }).Wait();

            Console.Read();
        }

        private static object List<T>()
        {
            throw new NotImplementedException();
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
            var mostFrequentWord = input.Split(' ')
                .GroupBy(word => word)
                .OrderBy(group => group.Count())
                .Last()
                .Key;
            Console.WriteLine(mostFrequentWord);
            return mostFrequentWord;
        }

        private static int CountChars(string mostCommon)
        {
            Console.WriteLine(mostCommon);
            return mostCommon.Length;
        }

        private static bool IsOdd(int number)
        {
            Console.WriteLine(number.ToString());
            var res = number % 2 == 1;
            Console.WriteLine(res.ToString());
            return res;
        }
    }
}
