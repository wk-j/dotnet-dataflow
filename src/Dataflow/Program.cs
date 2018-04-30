using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks.Dataflow;

namespace Dataflow {
    class Program {
        static void Main(string[] args) {
            var downloadString = new TransformBlock<string, string>(async uri => {
                Console.WriteLine("Downloading {0}", uri);
                return await new HttpClient().GetStringAsync(uri);
            });

            var createWordList = new TransformBlock<string, string[]>(text => {
                Console.WriteLine("Creating word list ...");
                char[] tokens = text.Select(c => char.IsLetter(c) ? c : ' ').ToArray();
                text = new string(tokens);
                return text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            });

            var filterWordList = new TransformBlock<string[], string[]>(words => {
                Console.WriteLine("Filtering word list");
                return words.Where(word => word.Length > 3).Distinct().ToArray();

            });

            var findReverseWorlds = new TransformManyBlock<string[], string>(words => {
                Console.WriteLine("Finding reversed word...");
                var wordsSet = new HashSet<string>(words);
                var result =
                    from word in words.AsParallel()
                    let reverse = new string(word.Reverse().ToArray())
                    where word != reverse && wordsSet.Contains(reverse)
                    select word;
                return result;

            });

            var printReversedWords = new ActionBlock<string>(reversedWord => {
                Console.WriteLine("Found revoerse words {0}/{1}", reversedWord, new String(reversedWord.Reverse().ToArray()));
            });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            downloadString.LinkTo(createWordList, linkOptions);
            createWordList.LinkTo(filterWordList, linkOptions);
            filterWordList.LinkTo(findReverseWorlds, linkOptions);
            findReverseWorlds.LinkTo(printReversedWords, linkOptions);

            downloadString.Post("http://www.gutenberg.org/files/6130/6130-0.txt");
            downloadString.Complete();
            printReversedWords.Completion.Wait();
        }
    }
}
