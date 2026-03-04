using Xunit;

namespace FBZSystemMvc.UITests.Infrastructure;

[CollectionDefinition("UI")]
public sealed class UiTestCollection : ICollectionFixture<AppServerFixture>, ICollectionFixture<SeleniumFixture>
{
}