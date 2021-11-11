using System.Collections.Generic;
using System.Globalization;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests
{
    public class MockCreator : Trakx.Utils.Testing.MockCreator
    {
        public MockCreator(ITestOutputHelper output)
            : base(output)
        {
        }

        public TransactionRequest GetRandomTransaction(bool flag1, bool flag2, bool flag3)
        {
            return new TransactionRequest
            {
                Amount = 0.1,
                Destination = flag1 ? new DestinationTransferPeerPath
                {
                    Id = "0",
                    Type = TransferPeerPathType.VAULT_ACCOUNT,
                } : null,
                Fee = 0.1,
                Note = "nonono",
                Operation = TransactionOperation.SUPPLY_TO_COMPOUND,
                Source = flag3 ? new TransferPeerPath
                {
                    Id = "0",
                    Type = TransferPeerPathType.VAULT_ACCOUNT,
                } : null,
                AutoStaking = true,
                AssetId = "ETH_TEST",
                CpuStaking = GetRandomDecimals(),
                FeeLevel = TransactionRequestFeeLevel.LOW,
                GasLimit = 0.1,
                GasPrice = 0.1,
                MaxFee = "1",
                NetworkFee = 0.1,
                NetworkStaking = GetRandomDecimals(),
                CustomerRefId = GetRandomString(5),
                FailOnLowFee = true
            };
        }
        
        
    }
}