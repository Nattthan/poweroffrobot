using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text;

namespace TransactionalBankingSimulation
{
  public class LoremIpsum
  {

    public static string GenerateLoremIpsum(int size)
    {
      string[] loremIpsumWords = {
            "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit",
            "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore",
            "magna", "aliqua", "ut", "enim", "ad", "minim", "veniam", "quis", "nostrud",
            "exercitation", "ullamco", "laboris", "nisi", "ut", "aliquip", "ex", "ea",
            "commodo", "consequat", "duis", "aute", "irure", "dolor", "in", "reprehenderit",
            "in", "voluptate", "velit", "esse", "cillum", "dolore", "eu", "fugiat", "nulla",
            "pariatur", "excepteur", "sint", "occaecat", "cupidatat", "non", "proident",
            "sunt", "in", "culpa", "qui", "officia", "deserunt", "mollit", "anim", "id", "est", "laborum"
        };

      Random random = new Random();
      StringBuilder result = new StringBuilder();
      int currentLength = 0;

      while (currentLength < size)
      {
        int sentenceLength = random.Next(5, 15); // Random sentence length between 5 and 15 words
        StringBuilder sentence = new StringBuilder();

        for (int i = 0; i < sentenceLength; i++)
        {
          if (currentLength >= size)
            break;

          string word = loremIpsumWords[random.Next(loremIpsumWords.Length)];
          sentence.Append(word);
          sentence.Append(" ");
          currentLength += word.Length + 1; // Adding 1 for the space
        }

        // Capitalize the first letter of the sentence and add a period at the end
        if (sentence.Length > 0)
        {
          sentence[0] = char.ToUpper(sentence[0]);
          sentence[sentence.Length - 1] = '.';
        }

        result.Append(sentence.ToString().Trim());
        result.Append(" ");
      }

      // Ensure the result is exactly the specified size
      return result.ToString().Substring(0, Math.Min(result.Length, size)).Trim();
    }
  }




}
