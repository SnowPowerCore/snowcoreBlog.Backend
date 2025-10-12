using snowcoreBlog.ApplicationLaunch.Interfaces;
using Marten;
using snowcoreBlog.Backend.Articles.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Article;

namespace snowcoreBlog.Backend.Articles.Services;

public class ArticlesApplicationLaunchService : IApplicationLaunchService
{
    private readonly IServiceProvider _serviceProvider;

    public ArticlesApplicationLaunchService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InitAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var articleRepository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();
        var querySession = scope.ServiceProvider.GetRequiredService<IQuerySession>();

        try
        {
            // If there are already articles in the DB, skip seeding
            var any = await querySession.Query<ArticleEntity>().AnyAsync();
            if (any)
                return;

            // Seed articles and snapshots from ArticleSeedData
            foreach (var article in ArticleSeedData.Articles)
            {
                // Find the initial snapshot(s) for this article
                var initial = ArticleSeedData.Snapshots.OrderBy(s => s.ModifiedAt).FirstOrDefault(s => s.ArticleId == article.Id);
                if (initial is null)
                    continue;

                // Ensure article.LatestSnapshotId references the snapshot we are inserting
                var entityToInsert = article with { LatestSnapshotId = initial.Id };

                try
                {
                    await articleRepository.InsertArticleWithSnapshotAsync(entityToInsert, initial);
                }
                catch
                {
                    // swallow - seeding should not crash app startup
                }
            }
        }
        catch
        {
            // ignore errors during application launch seeding
        }
    }
}