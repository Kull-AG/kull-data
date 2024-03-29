﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kull.Data
{
    internal static class SQLKeywords
    {
        // Everything that is a keywords in MS-SQL Server, SQL Standard, Postgres and SQLite
        private static SortedSet<string> keywords = new SortedSet<string>(StringComparer.CurrentCultureIgnoreCase)
        {
            "A",
"ABSOLUTE",
"HOST",
"RELATIVE",
"ACTION",
"HOUR",
"RELEASE",
"ADMIN",
"IGNORE",
"RESULT",
"AFTER",
"IMMEDIATE",
"RETURNS",
"AGGREGATE",
"INDICATOR",
"ROLE",
"ALIAS",
"INITIALIZE",
"ROLLUP",
"ALLOCATE",
"INITIALLY",
"ROUTINE",
"ARE",
"INOUT",
"ROW",
"ARRAY",
"INPUT",
"ROWS",
"ASENSITIVE",
"INT",
"SAVEPOINT",
"ASSERTION",
"INTEGER",
"SCROLL",
"ASYMMETRIC",
"INTERSECTION",
"SCOPE",
"AT",
"INTERVAL",
"SEARCH",
"ATOMIC",
"ISOLATION",
"SECOND",
"BEFORE",
"ITERATE",
"SECTION",
"BINARY",
"LANGUAGE",
"SENSITIVE",
"BIT",
"LARGE",
"SEQUENCE",
"BLOB",
"LAST",
"SESSION",
"BOOLEAN",
"LATERAL",
"SETS",
"BOTH",
"LEADING",
"SIMILAR",
"BREADTH",
"LESS",
"SIZE",
"CALL",
"LEVEL",
"SMALLINT",
"CALLED",
"LIKE_REGEX",
"SPACE",
"CARDINALITY",
"LIMIT",
"SPECIFIC",
"CASCADED",
"LN",
"SPECIFICTYPE",
"CAST",
"LOCAL",
"SQL",
"CATALOG",
"LOCALTIME",
"SQLEXCEPTION",
"CHAR",
"LOCALTIMESTAMP",
"SQLSTATE",
"CHARACTER",
"LOCATOR",
"SQLWARNING",
"CLASS",
"MAP",
"START",
"CLOB",
"MATCH",
"STATE",
"COLLATION",
"MEMBER",
"STATEMENT",
"COLLECT",
"METHOD",
"STATIC",
"COMPLETION",
"MINUTE",
"STDDEV_POP",
"CONDITION",
"MOD",
"STDDEV_SAMP",
"CONNECT",
"MODIFIES",
"STRUCTURE",
"CONNECTION",
"MODIFY",
"SUBMULTISET",
"CONSTRAINTS",
"MODULE",
"SUBSTRING_REGEX",
"CONSTRUCTOR",
"MONTH",
"SYMMETRIC",
"CORR",
"MULTISET",
"SYSTEM",
"CORRESPONDING",
"NAMES",
"TEMPORARY",
"COVAR_POP",
"NATURAL",
"TERMINATE",
"COVAR_SAMP",
"NCHAR",
"THAN",
"CUBE",
"NCLOB",
"TIME",
"CUME_DIST",
"NEW",
"TIMESTAMP",
"CURRENT_CATALOG",
"NEXT",
"TIMEZONE_HOUR",
"CURRENT_DEFAULT_TRANSFORM_GROUP",
"NO",
"TIMEZONE_MINUTE",
"CURRENT_PATH",
"NONE",
"TRAILING",
"CURRENT_ROLE",
"NORMALIZE",
"TRANSLATE_REGEX",
"CURRENT_SCHEMA",
"NUMERIC",
"TRANSLATION",
"CURRENT_TRANSFORM_GROUP_FOR_TYPE",
"OBJECT",
"TREAT",
"CYCLE",
"OCCURRENCES_REGEX",
"TRUE",
"DATA",
"OLD",
"UESCAPE",
"DATE",
"ONLY",
"UNDER",
"DAY",
"OPERATION",
"UNKNOWN",
"DEC",
"ORDINALITY",
"UNNEST",
"DECIMAL",
"OUT",
"USAGE",
"DEFERRABLE",
"OVERLAY",
"USING",
"DEFERRED",
"OUTPUT",
"VALUE",
"DEPTH",
"PAD",
"VAR_POP",
"DEREF",
"PARAMETER",
"VAR_SAMP",
"DESCRIBE",
"PARAMETERS",
"VARCHAR",
"DESCRIPTOR",
"PARTIAL",
"VARIABLE",
"DESTROY",
"PARTITION",
"WHENEVER",
"DESTRUCTOR",
"PATH",
"WIDTH_BUCKET",
"DETERMINISTIC",
"POSTFIX",
"WITHOUT",
"DICTIONARY",
"PREFIX",
"WINDOW",
"DIAGNOSTICS",
"PREORDER",
"WITHIN",
"DISCONNECT",
"PREPARE",
"WORK",
"DOMAIN",
"PERCENT_RANK",
"WRITE",
"DYNAMIC",
"PERCENTILE_CONT",
"XMLAGG",
"EACH",
"PERCENTILE_DISC",
"XMLATTRIBUTES",
"ELEMENT",
"POSITION_REGEX",
"XMLBINARY",
"END-EXEC",
"PRESERVE",
"XMLCAST",
"EQUALS",
"PRIOR",
"XMLCOMMENT",
"EVERY",
"PRIVILEGES",
"XMLCONCAT",
"EXCEPTION",
"RANGE",
"XMLDOCUMENT",
"FALSE",
"READS",
"XMLELEMENT",
"FILTER",
"REAL",
"XMLEXISTS",
"FIRST",
"RECURSIVE",
"XMLFOREST",
"FLOAT",
"REF",
"XMLITERATE",
"FOUND",
"REFERENCING",
"XMLNAMESPACES",
"FREE",
"REGR_AVGX",
"XMLPARSE",
"FULLTEXTTABLE",
"REGR_AVGY",
"XMLPI",
"FUSION",
"REGR_COUNT",
"XMLQUERY",
"GENERAL",
"REGR_INTERCEPT",
"XMLSERIALIZE",
"GET",
"REGR_R2",
"XMLTABLE",
"GLOBAL",
"REGR_SLOPE",
"XMLTEXT",
"GO",
"REGR_SXX",
"XMLVALIDATE",
"GROUPING",
"REGR_SXY",
"YEAR",
"HOLD",
"REGR_SYY",
"ZONE",
"A",
"ABORT",
"ABS",
"ACCESS",
"ADA",
"ADD",
"ALL",
"ALSO",
"ALTER",
"ALWAYS",
"ANALYSE",
"ANALYZE",
"AND",
"ANY",
"AS",
"ASC",
"ASSIGNMENT",
"ATTRIBUTE",
"ATTRIBUTES",
"AUTHORIZATION",
"AVG",
"BACKWARD",
"BEGIN",
"BERNOULLI",
"BETWEEN",
"BIGINT",
"BITVAR",
"BIT_LENGTH",
"BY",
"C",
"CACHE",
"CASCADE",
"CASE",
"CATALOG_NAME",
"CEIL",
"CEILING",
"CHAIN",
"CHARACTERISTICS",
"CHARACTERS",
"CHARACTER_LENGTH",
"CHARACTER_SET_CATALOG",
"CHARACTER_SET_NAME",
"CHARACTER_SET_SCHEMA",
"CHAR_LENGTH",
"CHECK",
"CHECKED",
"CHECKPOINT",
"CLASS_ORIGIN",
"CLOSE",
"CLUSTER",
"COALESCE",
"COBOL",
"COLLATE",
"COLLATION_CATALOG",
"COLLATION_NAME",
"COLLATION_SCHEMA",
"COLUMN",
"COLUMN_NAME",
"COMMAND_FUNCTION",
"COMMAND_FUNCTION_CODE",
"COMMENT",
"COMMIT",
"COMMITTED",
"CONDITION_NUMBER",
"CONNECTION_NAME",
"CONSTRAINT",
"CONSTRAINT_CATALOG",
"CONSTRAINT_NAME",
"CONSTRAINT_SCHEMA",
"CONTAINS",
"CONTINUE",
"CONVERSION",
"CONVERT",
"COPY",
"COUNT",
"CREATE",
"CREATEDB",
"CREATEROLE",
"CREATEUSER",
"CROSS",
"CSV",
"CURRENT",
"CURRENT_DATE",
"CURRENT_TIME",
"CURRENT_TIMESTAMP",
"CURRENT_USER",
"CURSOR",
"CURSOR_NAME",
"DATABASE",
"DATETIME_INTERVAL_CODE",
"DATETIME_INTERVAL_PRECISION",
"DEALLOCATE",
"DECLARE",
"DEFAULT",
"DEFAULTS",
"DEFINED",
"DEFINER",
"DEGREE",
"DELETE",
"DELIMITER",
"DELIMITERS",
"DENSE_RANK",
"DERIVED",
"DESC",
"DISABLE",
"DISPATCH",
"DISTINCT",
"DO",
"DOUBLE",
"DROP",
"DYNAMIC_FUNCTION",
"DYNAMIC_FUNCTION_CODE",
"ELSE",
"ENABLE",
"ENCODING",
"ENCRYPTED",
"END",
"ESCAPE",
"EXCEPT",
"EXCLUDE",
"EXCLUDING",
"EXCLUSIVE",
"EXEC",
"EXECUTE",
"EXISTING",
"EXISTS",
"EXP",
"EXPLAIN",
"EXTERNAL",
"EXTRACT",
"FETCH",
"FINAL",
"FLOOR",
"FOLLOWING",
"FOR",
"FORCE",
"FOREIGN",
"FORTRAN",
"FORWARD",
"FREEZE",
"FROM",
"FULL",
"FUNCTION",
"G",
"GENERATED",
"GOTO",
"GRANT",
"GRANTED",
"GREATEST",
"GROUP",
"HANDLER",
"HAVING",
"HEADER",
"HIERARCHY",
"IDENTITY",
"ILIKE",
"IMMUTABLE",
"IMPLEMENTATION",
"IMPLICIT",
"IN",
"INCLUDING",
"INCREMENT",
"INDEX",
"INFIX",
"INHERIT",
"INHERITS",
"INNER",
"INSENSITIVE",
"INSERT",
"INSTANCE",
"INSTANTIABLE",
"INSTEAD",
"INTERSECT",
"INTO",
"INVOKER",
"IS",
"ISNULL",
"JOIN",
"K",
"KEY",
"KEY_MEMBER",
"KEY_TYPE",
"LANCOMPILER",
"LEAST",
"LEFT",
"LENGTH",
"LIKE",
"LISTEN",
"LOAD",
"LOCATION",
"LOCK",
"LOGIN",
"LOWER",
"M",
"MATCHED",
"MAX",
"MAXVALUE",
"MERGE",
"MESSAGE_LENGTH",
"MESSAGE_OCTET_LENGTH",
"MESSAGE_TEXT",
"MIN",
"MINVALUE",
"MODE",
"MORE",
"MOVE",
"MUMPS",
"NAME",
"NATIONAL",
"NESTING",
"NOCREATEDB",
"NOCREATEROLE",
"NOCREATEUSER",
"NOINHERIT",
"NOLOGIN",
"NORMALIZED",
"NOSUPERUSER",
"NOT",
"NOTHING",
"NOTIFY",
"NOTNULL",
"NOWAIT",
"NULL",
"NULLABLE",
"NULLIF",
"NULLS",
"NUMBER",
"OCTETS",
"OCTET_LENGTH",
"OF",
"OFF",
"OFFSET",
"OIDS",
"ON",
"OPEN",
"OPERATOR",
"OPTION",
"OPTIONS",
"OR",
"ORDER",
"ORDERING",
"OTHERS",
"OUTER",
"OVER",
"OVERLAPS",
"OVERRIDING",
"OWNER",
"PARAMETER_MODE",
"PARAMETER_NAME",
"PARAMETER_ORDINAL_POSITION",
"PARAMETER_SPECIFIC_CATALOG",
"PARAMETER_SPECIFIC_NAME",
"PARAMETER_SPECIFIC_SCHEMA",
"PASCAL",
"PASSWORD",
"PLACING",
"PLI",
"POSITION",
"POWER",
"PRECEDING",
"PRECISION",
"PREPARED",
"PRIMARY",
"PROCEDURAL",
"PROCEDURE",
"PUBLIC",
"QUOTE",
"RANK",
"READ",
"RECHECK",
"REFERENCES",
"REINDEX",
"RENAME",
"REPEATABLE",
"REPLACE",
"RESET",
"RESTART",
"RESTRICT",
"RETURN",
"RETURNED_CARDINALITY",
"RETURNED_LENGTH",
"RETURNED_OCTET_LENGTH",
"RETURNED_SQLSTATE",
"REVOKE",
"RIGHT",
"ROLLBACK",
"ROUTINE_CATALOG",
"ROUTINE_NAME",
"ROUTINE_SCHEMA",
"ROW_COUNT",
"ROW_NUMBER",
"RULE",
"SCALE",
"SCHEMA",
"SCHEMA_NAME",
"SCOPE_CATALOG",
"SCOPE_NAME",
"SCOPE_SCHEMA",
"SECURITY",
"SELECT",
"SELF",
"SERIALIZABLE",
"SERVER_NAME",
"SESSION_USER",
"SET",
"SETOF",
"SHARE",
"SHOW",
"SIMPLE",
"SOME",
"SOURCE",
"SPECIFIC_NAME",
"SQLCODE",
"SQLERROR",
"SQRT",
"STABLE",
"STATISTICS",
"STDIN",
"STDOUT",
"STORAGE",
"STRICT",
"STYLE",
"SUBCLASS_ORIGIN",
"SUBLIST",
"SUBSTRING",
"SUM",
"SUPERUSER",
"SYSID",
"SYSTEM_USER",
"TABLE",
"TABLESAMPLE",
"TABLESPACE",
"TABLE_NAME",
"TEMP",
"TEMPLATE",
"THEN",
"TIES",
"TO",
"TOAST",
"TOP_LEVEL_COUNT",
"TRANSACTION",
"TRANSACTIONS_COMMITTED",
"TRANSACTIONS_ROLLED_BACK",
"TRANSACTION_ACTIVE",
"TRANSFORM",
"TRANSFORMS",
"TRANSLATE",
"TRIGGER",
"TRIGGER_CATALOG",
"TRIGGER_NAME",
"TRIGGER_SCHEMA",
"TRIM",
"TRUNCATE",
"TRUSTED",
"TYPE",
"UNBOUNDED",
"UNCOMMITTED",
"UNENCRYPTED",
"UNION",
"UNIQUE",
"UNLISTEN",
"UNNAMED",
"UNTIL",
"UPDATE",
"UPPER",
"USER",
"USER_DEFINED_TYPE_CATALOG",
"USER_DEFINED_TYPE_CODE",
"USER_DEFINED_TYPE_NAME",
"USER_DEFINED_TYPE_SCHEMA",
"VACUUM",
"VALID",
"VALIDATOR",
"VALUES",
"VARYING",
"VERBOSE",
"VIEW",
"VOLATILE",
"WHEN",
"WHERE",
"WITH",
"ATTACH",
"AUTOINCREMENT",
"CONFLICT",
"DETACH",
"FAIL",
"GLOB",
"GROUPS",
"IF",
"INDEXED",
"MATERIALIZED",
"PLAN",
"PRAGMA",
"QUERY",
"RAISE",
"REGEXP",
"RETURNING",
"VIRTUAL"
        };

        public static bool IsSqlKeywords(string word) => keywords.Contains(word);
    }
}
