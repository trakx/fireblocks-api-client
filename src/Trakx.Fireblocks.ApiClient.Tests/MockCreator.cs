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

        public TransactionRequest GetRandomTransaction()
        {
            return new TransactionRequest
            {
                Amount = GetRandomDecimals(),
                Destination = new DestinationTransferPeerPath
                {
                    Id = GetRandomString(10),
                    Type = TransferPeerPathType.VAULT_ACCOUNT,
                },
                Destinations = new()
                {
                    new()
                    {
                        Amount = GetRandomValue().ToString(CultureInfo.InvariantCulture),
                        Destination = new DestinationTransferPeerPath
                        {
                            Id = GetRandomString(10),
                            Type = TransferPeerPathType.ONE_TIME_ADDRESS,
                            OneTimeAddress = new OneTimeAddress
                            {
                                Address = GetRandomString(5),
                                Tag = GetRandomString(5),
                            },
                        },
                    }
                },
                Fee = GetRandomDecimals(),
                Note = GetRandomString(40),
                Operation = TransactionOperation.SUPPLY_TO_COMPOUND,
                Source = new TransferPeerPath
                {
                    Id = GetRandomString(10),
                    Type = TransferPeerPathType.FIAT_ACCOUNT
                },
                AutoStaking = true,
                AssetId = GetRandomString(3),
                CpuStaking = GetRandomDecimals(),
                FeeLevel = TransactionRequestFeeLevel.HIGH,
                GasLimit = GetRandomDecimals(),
                GasPrice = GetRandomDecimals(),
                MaxFee = GetRandomValue().ToString(CultureInfo.InvariantCulture),
                NetworkFee = GetRandomDecimals(),
                NetworkStaking = GetRandomDecimals(),
                CustomerRefId = GetRandomString(5),
                FailOnLowFee = true
            };
        }
        
        
    }
}