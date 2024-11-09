namespace CertificationHub.Core.Library.Models;

public interface IFilter<TEntity> where TEntity : IEntity
{
    public string? SearchTerm { get; set; }
    IQueryable<TEntity> Filter(IQueryable<TEntity> filterQuery);
}