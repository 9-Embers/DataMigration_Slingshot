using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace lib.Onrealm.Data;

public static class Database
{


    public static void ExecuteNonQuery( string commandText, Dictionary<string, string>? parameters = null )
    {
        try
        {

            var command = Connection.CreateCommand();
            command.CommandText = commandText;
            if ( parameters != null )
            {
                foreach ( var kvp in parameters )
                {
                    command.Parameters.AddWithValue( kvp.Key, kvp.Value );
                }
            }
            command.ExecuteNonQuery();
        }
        catch ( Exception ex )
        { }
    }


    public static IEnumerable<T> ExecuteQuery<T>( string commandText, Dictionary<string, string>? parameters = null )
    {
        try
        {
            var dynamicParameters = new DynamicParameters();
            foreach ( var par in parameters ?? new Dictionary<string, string>() )
            {
                dynamicParameters.Add( par.Key, par.Value );
            }


            var com = new CommandDefinition( commandText, dynamicParameters );
            return Connection.Query<T>( com );
        }
        catch ( Exception ex )
        {
        }
        return new List<T>();
    }

    private static SqliteConnection? _connection = null;
    public static SqliteConnection Connection
    {
        get
        {
            if ( _connection == null )
            {
                var path = Path.Combine( System.IO.Path.GetDirectoryName( Assembly.GetEntryAssembly()!.Location )!, "Onrealm.db" );
                _connection = new SqliteConnection( "Data Source=" + path );
            }
            if ( _connection.State != System.Data.ConnectionState.Open )
            {
                _connection.Open();
            }
            return _connection;
        }
    }
}

