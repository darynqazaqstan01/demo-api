namespace DemoApi.Data.InMemory;

using DemoApi.Domain;

public sealed class InMemoryProjectRepository(InMemoryStore store) : IProjectRepository
{
    public Task<IReadOnlyList<Project>> ListAsync(CancellationToken ct)
    {
        lock (store.Sync)
        {
            return Task.FromResult<IReadOnlyList<Project>>(store.Projects.ToList());
        }
    }

    public Task<Project?> GetAsync(int id, CancellationToken ct)
    {
        lock (store.Sync)
        {
            return Task.FromResult(store.Projects.FirstOrDefault(p => p.Id == id));
        }
    }

    public Task<bool> NameExistsAsync(string name, CancellationToken ct)
    {
        lock (store.Sync)
        {
            return Task.FromResult(store.Projects.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }
    }

    public Task<Project> CreateAsync(string name, CancellationToken ct)
    {
        lock (store.Sync)
        {
            var id = store.Projects.Count > 0 ? store.Projects.Max(p => p.Id) + 1 : 1;
            var project = new Project(id, name, DateTimeOffset.UtcNow);
            store.Projects.Add(project);
            return Task.FromResult(project);
        }
    }

    public Task<Project?> RenameAsync(int id, string name, CancellationToken ct)
    {
        lock (store.Sync)
        {
            var index = store.Projects.FindIndex(p => p.Id == id);
            if (index == -1) return Task.FromResult<Project?>(null);

            var updated = store.Projects[index] with { Name = name };
            store.Projects[index] = updated;
            return Task.FromResult<Project?>(updated);
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        lock (store.Sync)
        {
            var count = store.Projects.RemoveAll(p => p.Id == id);
            return Task.FromResult(count > 0);
        }
    }
}