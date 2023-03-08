using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IEntityTypeToTableNameMap
{
    string GetTableName<TEntity>() where TEntity : class;
    string GetViewName<TEntity>() where TEntity : class;
}

public class EntityTypeToTableNameMap : IEntityTypeToTableNameMap
{
    private readonly Dictionary<Type, string> _entityTypeToTableNameMap = new Dictionary<Type, string>();
    private readonly Dictionary<Type, string> _entityTypeToViewNameMap = new Dictionary<Type, string>();

    public EntityTypeToTableNameMap(DbContext context)
    {
        foreach (var entityType in context.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (!_entityTypeToTableNameMap.ContainsKey(clrType))
            {
                var tableName = entityType.GetTableName();
                _entityTypeToTableNameMap[clrType] = tableName;
            }
            if (!_entityTypeToViewNameMap.ContainsKey(clrType))
            {
                var viewNameAnnotation = entityType.FindAnnotation("Relational:ViewName");
                if (viewNameAnnotation != null)
                {
                    var viewName = viewNameAnnotation.Value.ToString();
                    _entityTypeToViewNameMap[clrType] = viewName;
                }
            }
        }
    }

    public string GetTableName<TEntity>() where TEntity : class
    {
        var entityType = typeof(TEntity);
        if (!_entityTypeToTableNameMap.TryGetValue(entityType, out var tableName))
        {
            throw new ArgumentException($"Entity type '{entityType.FullName}' not found in mapping dictionary.", nameof(TEntity));
        }
        return tableName;
    }

    public string GetViewName<TEntity>() where TEntity : class
    {
        var entityType = typeof(TEntity);
        if (!_entityTypeToViewNameMap.TryGetValue(entityType, out var viewName))
        {
            throw new ArgumentException($"Entity type '{entityType.FullName}' not found in mapping dictionary.", nameof(TEntity));
        }
        return viewName;
    }
}
