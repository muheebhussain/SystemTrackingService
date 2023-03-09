using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DbContextExtensions
{
    private static readonly Dictionary<Type, string> EntityTypeToTableNameMap = new Dictionary<Type, string>();

    public static void InitializeEntityTypeToTableNameMap(this DbContext context)
    {
        foreach (var entityType in context.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (!EntityTypeToTableNameMap.ContainsKey(clrType))
            {
                var tableName = entityType.GetTableName();
                EntityTypeToTableNameMap[clrType] = tableName;
            }
        }
    }

    public static IQueryable<string> GetTableNamesForEntity<TEntity>(this DbContext context) where TEntity : class
    {
        var entityType = typeof(TEntity);
        if (!EntityTypeToTableNameMap.TryGetValue(entityType, out var tableName))
        {
            throw new ArgumentException($"Entity type '{entityType.FullName}' not found in mapping dictionary.", nameof(TEntity));
        }
        var tableNames = context.Model.GetEntityTypes()
                                       .Where(t => t.GetTableName() == tableName || t.GetTableName() == tableName.Replace('.', '_'))
                                       .Select(t => t.GetTableName())
                                       .Distinct()
                                       .AsQueryable();
        return tableNames;
    }
}
