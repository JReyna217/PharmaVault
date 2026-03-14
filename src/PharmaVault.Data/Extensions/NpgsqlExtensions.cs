using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Npgsql;

namespace PharmaVault.Data.Extensions;

public static class NpgsqlExtensions
{
    private static readonly HashSet<string> VALORES_BOOLEAN_TRUE = new(StringComparer.OrdinalIgnoreCase) { "1", "S", "Y", "TRUE", "T" };

    public static async Task<List<T>> FillToObjectListAsync<T>(this NpgsqlCommand dbCommand) where T : new()
    {
        var result = new List<T>();
        
        await using var reader = await dbCommand.ExecuteReaderAsync();

        if (!reader.HasRows) return result;

        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();

        while (await reader.ReadAsync())
        {
            T obj = new T();

            foreach (PropertyInfo property in properties)
            {
                var value = GetValueSafe(reader, property.Name);

                if (value != null && value != DBNull.Value)
                {
                    SetValue(obj, property, value);
                }
            }
            result.Add(obj);
        }

        return result;
    }

    public static async Task<T?> FillToObjectAsync<T>(this NpgsqlCommand dbCommand) where T : class, new()
    {
        await using var reader = await dbCommand.ExecuteReaderAsync();

        if (!reader.HasRows || !await reader.ReadAsync()) 
            return null;

        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        T obj = new T();

        foreach (PropertyInfo property in properties)
        {
            var value = GetValueSafe(reader, property.Name);

            if (value != null && value != DBNull.Value)
            {
                SetValue(obj, property, value);
            }
        }

        return obj;
    }

    private static object? GetValueSafe(DbDataReader reader, string columnName)
    {
        try
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal);
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    private static void SetValue<T>(T obj, PropertyInfo property, object value)
    {
        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        try
        {
            if (targetType == typeof(bool))
            {
                var strValue = value.ToString()?.Trim();
                bool boolValue = strValue != null && VALORES_BOOLEAN_TRUE.Contains(strValue);
                property.SetValue(obj, boolValue);
            }
            else if (targetType == typeof(DateTime))
            {
                property.SetValue(obj, Convert.ToDateTime(value));
            }
            else if (targetType.IsEnum)
            {
                property.SetValue(obj, Enum.Parse(targetType, value.ToString()!));
            }
            else
            {
                var convertedValue = Convert.ChangeType(value, targetType);
                property.SetValue(obj, convertedValue);
            }
        }
        catch
        {
            //TODO: Save the error in log table or file
        }
    }
}