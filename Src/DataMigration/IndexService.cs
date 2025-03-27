using DbType = SqlSugar.DbType;

namespace DataMigration;

public class IndexService
{
    private readonly SqlSugarClient _db;

    public IndexService(SqlSugarClient db)
    {
        _db = db;
    }

    public List<IndexInfo> GetIndexes(string tableName)
    {
        return _db.CurrentConnectionConfig.DbType switch
        {
            DbType.SqlServer => GetSqlServerIndexes(tableName),
            DbType.MySql => GetMySqlIndexes(tableName),
            DbType.Oracle => GetOracleIndexes(tableName),
            DbType.PostgreSQL => GetPostgreSqlIndexes(tableName),
            DbType.Dm => GetDmIndexes(tableName),
            DbType.Kdbndp => GetKdbndpIndexes(tableName),
            DbType.Sqlite => GetSqliteIndexes(tableName),
            _ => new List<IndexInfo>()
        };
    }

    private List<IndexInfo> GetSqlServerIndexes(string tableName)
    {
        var sql = $@"SELECT 
                        i.name AS IndexName,
                        c.name + CASE ic.is_descending_key WHEN 1 THEN ' DESC' ELSE ' ASC' END AS ColumnName,
                        i.is_unique AS IsUnique
                    FROM sys.indexes i
                    INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                    WHERE i.is_primary_key = 0 AND i.object_id = OBJECT_ID('{tableName}')";

        return ExecuteAndMap(sql);
    }

    private List<IndexInfo> GetMySqlIndexes(string tableName)
    {
        var sql = $@"SELECT 
                        INDEX_NAME AS IndexName,
                        CONCAT(COLUMN_NAME, CASE WHEN COLLATION = 'D' THEN ' DESC' ELSE ' ASC' end) AS ColumnName,
                        CASE WHEN NON_UNIQUE = 0 THEN 1
					        ELSE 0
					    END AS IsUnique
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE INDEX_NAME != 'PRIMARY' AND TABLE_NAME = '{tableName}'";

        return ExecuteAndMap(sql);
    }

    private List<IndexInfo> GetOracleIndexes(string tableName)
    {
        var sql = $@"SELECT UI.INDEX_NAME AS INDEXNAME,
                           UIC.COLUMN_NAME || CASE UIC.DESCEND
                             WHEN 'DESC' THEN
                              ' DESC'
                             ELSE
                              ' ASC'
                           END AS COLUMNNAME,
                           CASE
                             WHEN UI.UNIQUENESS = 'UNIQUE' THEN
                              1
                             ELSE
                              0
                           END AS ISUNIQUE
                      FROM USER_INDEXES UI
                      JOIN USER_IND_COLUMNS UIC
                        ON UI.INDEX_NAME = UIC.INDEX_NAME
                      LEFT JOIN USER_CONSTRAINTS UC 
                        ON UI.INDEX_NAME = UC.CONSTRAINT_NAME 
                        AND UC.CONSTRAINT_TYPE = 'P'
                     WHERE UC.CONSTRAINT_NAME IS NULL AND UI.TABLE_NAME = '{tableName.ToUpper()}'";

        return ExecuteAndMap(sql);
    }

    private List<IndexInfo> GetPostgreSqlIndexes(string tableName)
    {
        var sql = $@"SELECT
                        i.relname AS IndexName,
                        a.attname || CASE 
                            WHEN ix.indoption[k.pos-1] & 1 = 1 THEN ' DESC' 
                            ELSE ' ASC' 
                        END AS ColumnName,
                        CASE 
                            WHEN ix.indisunique THEN 1 
                            ELSE 0 
                        END AS IsUnique
                    FROM
                        pg_class t
                        JOIN pg_index ix ON t.oid = ix.indrelid
                        JOIN pg_class i ON ix.indexrelid = i.oid
                        JOIN pg_am am ON i.relam = am.oid,
                        LATERAL unnest(ix.indkey) WITH ORDINALITY AS k(attnum, pos)
                        JOIN pg_attribute a ON a.attrelid = t.oid AND a.attnum = k.attnum
                    WHERE NOT ix.indisprimary
                        t.relname = '{tableName}'
                    ORDER BY
                        table_name, index_name, k.pos";

        return ExecuteAndMap(sql);
    }

    private List<IndexInfo> GetDmIndexes(string tableName)
    {
        var sql = $@"SELECT 
                        a.INDEX_NAME IndexName,
                        b.COLUMN_NAME || ' ' || b.DESCEND ColumnName,
                        CASE WHEN a.UNIQUENESS = 'UNIQUE' THEN 1
                             ELSE 0 END AS IsUnique
                    FROM 
                        USER_INDEXES a
                    JOIN 
                        USER_IND_COLUMNS b                     
                    ON 
                        a.INDEX_NAME = b.INDEX_NAME 
                    LEFT JOIN 
                        USER_CONSTRAINTS c ON a.INDEX_NAME = c.CONSTRAINT_NAME
                    WHERE 
                        (c.CONSTRAINT_TYPE != 'P' OR c.CONSTRAINT_TYPE IS NULL)
                        AND a.TABLE_NAME = '{tableName.ToUpper()}'";

        return ExecuteAndMap(sql);
    }

    private List<IndexInfo> GetKdbndpIndexes(string tableName)
    {
        var sql = $@"SELECT 
                        i.relname AS IndexName,
                        a.attname || CASE ix.indoption[i.key] & 1 
                            WHEN 1 THEN ' DESC' 
                            ELSE ' ASC' 
                        END AS ColumnName,
                        CASE ix.indisunique 
                            WHEN true THEN 1 
                            ELSE 0 
                        END AS IsUnique
                    FROM 
                        pg_class c
                    JOIN pg_index ix ON c.oid = ix.indrelid
                    JOIN pg_class i ON ix.indexrelid = i.oid
                    JOIN pg_attribute a ON a.attrelid = c.oid AND a.attnum = ANY(ix.indkey)
                    CROSS JOIN LATERAL unnest(ix.indkey) WITH ORDINALITY AS i(key, idx)
                    LEFT JOIN pg_constraint co ON co.conindid = ix.indexrelid AND co.contype = 'p'
                    WHERE 
                        c.relname = '{tableName}' 
                        AND c.relkind = 'r' 
                        AND co.contype IS NULL 
                    ORDER BY i.relname, i.idx";

        return ExecuteAndMap(sql);
    }

    private List<IndexInfo> GetSqliteIndexes(string tableName)
    {
        var sql = $@"SELECT 
                        m.name AS IndexName,
                        ii.name || CASE 
                            WHEN m.sql LIKE '%' || ii.name || ' DESC%' THEN 'DESC' 
                            ELSE 'ASC' 
                        END AS ColumnName,
                        CASE WHEN m.sql LIKE '%UNIQUE%' THEN 1 ELSE 0 END AS IsUnique
                    FROM sqlite_master AS m
                    JOIN pragma_index_info(m.name) AS ii
                    WHERE m.type = 'index' AND m.name NOT LIKE 'sqlite_autoindex%'
                    AND m.tbl_name = '{tableName}'";

        return ExecuteAndMap(sql);
    }

    private List<IndexInfo> ExecuteAndMap(string sql)
    {
        var list = _db.Ado.SqlQuery<IndexData>(sql);
        return list?
            .GroupBy(x => (string)x.IndexName)
            .Select(g => new IndexInfo
            {
                IndexName = g.Key.IndexNameFormat(),
                Columns = g.Select(c => (string)c.ColumnName).ToArray(),
                IsUnique = g.First().IsUnique
            }).ToList();
    }
}

public class IndexData
{
    public string IndexName { get; set; }
    public string ColumnName { get; set; }
    public bool IsUnique { get; set; }
}

public class IndexInfo
{
    public string IndexName { get; set; }
    public string[] Columns { get; set; }
    public bool IsUnique { get; set; }
}