using Marten;
using snowcoreBlog.ApplicationLaunch.Interfaces;
using snowcoreBlog.Backend.Articles.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Article;

namespace snowcoreBlog.Backend.Articles.Services;

public class ArticlesApplicationLaunchService(IServiceProvider serviceProvider) : IApplicationLaunchService
{
    public async Task InitAsync()
    {
        using var scope = serviceProvider.CreateScope();
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
                // All versions of the article ordered oldest → newest
                var snapshots = ArticleSeedData.Snapshots
                    .Where(s => s.ArticleId == article.Id)
                    .OrderBy(s => s.ModifiedAt)
                    .ToArray();

                if (snapshots.Length == 0)
                    continue;

                // Ensure article.LatestSnapshotId references the NEWEST snapshot we are inserting
                var latest = snapshots[^1];
                var entityToInsert = article with { LatestSnapshotId = latest.Id };

                try
                {
                    await articleRepository.InsertArticleWithSnapshotAsync(entityToInsert, latest);

                    // Insert the older snapshots as version history
                    for (var i = 0; i < snapshots.Length - 1; i++)
                        await articleRepository.InsertSnapshotAsync(snapshots[i]);
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