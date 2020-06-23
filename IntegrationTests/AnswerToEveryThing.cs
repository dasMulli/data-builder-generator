using DasMulli.DataBuilderGenerator;
using System;

namespace DataBuilderIntegrationTests
{
    [GenerateDataBuilder]
    public class AnswerToEveryThing
    {
        public DateTime CreationDate { get; set; }

        public string Author { get; set; }

        public int Value { get; set; } = 42;

        public string Question { get; set; } = null!;

        public AnswerToEveryThing(DateTime creationDate, string author)
        {
            CreationDate = creationDate;
            Author = author;
        }
    }
}
