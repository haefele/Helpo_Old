using Helpo.Common;
using Helpo.Services.Auth;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpo.Services.Questions
{
    public class QuestionsService
    {
        private readonly IDocumentStore _documentStore;

        public QuestionsService(IDocumentStore documentStore)
        {
            Guard.NotNull(documentStore, nameof(documentStore));

            this._documentStore = documentStore;
        }

        public async Task<List<Questions_Newest.Result>> GetNewestQuestions(int page)
        {
            Guard.NotZeroOrNegative(page, nameof(page));

            const int PagingSize = 50;

            using var session = this._documentStore.OpenAsyncSession();

            return await session.Query<Questions_Newest.Result, Questions_Newest>()
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * PagingSize)
                .Take(PagingSize)
                .ProjectInto<Questions_Newest.Result>()
                .ToListAsync();
        }

        public async Task<Question> CreateNew(string title, string body, List<string> tags)
        {
            using var session = this._documentStore.OpenAsyncSession();

            var question = Question.CreateNew(title, body, null!, tags);
            await session.StoreAsync(question);

            await session.SaveChangesAsync();

            return question;
        }
    }

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
            public List<string> Tags { get; set; } = new List<string>();

            public string CreatedByName { get; set; } = default!;
            public DateTimeOffset CreatedAt { get; set; } = default!;

            public bool HasAnswer { get; set; }
        }
    }
}
