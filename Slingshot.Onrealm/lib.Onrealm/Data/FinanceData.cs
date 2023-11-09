using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using lib.Onrealm.Contracts;
using lib.Onrealm.Manager;
using lib.Onrealm.Model;

namespace lib.Onrealm.Data;

public static class FinanceData
{
    public static List<Fund> Funds { get; set; } = new List<Fund>();

    public static List<Batch> ManualBatches { get; set; } = new List<Batch>();
    private static ConcurrentQueue<Batch> ManualBatchQueue { get; set; } = new ConcurrentQueue<Batch>();


    public static List<Batch> OnlineBatches { get; set; } = new List<Batch>();
    private static ConcurrentQueue<Batch> OnlineBatchQueue { get; set; } = new ConcurrentQueue<Batch>();

    public static List<Refund> Refunds { get; set; } = new List<Refund>();

    public static List<ManualContribution> ManualContributions { get; set; } = new List<ManualContribution>();
    private static ConcurrentQueue<ManualContribution> ManualContributionsQueue { get; set; } = new ConcurrentQueue<ManualContribution>();
    public static List<OnlineContribution> OnlineContributions { get; set; } = new List<OnlineContribution>();
    private static ConcurrentQueue<OnlineContribution> OnlineContributionsQueue { get; set; } = new ConcurrentQueue<OnlineContribution>();


    public static async Task Run( string cookie )
    {
        Funds.Clear();
        ManualBatchQueue.Clear();
        ManualBatches.Clear();
        OnlineBatches.Clear();
        OnlineBatchQueue.Clear();

        Database.ExecuteNonQuery( "CREATE TABLE IF NOT EXISTS [ManualBatch] (Id INTEGER PRIMARY KEY AUTOINCREMENT, BatchId TEXT, BatchJson TEXT, ContributionsJson TEXT)" );
        Database.ExecuteNonQuery( "CREATE TABLE IF NOT EXISTS [OnlineBatch] (Id INTEGER PRIMARY KEY AUTOINCREMENT, BatchId TEXT, BatchJson TEXT, ContributionsJson TEXT)" );
        Database.ExecuteNonQuery( "CREATE TABLE IF NOT EXISTS [Fund] (Id INTEGER PRIMARY KEY AUTOINCREMENT, FundId TEXT, FundJson TEXT)" );
        Database.ExecuteNonQuery( "CREATE TABLE IF NOT EXISTS [Refund] (Id INTEGER PRIMARY KEY AUTOINCREMENT, RefundId TEXT, RefundJson TEXT)" );


        var requestManager = new RequestManager( cookie );

        //FUNDS
        Debug.WriteLine( $"Loading Funds" );
        if ( !Database.ExecuteQuery<int>( "SELECT Id FROM [Fund]" ).Any() )
        {
            try
            {
                var funds = requestManager.GetFundsListAsync();
                await foreach ( var fund in funds )
                {
                    Database.ExecuteNonQuery( "INSERT INTO [Fund] (FundId,FundJson) Values ($fundId,$fundJson)",
                        new Dictionary<string, string>
                        {
                            { "$fundId", fund.Id! },
                            { "$fundJson", JsonSerializer.Serialize(fund) }
                        } );
                }
            }
            catch ( Exception ex )
            {
                Database.ExecuteNonQuery( "DROP TABLE [Fund]" );
                throw new Exception( "", ex );
            }
        }


        Debug.WriteLine( $"Loading Manual Batches" );
        if ( !Database.ExecuteQuery<int>( "SELECT Id FROM [ManualBatch]" ).Any() )
        {
            try
            {
                var manualBatches = requestManager.GetManualBatchListAsync();
                await foreach ( var batch in manualBatches )
                {
                    Database.ExecuteNonQuery( "INSERT INTO [ManualBatch] (BatchId,BatchJson) Values ($batchId,$batchJson)",
                       new Dictionary<string, string>
                       {
                            { "$batchId", batch.Id! },
                            { "$batchJson", JsonSerializer.Serialize(batch) }
                       } );
                }
            }
            catch ( Exception ex )
            {
                Database.ExecuteNonQuery( "DROP TABLE [ManualBatch]" );
                throw new Exception( "", ex );
            }
        }

        Debug.WriteLine( $"Loading Online Batches" );
        if ( !Database.ExecuteQuery<int>( "SELECT Id FROM [OnlineBatch]" ).Any() )
        {
            try
            {
                var onlineBatches = requestManager.GetOnlineBatchListAsync();
                await foreach ( var batch in onlineBatches )
                {
                    Database.ExecuteNonQuery( "INSERT INTO [OnlineBatch] (BatchId,BatchJson) Values ($batchId,$batchJson)",
                       new Dictionary<string, string>
                       {
                            { "$batchId", batch.Id! },
                            { "$batchJson", JsonSerializer.Serialize(batch) }
                       } );
                }
            }
            catch ( Exception ex )
            {
                Database.ExecuteNonQuery( "DROP TABLE [OnlineBatch]" );
                throw new Exception( "", ex );
            }
        }


        Debug.WriteLine( $"Loading Refunds" );
        Refunds = await requestManager.GetRefundsAsync();

        var series = Enumerable.Range( 1, 5 ).ToList();

        var manualBatchQry = Database.ExecuteQuery<SerializedBatch>( "SELECT * FROM [ManualBatch] WHERE ContributionsJson is null" );
        foreach ( var item in manualBatchQry )
        {
            ManualBatchQueue.Enqueue( JsonSerializer.Deserialize<Batch>( item.BatchJson! )! );
        }

        var manualTasks = new List<Task>();
        foreach ( var i in series )
        {
            manualTasks.Add( ProcessManualContributions( cookie ) );
        }
        await Task.WhenAll( manualTasks );
        ManualContributions.AddRange( ManualContributionsQueue );
        ManualContributionsQueue.Clear();


        var onlineBatchQry = Database.ExecuteQuery<SerializedBatch>( "SELECT * FROM [OnlineBatch] WHERE ContributionsJson is null" );
        foreach ( var item in onlineBatchQry )
        {
            OnlineBatchQueue.Enqueue( JsonSerializer.Deserialize<Batch>( item.BatchJson! )! );
        }

        var onlineTasks = new List<Task>();
        foreach ( var i in series )
        {
            onlineTasks.Add( ProcessOnlineContributions( cookie ) );
        }
        await Task.WhenAll( onlineTasks );
        OnlineContributions.AddRange( OnlineContributionsQueue );
        OnlineContributionsQueue.Clear();
    }

    private static async Task ProcessManualContributions( string cookie )
    {
        var requestManager = new RequestManager( cookie );

        while ( true )
        {
            if ( ManualBatchQueue.TryDequeue( out var batch ) )
            {
                if ( batch?.Id == null )
                {
                    continue;
                }

                Debug.WriteLine( $"Loading Manual Contributions For {batch.Id}" );
                var contributions = await requestManager.GetManualBatchContributionsAsync( batch.Id );

                Database.ExecuteNonQuery( "UPDATE [ManualBatch] SET ContributionsJson = $contributionsJson WHERE BatchId = $batchId",
                                   new Dictionary<string, string> {
                                            { "$contributionsJson", JsonSerializer.Serialize(contributions) },
                                            { "$batchId", batch.Id}
                                   } );
            }
            else
            {
                break;
            }
        }
    }

    private static async Task ProcessOnlineContributions( string cookie )
    {
        var requestManager = new RequestManager( cookie );

        while ( true )
        {
            if ( ManualBatchQueue.TryDequeue( out var batch ) )
            {
                if ( batch?.Id == null )
                {
                    continue;
                }

                try
                {

                    Debug.WriteLine( $"Loading Online Contributions For {batch.Id}" );
                    var contributions = await requestManager.GetOnlineBatchContributionsAsync( batch.Id );

                    Database.ExecuteNonQuery( "UPDATE [OnlineBatch] SET ContributionsJson = $contributionsJson WHERE BatchId = $batchId",
                                  new Dictionary<string, string> {
                                            { "$contributionsJson", JsonSerializer.Serialize(contributions) },
                                            { "$batchId", batch.Id}
                                  } );
                }
                catch ( Exception ex )
                {
                    Debug.WriteLine( ex.Message );
                    ManualBatchQueue.Enqueue( batch );
                }
            }
            else
            {
                break;
            }
        }
    }
}
