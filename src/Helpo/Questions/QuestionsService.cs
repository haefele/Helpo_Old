using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpo.Common;
using Helpo.Services.Auth;
using Raven.Client.Documents;

namespace Helpo.Questions
{
    public class QuestionsService
    {
        private readonly IDocumentStore _documentStore;
        private readonly AuthService _authService;

        public QuestionsService(IDocumentStore documentStore, AuthService authService)
        {
            Guard.NotNull(documentStore, nameof(documentStore));
            Guard.NotNull(authService, nameof(authService));

            this._documentStore = documentStore;
            this._authService = authService;
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

        public async Task<Question> CreateNew(string title, string body, List<string> tags, string application)
        {
            var user = await this._authService.GetLoggedInUser();

            if (user == null)
                throw new Exception("Not logged in.");
            
            using var session = this._documentStore.OpenAsyncSession();

            var question = Question.CreateNew(title, body, user.UserId, tags, application);
            await session.StoreAsync(question);

            await session.SaveChangesAsync();

            return question;
        }
    }
}
