using System;
using System.Collections.Generic;
using System.Linq;
using Helpo.Pages.Auth;
using Raven.Client.Documents.Indexes;

namespace Helpo.Pages.Questions
{
    public class Questions_Newest : AbstractIndexCreationTask<Question, Questions_Newest.Result>
    {
        public Questions_Newest()
        {
            this.Map = questions =>
                from question in questions
                select new Result
                {
                    Id = question.Id,
                    Title = question.Title,
                    Tags = question.Tags,
                    Application = question.Application,

                    CreatedByName = LoadDocument<User>(question.CreatedByUserId).Name,
                    CreatedAt = question.CreatedAt,

                    HasAnswer = question.AnswerId != null
                };

            this.StoreAllFields(FieldStorage.Yes);
        }

        public class Result
        {
            public string Id { get; set; } = default!;
            public string Title { get; set; } = default!;
            public List<string> Tags { get; set; } = new();
            public string Application { get; set; } = default!;

            public string CreatedByName { get; set; } = default!;
            public DateTimeOffset CreatedAt { get; set; }

            public bool HasAnswer { get; set; }
        }
    }
}