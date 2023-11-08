using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.Onrealm.Contracts;
using lib.Onrealm.Manager;

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

        var requestManager = new RequestManager( cookie );

        Debug.WriteLine( $"Loading Funds" );
        var funds = requestManager.GetFundsListAsync();
        await foreach ( var fund in funds )
        {
            Funds.Add( fund );
        }

        Debug.WriteLine( $"Loading Manual Batches" );
        var manualBatches = requestManager.GetManualBatchListAsync();
        await foreach ( var batch in manualBatches )
        {
            ManualBatchQueue.Enqueue( batch );
            ManualBatches.Add( batch );
        }

        Debug.WriteLine( $"Loading Online Batches" );
        var onlineBatches = requestManager.GetOnlineBatchListAsync();
        await foreach ( var batch in onlineBatches )
        {
            OnlineBatchQueue.Enqueue( batch );
            OnlineBatches.Add( batch );
        }

        Debug.WriteLine( $"Loading Refunds" );
        Refunds = await requestManager.GetRefundsAsync();

        var series = Enumerable.Range( 1, 5 ).ToList();


        var manualTasks = new List<Task>();
        foreach ( var i in series )
        {
            manualTasks.Add( ProcessManualContributions( cookie ) );
        }
        await Task.WhenAll( manualTasks );
        ManualContributions.AddRange( ManualContributionsQueue );
        ManualContributionsQueue.Clear();


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
                foreach ( var contribution in contributions )
                {
                    ManualContributionsQueue.Enqueue( contribution );
                }
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

                Debug.WriteLine( $"Loading Online Contributions For {batch.Id}" );
                var contributions = await requestManager.GetOnlineBatchContributionsAsync( batch.Id );
                foreach ( var contribution in contributions )
                {
                    OnlineContributionsQueue.Enqueue( contribution );
                }

            }
            else
            {
                break;
            }
        }
    }
}
